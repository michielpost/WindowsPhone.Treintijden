using System.Collections.Generic;
using System.Threading.Tasks;
using Treintijden.PCL.Api.Models;

namespace ActueelNS.Services.Interfaces
{
    public interface ILastStationService
    {
        Task<List<Station>> Add(Station station1, Station station2, Station station3);

        Task<List<Station>> GetAll();
    }
}
