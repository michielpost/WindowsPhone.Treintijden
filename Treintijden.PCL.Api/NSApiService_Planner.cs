using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Treintijden.PCL.Api.Constants;
using Treintijden.PCL.Api.Helpers;
using Treintijden.PCL.Api.Interfaces;
using Treintijden.PCL.Api.Models;

namespace Treintijden.PCL.Api
{
    public partial class NSApiService : INSApiService
    {
        public async Task<List<ReisMogelijkheid>> GetSearchResult(PlannerSearch search)
        {
          
            string stringDateTime = search.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + "T" + search.Time.ToString("HH:mm:ss", CultureInfo.InvariantCulture);

            string query = string.Format("previousAdvices=5&nextAdvices=5&fromStation={0}&toStation={1}&dateTime={2}", search.VanStation.Code, search.NaarStation.Code, stringDateTime);

            if (search.ViaStation != null)
                query += "&viaStation=" + search.ViaStation.Code;

            if (search.Type != null && search.Type.ToLower() == "vertrek")
                query += "&departure=true";
            else
                query += "&departure=false";


            query += string.Format("&hslAllowed={0}&yearCard={1}", search.IsHogesnelheid, search.IsYearCard);

            Uri address = new Uri(string.Format(NSApi.BaseUrl + "/ns-api-treinplanner?{0}&a={1}", query, DateTime.Now.Ticks), UriKind.Absolute);
            HttpClient webClient = new HttpClient();

            string response = await webClient.GetStringAsync(address);

            return await Task.Run(() =>
            {

                XElement tijdenXmlElement = XElement.Parse(response);

                List<ReisMogelijkheid> reismogelijkheidList = new List<ReisMogelijkheid>();

                foreach (var element in tijdenXmlElement.Descendants("ReisMogelijkheid"))
                {
                    ReisMogelijkheid mogelijkheid = new ReisMogelijkheid();

                    if (element.Element("AantalOverstappen") != null)
                        mogelijkheid.AantalOverstappen = int.Parse(element.Element("AantalOverstappen").Value);

                    mogelijkheid.GeplandeVertrekTijd = XmlHelper.GetDateTime(element, "GeplandeVertrekTijd") ?? DateTime.Now;
                    mogelijkheid.ActueleVertrekTijd = XmlHelper.GetDateTime(element, "ActueleVertrekTijd") ?? DateTime.Now;

                    mogelijkheid.GeplandeAankomstTijd = XmlHelper.GetDateTime(element, "GeplandeAankomstTijd") ?? DateTime.Now;
                    mogelijkheid.ActueleAankomstTijd = XmlHelper.GetDateTime(element, "ActueleAankomstTijd") ?? DateTime.Now;

                    mogelijkheid.GeplandeReisTijd = XmlHelper.GetElementText(element.Element("GeplandeReisTijd"));
                    mogelijkheid.Status = XmlHelper.GetElementText(element.Element("Status"));


                    if (element.Element("Optimaal") != null)
                        mogelijkheid.Optimaal = bool.Parse(element.Element("Optimaal").Value);

                    mogelijkheid.ReisDelen = new List<ReisDeel>();

                    bool isAlternate = false;

                    foreach (var reisdeelElement in element.Descendants("ReisDeel"))
                    {
                        ReisDeel deel = new ReisDeel();
                        deel.IsAlternate = isAlternate;
                        isAlternate = !isAlternate;

                        deel.VervoerType = XmlHelper.GetElementText(reisdeelElement.Element("VervoerType"));
                        deel.Vervoerder = XmlHelper.GetElementText(reisdeelElement.Element("Vervoerder"));
                        deel.Status = XmlHelper.GetElementText(reisdeelElement.Element("Status"));

                        deel.ReisStops = new List<ReisStop>();
                        foreach (var stopElement in reisdeelElement.Descendants("ReisStop"))
                        {
                            ReisStop stop = new ReisStop();

                            stop.Naam = XmlHelper.GetElementText(stopElement.Element("Naam"));
                            stop.Vertrekspoor = XmlHelper.GetElementText(stopElement.Element("Spoor"));
                            stop.Tijd = XmlHelper.GetDateTime(stopElement, "Tijd");


                            if (stopElement.Element("Spoor") != null
                                && stopElement.Element("Spoor").Attribute("wijziging") != null)
                            {
                                stop.IsVertrekspoorWijziging = bool.Parse(stopElement.Element("Spoor").Attribute("wijziging").Value);
                            }


                            deel.ReisStops.Add(stop);
                        }

                        mogelijkheid.ReisDelen.Add(deel);

                    }

                    //Set vertrek vertraging
                    var first = mogelijkheid.ReisDelen.FirstOrDefault();
                    if (first != null && first.FirstStop != null)
                    {
                        first.FirstStop.VertragingTekst = mogelijkheid.VertrekVertraging;
                    }

                    //Set aankomst vertraging
                    var last = mogelijkheid.ReisDelen.LastOrDefault();
                    if (last != null && last.FirstStop != null)
                    {
                        last.LastStop.VertragingTekst = mogelijkheid.AankomstVertraging;
                    }

                    //Delete vervoerders als het NS is. Tenzij het met iets anders begint. (Bijv Arriva, dan NS daarna wel tonen)
                    foreach (var deel in mogelijkheid.ReisDelen)
                    {
                        if (deel.Vervoerder.ToLower() == "ns" || deel.Vervoerder.ToLower().Contains("spoorwegen"))
                            deel.Vervoerder = null;
                        else
                            break;
                    }

                    //TODO: Perm store reis result
                    //PermStoreSearchResult(search.Id, reismogelijkheidList);

                    reismogelijkheidList.Add(mogelijkheid);
                }

                return reismogelijkheidList;
            });
        }

     
    }
}
