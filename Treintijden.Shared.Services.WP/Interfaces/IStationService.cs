using System.Collections.Generic;
using Treintijden.PCL.Api.Models;

namespace Treintijden.Shared.Services.Interfaces
{
    public interface IStationService
    {

        IList<Station> GetMyStations();

        void AddStation(Station name);

        void DeleteStation(string name);

        void Clear();

    
    }
}
