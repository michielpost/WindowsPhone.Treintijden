using System.Collections.Generic;
using System.Threading.Tasks;
using Treintijden.PCL.Api.Models;

namespace Treintijden.Shared.Services.Interfaces
{
    public interface IStationService
    {

      Task<List<Station>> GetMyStationsAsync();

      Task AddStationAsync(Station name);

      Task DeleteStationAsync(string name);

      Task ClearAsync();

    
    }
}
