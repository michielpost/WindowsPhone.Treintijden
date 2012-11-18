using System;
using ActueelNS.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ActueelNS.Services.Interfaces
{
    public interface IPlannerService
    {

        void AddSearch(PlannerSearch search);
        void AddPermSearch(PlannerSearch search, List<ReisMogelijkheid> mogelijkheden);

        List<PlannerSearch> GetListFromStore();
        PlannerSearch GetSearch(Guid id);

        Task<List<ReisMogelijkheid>> GetSearchResult(PlannerSearch search);

        void DeleteSearchHistory();

        void DeleteSearch(Guid id);
    }
}
