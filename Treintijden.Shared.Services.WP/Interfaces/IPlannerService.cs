using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Treintijden.PCL.Api.Models;

namespace Treintijden.Shared.Services.Interfaces
{
    public interface IPlannerService
    {

      Task AddSearchAsync(PlannerSearch search);
      Task AddPermSearchAsync(PlannerSearch search, List<ReisMogelijkheid> mogelijkheden);

      Task<List<PlannerSearch>> GetListFromStoreAsync();
      Task<PlannerSearch> GetSearchAsync(Guid id);
      Task<List<ReisMogelijkheid>> GetPermStoreSearchResultAsync(Guid id);

      Task PermStoreSearchResultAsync(Guid id, List<ReisMogelijkheid> mogelijkheden);

      Task DeleteSearchHistoryAsync();

      Task DeleteSearchAsync(Guid id);
    }
}
