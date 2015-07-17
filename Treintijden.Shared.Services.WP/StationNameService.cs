using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using Treintijden.PCL.Api.Models;
using Treintijden.Shared.Services.Interfaces;

namespace Treintijden.Shared.Services
{
    public class StationNameService : IStationNameService
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
                                Sort = int.Parse(x.Element("t").Value),
                                NamesExtra = x.Descendants("Sy").Select(s => s.Value).ToArray(),
                                Country = x.Element("c") != null ? x.Element("c").Value : null,
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
          name = name.ToLower();

          var allStations = GetStations();

          var station = allStations.Where(x => x.Name.ToLower() == name).FirstOrDefault();

          if (station != null)
            return station;
          else
          {
            //Try international
            allStations = GetStations(true);

            return allStations.Where(x => x.Name.ToLower() == name).FirstOrDefault();
          }
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

        public IList<Station> GetStationsByCode(IEnumerable<string> stationCodes)
        {
            var allStations = GetStations();

            var result = allStations.Where(x => stationCodes.Contains(x.Code.ToLower())).ToList();

            return result;
        }
    }
}
