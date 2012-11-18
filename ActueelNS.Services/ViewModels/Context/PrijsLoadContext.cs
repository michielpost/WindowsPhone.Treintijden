using ActueelNS.Services.Models;
using AgFx;

namespace ActueelNS.Services.ViewModels.Context
{
    public class PrijsLoadContext : LoadContext
    {
        public PlannerSearch PlannerSearch { get; set; }


        public PrijsLoadContext(PlannerSearch search)
            : base(search.GetUniqueId())
        {

            PlannerSearch = search;

        }
    }
}
