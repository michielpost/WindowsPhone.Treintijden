using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using Treintijden.PCL.Api.Models;

namespace ActueelNS.Services.Interfaces
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
