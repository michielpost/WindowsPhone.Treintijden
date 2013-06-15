using System.Collections.Generic;
using ActueelNS.Services.Models;

namespace ActueelNS.Services.Interfaces
{
    public interface IStationService
    {
        List<Station> GetStations(bool searchInAll = false);

        IList<Station> GetMyStations();

        void AddStation(Station name);

        void DeleteStation(string name);

        Station GetStationByName(string name);
        Station GetStationByCode(string code);

    }
}
