using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Treintijden.PCL.Api.Models;

namespace Treintijden.PCL.Api.Interfaces
{
    public interface INSApiService
    {
        Task<List<ReisMogelijkheid>> GetSearchResult(PlannerSearch search);
        Task<ReisPrijs> GetPrijs(PlannerSearch search);
        Task<List<ServiceRitInfo>> GetRit(string id, string company, DateTime date);
        Task<TreinInfo> GetTreinInfo(string treinnummer);
        Task<StoringenEnWerkzaamheden> GetStoringenEnWerkzaamheden(string station);
        Task<List<Vertrektijd>> GetVertrektijden(string station);
    }
}
