﻿using Q42.WinRT.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treintijden.PCL.Api.Interfaces;
using Treintijden.PCL.Api.Models;
using Treintijden.Shared.Services.Interfaces;

namespace Treintijden.Shared.Services.WP
{
    public class CachedNSApiService : INSApiService
    {
        public INSApiService Original { get; set; }
        public IPlannerService PlannerService { get; set; }

        public CachedNSApiService(INSApiService original, IPlannerService plannerService)
        {
            Original = original;
            PlannerService = plannerService;
        }

        public async Task<List<ReisMogelijkheid>> GetSearchResult(PlannerSearch search)
        {
            try
            {
                var result = PlannerService.GetPermStoreSearchResult(search.Id);
                if (result != null)
                    return result;
            }
            catch
            { }

            var searchResult = await Original.GetSearchResult(search);

            try
            {
                PlannerService.PermStoreSearchResult(search.Id, searchResult);
            }
            catch{}

            return searchResult;
        }

        public Task<ReisPrijs> GetPrijs(PlannerSearch search)
        {
            string cacheKey = "GetPrijs_" + search.GetUniqueId();

            return JsonCache.GetAsync(cacheKey, () => Original.GetPrijs(search), new DateTime(DateTime.Now.Year + 1, 1, 1));
        }

        public Task<List<ServiceRitInfo>> GetRit(string id, string company, DateTime date)
        {
            return Original.GetRit(id, company, date);
        }

        public Task<StoringenEnWerkzaamheden> GetStoringenEnWerkzaamheden(string station)
        {
            string cacheKey = "GetStoringenEnWerkzaamheden_" + station;

            return JsonCache.GetAsync(cacheKey, () => Original.GetStoringenEnWerkzaamheden(station), DateTime.Now.AddMinutes(5));
        }

        public Task<List<Vertrektijd>> GetVertrektijden(string station)
        {
            string cacheKey = "GetVertrektijden_" + station;

            return JsonCache.GetAsync(cacheKey, () => Original.GetVertrektijden(station), DateTime.Now.AddSeconds(30));
        }
    }
}
