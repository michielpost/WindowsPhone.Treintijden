using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ActueelNS.Services.Interfaces;
using Q42.WinRT.Storage;
using Treintijden.Shared.Services.Interfaces;
using Treintijden.PCL.Api.Models;

namespace Treintijden.Shared.Services
{
    public class LastStationService : ILastStationService
    {
        private readonly string fileName = "last_stations";

        StorageHelper<string[]> sh = new StorageHelper<string[]>(Windows.Storage.ApplicationData.Current.LocalFolder);

        public IStationNameService StationNameService { get; set; }

        public List<Station> AllInMemmory { get; set; }

        public LastStationService(IStationNameService stationNameService)
        {
            StationNameService = stationNameService;
        }

        public async Task<List<Station>> Add(Station station1, Station station2, Station station3)
        {
            var current = await GetAll();

            if (current == null)
                current = new List<Station>();

            current.Reverse();

            current = AddSingleStation(station1, current);
            current = AddSingleStation(station2, current);
            current = AddSingleStation(station3, current);

            current.Reverse();
            current = current.Take(10).ToList();

            await sh.SaveAsync(current.Select(x => x.Code).ToArray(), fileName);

            AllInMemmory = current;

            return current;

        }

        private static List<Station> AddSingleStation(Station station1, List<Station> current)
        {
            if (station1 != null)
            {
                current = current.Where(x => x.Code != station1.Code).ToList();

                current.Add(station1);
            }

            return current;
        }

        public async Task<List<Station>> GetAll()
        {
            try
            {
                if (AllInMemmory != null)
                    return AllInMemmory;

                var load = await sh.LoadAsync(fileName);

                if (load == null)
                    return null;

                var all = StationNameService.GetStations();

                var filtered = new List<Station>();

                foreach (var l in load)
                {
                    var s = all.Where(x => x.Code == l).FirstOrDefault();
                    if (s != null)
                        filtered.Add(s);
                }

                AllInMemmory = filtered.ToList();

                return AllInMemmory;

            }
            catch
            {
                return null;
            }
        }
    }
}
