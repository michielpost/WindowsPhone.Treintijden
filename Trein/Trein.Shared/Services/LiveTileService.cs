using System;
using System.Collections.Generic;
using System.Text;
using Trein.Services.Interfaces;

namespace Trein.Services
{
  public class LiveTileService : ILiveTileService
  {
    public void CreateStation(string name, string code)
    {
      throw new NotImplementedException();
    }

    public bool ExistsStation(string name)
    {
      throw new NotImplementedException();
    }

    public void CreateAdvies(Treintijden.PCL.Api.Models.PlannerSearch search, int index, DateTime date)
    {
      throw new NotImplementedException();
    }

    public bool ExistsCreateAdvies(Treintijden.PCL.Api.Models.PlannerSearch SelectedSearch, int p, DateTime dateTime)
    {
      throw new NotImplementedException();
    }

    public void CreatePlanner(string from, string to, string via, string fromCode, string toCode, string viaCode)
    {
      throw new NotImplementedException();
    }

    public bool ExistsCreatePlanner(string from, string to, string via)
    {
      throw new NotImplementedException();
    }

    public void CreateStoringen()
    {
      throw new NotImplementedException();
    }

    public bool ExistsCreateStoringen()
    {
      throw new NotImplementedException();
    }
  }
}
