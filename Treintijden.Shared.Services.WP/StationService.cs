using System.Linq;
using System.Collections.Generic;
using Treintijden.Shared.Services.Interfaces;
using Treintijden.PCL.Api.Models;
using Q42.WinRT.Storage;
using System.Threading.Tasks;

namespace Treintijden.Shared.Services
{
    public class StationService : IStationService
    {
      public Task<List<Station>> GetMyStationsAsync()
        {
          return GetListFromStoreAsync();
        }

      private async Task<List<Station>> GetListFromStoreAsync()
        {
          var sh = new StorageHelper<List<Station>>(Windows.Storage.ApplicationData.Current.LocalFolder, serializerType: StorageSerializer.XML);
          var list = await sh.LoadAsync("favorite");

            if(list == null)
                list = new List<Station>();

            return list;
        }

      private Task SaveListToStoreAsync(List<Station> stations)
        {
            //Always save ordered
            stations = stations.OrderBy(x => x.Name).ToList();

            var sh = new StorageHelper<List<Station>>(Windows.Storage.ApplicationData.Current.LocalFolder, serializerType: StorageSerializer.XML);
            return sh.SaveAsync(stations, "favorite");
        }

      public async Task AddStationAsync(Station station)
        {
          List<Station> stations = await GetListFromStoreAsync();

          if (!stations.Where(x => x.Name == station.Name).Any())
            stations.Add(station);

          await SaveListToStoreAsync(stations);
        }

      public async Task DeleteStationAsync(string name)
        {
          List<Station> stations = await GetListFromStoreAsync();

          var station = stations.Where(x => x.Name == name).FirstOrDefault();
          if (station != null)
            stations.Remove(station);

          await SaveListToStoreAsync(stations);
        }

      public Task ClearAsync()
        {
          return SaveListToStoreAsync(new List<Station>());
        }




    }
}
