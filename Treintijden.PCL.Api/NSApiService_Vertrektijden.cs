using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Net.Http;
using Treintijden.PCL.Api.Interfaces;
using Treintijden.PCL.Api.Constants;
using Treintijden.PCL.Api.Models;
using Treintijden.PCL.Api.Helpers;

namespace Treintijden.PCL.Api
{
    public partial class NSApiService : INSApiService
    {
        public async Task<List<Vertrektijd>> GetVertrektijden(string station)
        {
            Uri address = new Uri(string.Format(NSApi.BaseUrl + "/ns-api-avt?station={0}&a={1}", station, DateTime.Now.Ticks), UriKind.Absolute);
            HttpClient webclient = new HttpClient();
            webclient.MaxResponseContentBufferSize = 9000000;

            //webclient.Credentials = new NetworkCredential(NSApi.Login, NSApi.Password);

            string response = await webclient.GetStringAsync(address);

            return await Task.Run(() =>
             {

            //System.Threading.Thread.Sleep(5000);

            XElement tijdenXmlElement = XElement.Parse(response);

            List<Vertrektijd> vertrektijdList = new List<Vertrektijd>();

            foreach (var element in tijdenXmlElement.Descendants("VertrekkendeTrein"))
            {
                Vertrektijd tijd = new Vertrektijd();

                if (element.Element("RitNummer") != null)
                    tijd.Ritnummer = int.Parse(element.Element("RitNummer").Value);

                if (element.Element("VertrekTijd") != null)
                {
                    string time = element.Element("VertrekTijd").Value;
                    int zoneIndex = time.LastIndexOf('+');
                    if (zoneIndex > 0)
                    {
                        time = time.Substring(0, zoneIndex);
                    }

                    tijd.Tijd = DateTime.Parse(time);
                }

                tijd.VertragingTekst = XmlHelper.GetElementText(element.Element("VertrekVertragingTekst"));
                tijd.Eindbestemming = XmlHelper.GetElementText(element.Element("EindBestemming"));
                tijd.TreinSoort = XmlHelper.GetElementText(element.Element("TreinSoort"));
                tijd.Route = XmlHelper.GetElementText(element.Element("RouteTekst"));
                tijd.ReisTip = XmlHelper.GetElementText(element.Element("ReisTip"));
                tijd.Vervoerder = XmlHelper.GetElementText(element.Element("Vervoerder"));

                tijd.Vertrekspoor = XmlHelper.GetElementText(element.Element("VertrekSpoor"));

                if (element.Element("VertrekSpoor") != null
                    && element.Element("VertrekSpoor").Attribute("wijziging") != null)
                {
                    tijd.IsVertrekspoorWijziging = bool.Parse(element.Element("VertrekSpoor").Attribute("wijziging").Value);
                }


                string sep = string.Empty;
                foreach (var opm in element.Descendants("Opmerking"))
                {
                    tijd.Opmerkingen += string.Format("{0}{1}", sep, opm.Value)
                        .Replace("\r", string.Empty)
                        .Replace("\t", string.Empty)
                        .Replace("\n", string.Empty)
                        .Trim();
                    sep = ", ";
                }



                vertrektijdList.Add(tijd);
            }



            if (vertrektijdList != null)
            {

              //var vervoerders = vertrektijdList.GroupBy(x => x.Vervoerder).Select(x => x.Key).ToList();

              bool showVervoerder = vertrektijdList.GroupBy(x => x.Vervoerder).Select(x => x.Key)
                .Where(x => !string.IsNullOrEmpty(x))
                .Where(x => !x.ToLower().Contains("hispeed"))
                .Where(x => !x.ToLower().Contains("spoorwegen"))
                .Count() > 1;


              bool _useAlternate = false;

              foreach (var tijd in vertrektijdList)
              {
                ////Set background color here, for performance
                tijd.IsAlternate = _useAlternate;
                _useAlternate = !_useAlternate;

                if (!showVervoerder)
                  tijd.Vervoerder = null;


              }
            }

            //await TaskEx.Delay(TimeSpan.FromSeconds(5));  

            return vertrektijdList;
            });

        }
    }
}
