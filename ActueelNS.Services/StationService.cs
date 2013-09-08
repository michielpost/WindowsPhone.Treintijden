﻿using ActueelNS.Services.Interfaces;
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
        private List<Station> _stationListAll = null;

        public List<Station> GetStations(bool searchInAll = false)
        {
            if (!searchInAll && _stationList != null)
                return _stationList;
            else if (searchInAll && _stationListAll != null)
                return _stationListAll;
            else
            {

                var fileName = "Data/stations.xml";
                if(searchInAll)
                    fileName = "Data/stations_all.xml";

                XElement stationXmlElement = XDocument.Load(fileName).Elements().First();

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

                if (!searchInAll)
                {
                    _stationList = list.ToList();
                    return _stationList;

                }
                else
                {
                    _stationListAll = list.ToList();
                    return _stationListAll;
                }

            }

        }

     
        public Station GetStationByName(string name)
        {

            var allStations = GetStations();

            return allStations.Where(x => x.Name == name).FirstOrDefault();
        }

        public Station GetStationByCode(string code)
        {
            code = code.ToLower();

            var allStations = GetStations();

            var result =  allStations.Where(x => x.Code.ToLower() == code).FirstOrDefault();

            if (result == null)
            {
                var allForeignStations = GetStations(true);

                result = allForeignStations.Where(x => x.Code.ToLower() == code).FirstOrDefault();

            }

            return result;
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




        public IList<Station> GetStationsByCode(IEnumerable<string> stationCodes)
        {
            var allStations = GetStations();

            var result = allStations.Where(x => stationCodes.Contains(x.Code.ToLower())).ToList();

            return result;
        }
    }
}
