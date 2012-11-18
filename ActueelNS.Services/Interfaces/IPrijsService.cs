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
using ActueelNS.Services.Models;
using System.Threading.Tasks;

namespace ActueelNS.Services.Interfaces
{
    public interface IPrijsService
    {
        Task<ReisPrijs> GetPrijs(PlannerSearch search);
    }
}
