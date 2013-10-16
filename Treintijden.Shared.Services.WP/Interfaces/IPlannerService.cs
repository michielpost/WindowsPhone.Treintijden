using System;
using System.Collections.Generic;
using Treintijden.PCL.Api.Models;

namespace Treintijden.Shared.Services.Interfaces
{
    public interface IPlannerService
    {

        void AddSearch(PlannerSearch search);
        void AddPermSearch(PlannerSearch search, List<ReisMogelijkheid> mogelijkheden);

        List<PlannerSearch> GetListFromStore();
        PlannerSearch GetSearch(Guid id);
        List<ReisMogelijkheid> GetPermStoreSearchResult(Guid id);

        void PermStoreSearchResult(Guid id, List<ReisMogelijkheid> mogelijkheden);

        void DeleteSearchHistory();

        void DeleteSearch(Guid id);
    }
}
