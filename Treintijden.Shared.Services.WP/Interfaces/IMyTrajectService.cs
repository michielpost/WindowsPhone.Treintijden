using System.Collections.Generic;
using System.Threading.Tasks;
using Treintijden.Shared.Services.Models;

namespace ActueelNS.Services.Interfaces
{
    public interface IMyTrajectService
    {
        Task<List<Traject>> Add(Traject traject);
        Task<List<Traject>> Delete(Traject station);

        Task<List<Traject>> GetAll();

        Task SaveList(List<Traject> current);
    }
}
