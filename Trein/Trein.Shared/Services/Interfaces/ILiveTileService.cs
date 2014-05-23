using System;
using System.Net;
using System.Windows;
using Treintijden.PCL.Api.Models;

namespace Trein.Services.Interfaces
{
    public interface ILiveTileService
    {
        void CreateStation(string name, string code);
        bool ExistsStation(string name);

        //bool Exists(string name);

        void CreateAdvies(PlannerSearch search, int index, DateTime date);
        bool ExistsCreateAdvies(PlannerSearch SelectedSearch, int p, DateTime dateTime);


        void CreatePlanner(string from, string to, string via, string fromCode, string toCode, string viaCode);
        bool ExistsCreatePlanner(string from, string to, string via);


        void CreateStoringen();
        bool ExistsCreateStoringen();

    }
}
