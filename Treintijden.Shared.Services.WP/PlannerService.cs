using System;
using System.Collections.Generic;
using Treintijden.Shared.Services.Interfaces;
using Treintijden.PCL.Api.Models;
using System.Linq;
using Q42.WinRT.Storage;
using System.Threading.Tasks;

namespace Treintijden.Shared.Services
{
    public class PlannerService : IPlannerService
    {

      public async Task<List<PlannerSearch>> GetListFromStoreAsync()
      {
        try
        {
          var sh = new StorageHelper<List<PlannerSearch>>(Windows.Storage.ApplicationData.Current.LocalFolder, serializerType: StorageSerializer.XML);
          var list = await sh.LoadAsync("planning_searches");

          if (list == null)
            list = new List<PlannerSearch>();

          return list;
        }
        catch
        {
          return new List<PlannerSearch>();
        }
      }

      private async Task SaveListToStoreAsync(List<PlannerSearch> list, int count = 20)
      {
        try
        {
          //Always save ordered
          List<PlannerSearch> saveList = list.OrderByDescending(x => x.SearchDateTime).Take(count).ToList();

          var notSaved = list.OrderByDescending(x => x.SearchDateTime).Skip(count).ToList();
          if (notSaved.Count > 0)
          {
            //Delete all searches from perm isolated storage
            //Except saved searched

            var permSearches = (await GetPermListFromStoreAsync()).Select(x => x.Id).ToList();

            notSaved = notSaved.Where(x => !permSearches.Contains(x.Id)).ToList();

            foreach (var item in notSaved)
            {
              await DeletePermStoredSearchResultAsync(item.Id);
            }
          }

          var sh = new StorageHelper<List<PlannerSearch>>(Windows.Storage.ApplicationData.Current.LocalFolder, serializerType: StorageSerializer.XML);
          await sh.SaveAsync(saveList, "planning_searches");
        }
        catch { }
      }

      public async Task<List<PlannerSearch>> GetPermListFromStoreAsync()
      {
        try
        {
          var sh = new StorageHelper<List<PlannerSearch>>(Windows.Storage.ApplicationData.Current.LocalFolder, serializerType: StorageSerializer.XML);
          var list = await sh.LoadAsync("perm_searches");

          if (list == null)
            list = new List<PlannerSearch>();

          return list;
        }
        catch
        {
          return new List<PlannerSearch>();
        }

      }

      private Task SavePermListToStoreAsync(List<PlannerSearch> list)
      {
        try
        {
          var sh = new StorageHelper<List<PlannerSearch>>(Windows.Storage.ApplicationData.Current.LocalFolder, serializerType: StorageSerializer.XML);
          return sh.SaveAsync(list, "perm_searches");
        }
        catch {
          return Task.FromResult("");
        }
      }

        public async Task AddSearchAsync(PlannerSearch search)
        {
          List<PlannerSearch> list = await GetListFromStoreAsync();

            if (!list.Where(x => x.Id == search.Id).Any())
            {
                list.Add(search);

                await SaveListToStoreAsync(list);
            }
        }

        public async Task DeleteSearchAsync(Guid id)
        {
          List<PlannerSearch> list = await GetListFromStoreAsync();

            var search = list.Where(x => x.Id == id).FirstOrDefault();
            if (search != null)
                list.Remove(search);

            await SaveListToStoreAsync(list);

            //Can delete perm result?

            list = await GetPermListFromStoreAsync();
            search = list.Where(x => x.Id == id).FirstOrDefault();

            if (search == null)
            {
                //Safe delete from perm result
              await DeletePermStoredSearchResultAsync(id);

            }
        }

        public async Task<PlannerSearch> GetSearchAsync(Guid id)
        {
          List<PlannerSearch> list = await GetListFromStoreAsync();

            var search = list.Where(x => x.Id == id).FirstOrDefault();

            if(search == null)
            {
              list = await GetPermListFromStoreAsync();

                search = list.Where(x => x.Id == id).FirstOrDefault();
            }

            return search;
        }

        public Task<List<ReisMogelijkheid>> GetPermStoreSearchResultAsync(Guid id)
        {
          try
          {
            var sh = new StorageHelper<List<ReisMogelijkheid>>(Windows.Storage.ApplicationData.Current.LocalFolder, serializerType: StorageSerializer.XML);

            return sh.LoadAsync(string.Format("SearchResult_{0}", id));
          }
          catch
          {
            return Task.FromResult(new List<ReisMogelijkheid>());
          }
        }

        public async Task AddPermSearchAsync(PlannerSearch search, List<ReisMogelijkheid> mogelijkheden)
        {
          List<PlannerSearch> list = await GetPermListFromStoreAsync();

            if (!list.Where(x => x.Id == search.Id).Any())
            {
                list.Add(search);

                await SavePermListToStoreAsync(list);

                await PermStoreSearchResultAsync(search.Id, mogelijkheden);
            }
        }


        public async Task PermStoreSearchResultAsync(Guid id, List<ReisMogelijkheid> mogelijkheden)
        {
            try
            {
              var sh = new StorageHelper<List<ReisMogelijkheid>>(Windows.Storage.ApplicationData.Current.LocalFolder, serializerType: StorageSerializer.XML);

              await sh.SaveAsync(mogelijkheden, string.Format("SearchResult_{0}", id));
            }
            catch { }
        }

        private async Task DeletePermStoredSearchResultAsync(Guid id)
        {
            try
            {
              var sh = new StorageHelper<string>(Windows.Storage.ApplicationData.Current.LocalFolder, serializerType: StorageSerializer.XML);

                await sh.DeleteAsync(string.Format("SearchResult_{0}", id));
            }
            catch { }
        }


        public async Task DeleteSearchHistoryAsync()
        {
          await SaveListToStoreAsync(await GetListFromStoreAsync(), 0);
        }
    }
}
