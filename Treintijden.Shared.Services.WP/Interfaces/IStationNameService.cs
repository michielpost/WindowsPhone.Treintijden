using System.Collections.Generic;
using Treintijden.PCL.Api.Models;

namespace Treintijden.Shared.Services.Interfaces
{
    public interface IStationNameService
    {
        List<Station> GetStations(bool searchInAll = false);


        Station GetStationByName(string name);
        Station GetStationByCode(string code);


        IList<Station> GetStationsByCode(IEnumerable<string> stationCodes);
    }
}
