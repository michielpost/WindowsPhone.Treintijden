using System;
using System.Collections.Generic;
using Treintijden.Shared.Services.Interfaces;
using Treintijden.PCL.Api.Models;
using TEMP;
using System.Linq;

namespace Treintijden.Shared.Services
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

            if (!list.Where(x => x.Id == search.Id).Any())
            {
                list.Add(search);

                SaveListToStore(list);
            }
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

        public List<ReisMogelijkheid> GetPermStoreSearchResult(Guid id)
        {
            return IsolatedStorageCacheManager<List<ReisMogelijkheid>>.Retrieve(string.Format("/SearchResult_{0}.xml", id));
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

        private void DeletePermStoredSearchResult(Guid id)
        {
            try
            {
                IsolatedStorageCacheManager<string>.Delete(string.Format("/SearchResult_{0}.xml", id));
            }
            catch { }
        }


        public void DeleteSearchHistory()
        {
            SaveListToStore(GetListFromStore(), 0);
        }
    }
}
