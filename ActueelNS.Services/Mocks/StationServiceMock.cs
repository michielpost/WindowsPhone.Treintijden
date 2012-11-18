using ActueelNS.Services.Interfaces;
using ActueelNS.Services.Models;
using System.Collections.Generic;

namespace ActueelNS.Services.Mocks
{
    public class StationServiceMock : IStationService
    {

        public List<Station> GetStations(string country)
        {
            List<Station> stationList = new List<Station>();

            var d = new Station()
            {
                Code = "Dt",
                Name = "Delft"
            };
            d.SetDistance(1000);

            stationList.Add(d);

            stationList.Add(new Station()
            {
                Code = "Dt",
                Name = "Amsterdam"
            });

            stationList.Add(new Station()
            {
                Code = "Dt",
                Name = "Rijswijk"
            });

            return stationList;
        }

        public IList<Station> GetMyStations()
        {
            List<Station> stationList = new List<Station>();

            stationList.Add(new Station()
            {
                Code = "Dt",
                Name = "Delft"
            });

            stationList.Add(new Station()
            {
                Code = "Dt",
                Name = "Amsterdam"
            });

            stationList.Add(new Station()
            {
                Code = "Dt",
                Name = "Rijswijk"
            });

            return stationList;

        }

        public void AddStation(Station name)
        {
        }

        public void DeleteStation(string name)
        {
        }


        public Station GetStationByName(string name)
        {
            return null;
        }

        public Station GetStationByCode(string name)
        {
            return null;
        }

        public void Test()
        {
        }
    }
}
