using Q42.WinRT.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treintijden.PCL.Api.Interfaces;
using Treintijden.PCL.Api.Models;

namespace Treintijden.Shared.Services.WP
{
    public class CachedNSApiService : INSApiService
    {
        public INSApiService Original { get; set; }

        public CachedNSApiService(INSApiService original)
        {
            Original = original;
        }

        public Task<List<ReisMogelijkheid>> GetSearchResult(PlannerSearch search)
        {
            return Original.GetSearchResult(search);
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

        public Task<List<Storing>> GetStoringen(string station)
        {
            string cacheKey = "GetStoringen_" + station;

            return JsonCache.GetAsync(cacheKey, () => Original.GetStoringen(station), DateTime.Now.AddMinutes(5));
        }

        public Task<List<Vertrektijd>> GetVertrektijden(string station)
        {
            string cacheKey = "GetVertrektijden_" + station;

            return JsonCache.GetAsync(cacheKey, () => Original.GetVertrektijden(station), DateTime.Now.AddSeconds(30));
        }
    }
}
