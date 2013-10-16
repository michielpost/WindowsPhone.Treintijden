using System.Linq;
using System.Collections.Generic;
using Treintijden.Shared.Services.Interfaces;
using Treintijden.PCL.Api.Models;
using TEMP;

namespace Treintijden.Shared.Services
{
    public class StationService : IStationService
    {
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
