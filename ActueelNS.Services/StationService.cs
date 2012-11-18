using ActueelNS.Services.Interfaces;
using System.Xml.Linq;
using System.Linq;
using ActueelNS.Services.Models;
using System.Collections.Generic;
using System.Globalization;

namespace ActueelNS.Services
{
    public class StationService : IStationService
    {
        private List<Station> _stationList = null;

        public List<Station> GetStations(string country)
        {
            if (_stationList != null)
                return _stationList;
            else
            {

                //StreamResourceInfo xml = Application.GetResourceStream(new Uri("/ActueelNS.Services;component/Data/stations.xml", System.UriKind.Relative));
                //XElement stationXmlElement = XElement.Load(xml.Stream);

                XElement stationXmlElement = XDocument.Load("Data/stations.xml").Elements().First();

                var list = (from x in stationXmlElement.Descendants("s")                            
                            select new Station
                            {
                                Name = x.Element("name").Value,
                                Code = x.Element("id").Value,
                                NamesExtra = x.Descendants("Sy").Select(s => s.Value).ToArray(),
                                //Country = x.Element("country").Value,
                                //Alias = bool.Parse(x.Element("alias").Value),
                                Lat = double.Parse(x.Element("lat").Value, CultureInfo.InvariantCulture),
                                Long = double.Parse(x.Element("lon").Value, CultureInfo.InvariantCulture)
                            });

                _stationList = list.ToList();

                return _stationList;

            }

        }

        public void Test()
        {
            //var start = DateTime.Now;
            //for (int i = 0; i < 20; i++)
            //{
            //    StreamResourceInfo xml = Application.GetResourceStream(new Uri("/ActueelNS.Services;component/Data/stations_all.xml", System.UriKind.Relative));

            //    XElement stationXmlElement = XElement.Load(xml.Stream);

            //    var list = (from x in stationXmlElement.Descendants("station")
            //                where x.Element("country").Value == "NL"
            //                && x.Element("alias").Value == "false"
            //                select new Station
            //                {
            //                    Name = x.Element("name").Value,
            //                    Code = x.Element("code").Value,
            //                    //Country = x.Element("country").Value,
            //                    //Alias = bool.Parse(x.Element("alias").Value),
            //                    //Lat = x.Element("lat").Value,
            //                    //Long = x.Element("long").Value
            //                });

            //    _stationList = list.ToList();
                
            //}
            //var stop = DateTime.Now;

            //MessageBox.Show((stop - start).ToString());

            //var startA = DateTime.Now;
            //for (int i = 0; i < 20; i++)
            //{
            //    StreamResourceInfo xml = Application.GetResourceStream(new Uri("/ActueelNS.Services;component/Data/stations.xml", System.UriKind.Relative));

            //    XElement stationXmlElement = XElement.Load(xml.Stream);
                             

            //    var list = (from x in stationXmlElement.Descendants("station")
            //                where x.Element("country").Value == "NL"
            //                 && x.Element("alias").Value == "false"
            //                select new Station
            //                {
            //                    Name = x.Element("name").Value,
            //                    Code = x.Element("code").Value,
            //                    //Country = x.Element("country").Value,
            //                    //Alias = bool.Parse(x.Element("alias").Value),
            //                    //Lat = x.Element("lat").Value,
            //                    //Long = x.Element("long").Value
            //                });

            //    _stationList = list.ToList();

            //}
            //var stopA = DateTime.Now;

            //MessageBox.Show((stopA - startA).ToString());
        }

        public Station GetStationByName(string name)
        {

            var allStations = GetStations("NL");

            return allStations.Where(x => x.Name == name).FirstOrDefault();
        }

        public Station GetStationByCode(string code)
        {

            var allStations = GetStations("NL");

            return allStations.Where(x => x.Code == code).FirstOrDefault();
        }


        public IList<Station> GetMyStations()
        {
            //TaskEx.Delay(TimeSpan.FromSeconds(5)).Wait();  
 
            List<Station> names = GetListFromStore();

            return names;
        }

        private List<Station> GetListFromStore()
        {
            var list = IsolatedStorageCacheManager<List<Station>>.Retrieve("favorite.xml");

            if(list == null)
                list = new List<Station>();

            return list;
        }

        private void SaveListToStore(List<Station> stations)
        {
            //Always save ordered
            stations = stations.OrderBy(x => x.Name).ToList();

            IsolatedStorageCacheManager<List<Station>>.Store("favorite.xml", stations);
        }

        public void AddStation(Station station)
        {
            List<Station> stations = GetListFromStore();

            if (!stations.Where(x => x.Name == station.Name).Any())
                stations.Add(station);

            SaveListToStore(stations);
        }

        public void DeleteStation(string name)
        {
            List<Station> stations = GetListFromStore();

            var station = stations.Where(x => x.Name == name).FirstOrDefault();
            if (station != null)
                stations.Remove(station);

            SaveListToStore(stations);
        }

        
    }
}
