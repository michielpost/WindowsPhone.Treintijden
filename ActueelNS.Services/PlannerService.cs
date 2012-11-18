using System;
using System.Net;
using ActueelNS.Services.Interfaces;
using ActueelNS.Services.Models;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Threading.Tasks;
using ActueelNS.Services.Constants;

namespace ActueelNS.Services
{
    public class PlannerService : IPlannerService
    {

        public List<PlannerSearch> GetListFromStore()
        {
            var list = IsolatedStorageCacheManager<List<PlannerSearch>>.Retrieve("planning_searches.xml");

            if (list == null)
                list = new List<PlannerSearch>();

            return list;
        }

        private void SaveListToStore(List<PlannerSearch> list, int count = 20)
        {
            //Always save ordered
            List<PlannerSearch> saveList = list.OrderByDescending(x => x.SearchDateTime).Take(count).ToList();

            var notSaved = list.OrderByDescending(x => x.SearchDateTime).Skip(count).ToList();
            if (notSaved.Count > 0)
            {
                //Delete all searches from perm isolated storage
                //Except saved searched

                var permSearches = GetPermListFromStore().Select(x => x.Id).ToList();

                notSaved = notSaved.Where(x => !permSearches.Contains(x.Id)).ToList();

                foreach (var item in notSaved)
                {
                    DeletePermStoredSearchResult(item.Id);
                }
            }

            IsolatedStorageCacheManager<List<PlannerSearch>>.Store("planning_searches.xml", saveList);
        }

       

        public List<PlannerSearch> GetPermListFromStore()
        {
            var list = IsolatedStorageCacheManager<List<PlannerSearch>>.Retrieve("perm_searches.xml");

            if (list == null)
                list = new List<PlannerSearch>();

            return list;
        }

        private void SavePermListToStore(List<PlannerSearch> list)
        {
            IsolatedStorageCacheManager<List<PlannerSearch>>.Store("perm_searches.xml", list);
        }

        public void AddSearch(PlannerSearch search)
        {
            List<PlannerSearch> list = GetListFromStore();

            list.Add(search);
           
            SaveListToStore(list);
        }

        public void DeleteSearch(Guid id)
        {
            List<PlannerSearch> list = GetListFromStore();

            var search = list.Where(x => x.Id == id).FirstOrDefault();
            if (search != null)
                list.Remove(search);

            SaveListToStore(list);

            //Can delete perm result?

            list = GetPermListFromStore();
            search = list.Where(x => x.Id == id).FirstOrDefault();

            if (search == null)
            {
                //Safe delete from perm result
                DeletePermStoredSearchResult(id);

            }
        }

        public PlannerSearch GetSearch(Guid id)
        {
            List<PlannerSearch> list = GetListFromStore();

            var search = list.Where(x => x.Id == id).FirstOrDefault();

            if(search == null)
            {
                list = GetPermListFromStore();

                search = list.Where(x => x.Id == id).FirstOrDefault();
            }

            return search;
        }


        public void AddPermSearch(PlannerSearch search, List<ReisMogelijkheid> mogelijkheden)
        {
            List<PlannerSearch> list = GetPermListFromStore();

            if (!list.Where(x => x.Id == search.Id).Any())
            {
                list.Add(search);

                SavePermListToStore(list);

                PermStoreSearchResult(search.Id, mogelijkheden);
            }
        }
              

        public void PermStoreSearchResult(Guid id, List<ReisMogelijkheid> mogelijkheden)
        {
            try
            {
                IsolatedStorageCacheManager<List<ReisMogelijkheid>>.Store(string.Format("/SearchResult_{0}.xml", id), mogelijkheden);
            }
            catch { }
        }

        public List<ReisMogelijkheid> GetPermStoreSearchResult(Guid id)
        {
            return IsolatedStorageCacheManager<List<ReisMogelijkheid>>.Retrieve(string.Format("/SearchResult_{0}.xml", id));
        }


        private void DeletePermStoredSearchResult(Guid id)
        {
            try
            {
                IsolatedStorageCacheManager<string>.Delete(string.Format("/SearchResult_{0}.xml", id));
            }
            catch { }
        }



        public async Task<List<ReisMogelijkheid>> GetSearchResult(PlannerSearch search)
        {
            try
            {
                var result = GetPermStoreSearchResult(search.Id);
                if (result != null)
                    return result;
            }
            catch (Exception e)
            { }


            string stringDateTime = search.Date.ToString("yyyy-MM-dd") + "T" + search.Time.ToString("HH:mm:ss");

            string query = string.Format("previousAdvices=5&nextAdvices=5&fromStation={0}&toStation={1}&dateTime={2}", search.VanStation.Code, search.NaarStation.Code, stringDateTime);

            if (search.ViaStation != null)
                query += "&viaStation=" + search.ViaStation.Code;

            if (search.Type != null && search.Type.ToLower() == "vertrek")
                query += "&departure=true";
            else
                query += "&departure=false";


            query += string.Format("&hslAllowed={0}&yearCard={1}", search.IsHogesnelheid, search.IsYearCard);


            Uri address = new Uri(string.Format(NSApi.BaseUrl + "/ns-api-treinplanner?{0}&a={1}", query, DateTime.Now.Ticks), UriKind.Absolute);
            WebClient webclient = new WebClient();
            //webclient.Credentials = new NetworkCredential(NSApi.Login, NSApi.Password);

            string response = await webclient.DownloadStringTaskAsync(address);

           return await Task.Run(() =>
                {

                    XElement tijdenXmlElement = XElement.Parse(response);

                    List<ReisMogelijkheid> reismogelijkheidList = new List<ReisMogelijkheid>();

                    foreach (var element in tijdenXmlElement.Descendants("ReisMogelijkheid"))
                    {
                        ReisMogelijkheid mogelijkheid = new ReisMogelijkheid();

                        if (element.Element("AantalOverstappen") != null)
                            mogelijkheid.AantalOverstappen = int.Parse(element.Element("AantalOverstappen").Value);

                        mogelijkheid.GeplandeVertrekTijd = GetDateTime(element, "GeplandeVertrekTijd");
                        mogelijkheid.ActueleVertrekTijd = GetDateTime(element, "ActueleVertrekTijd");

                        mogelijkheid.GeplandeAankomstTijd = GetDateTime(element, "GeplandeAankomstTijd");
                        mogelijkheid.ActueleAankomstTijd = GetDateTime(element, "ActueleAankomstTijd");

                        mogelijkheid.GeplandeReisTijd = GetElementText(element.Element("GeplandeReisTijd"));

                        if(element.Element("Optimaal") != null)
                            mogelijkheid.Optimaal = bool.Parse(element.Element("Optimaal").Value);

                        mogelijkheid.ReisDelen = new List<ReisDeel>();

                        bool isAlternate = false;

                        foreach (var reisdeelElement in element.Descendants("ReisDeel"))
                        {
                            ReisDeel deel = new ReisDeel();
                            deel.IsAlternate = isAlternate;
                            isAlternate = !isAlternate;

                            deel.VervoerType = GetElementText(reisdeelElement.Element("VervoerType"));

                            deel.ReisStops = new List<ReisStop>();
                            foreach (var stopElement in reisdeelElement.Descendants("ReisStop"))
                            {
                                ReisStop stop = new ReisStop();

                                stop.Naam = GetElementText(stopElement.Element("Naam"));
                                stop.Vertrekspoor = GetElementText(stopElement.Element("Spoor"));
                                stop.Tijd = GetDateTime(stopElement, "Tijd");


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
                            last.FirstStop.VertragingTekst = mogelijkheid.AankomstVertraging;
                        }

                        reismogelijkheidList.Add(mogelijkheid);
                    }

                    PermStoreSearchResult(search.Id, reismogelijkheidList);

                    return reismogelijkheidList;
               });
        }

        private static DateTime GetDateTime(XElement element, string name)
        {
            DateTime dtime = DateTime.Now;
            if (element.Element(name) != null)
            {
                string time = element.Element(name).Value;
                int zoneIndex = time.LastIndexOf('+');
                if (zoneIndex > 0)
                {
                    time = time.Substring(0, zoneIndex);
                }

                dtime = DateTime.Parse(time);
            }

            return dtime;
        }


        private string GetElementText(XElement element)
        {
            if (element != null)
                return element.Value;


            return null;
        }


        public void DeleteSearchHistory()
        {
            SaveListToStore(GetListFromStore(), 0);
        }
    }
}
