using ActueelNS.Resources;
using ActueelNS.Services.Interfaces;
using GalaSoft.MvvmLight.Ioc;
using Q42.WinRT.Portable.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treintijden.PCL.Api.Interfaces;
using Treintijden.PCL.Api.Models;
using Treintijden.Shared.Services.Interfaces;

namespace ActueelNS.ViewModel
{
    public class RitInfoViewModel : CustomViewModelBase
    {
        public IStationService StationService { get; set; }
        public IStationNameService StationNameService { get; set; }
        public INSApiService RitnummerService { get; set; }
        public event EventHandler<EventArgs> RitInfoAvailable;




        private string _pageName;

        public string PageName
        {
            get { return _pageName.ToLower(); }
            set
            {
                _pageName = value;
                RaisePropertyChanged(() => PageName);
            }
        }

        private string _richting;

        public string Richting
        {
            get { return _richting; }
            set
            {
                _richting = value;
                RaisePropertyChanged(() => Richting);
            }
        }


        private List<RitInfoStop> _ritStops;

        public List<RitInfoStop> RitStops
        {
            get { return _ritStops; }
            set
            {
                _ritStops = value;
                RaisePropertyChanged(() => RitStops);
            }
        }

        public DataLoader DataLoader { get; set; }

        public RitInfoViewModel()
        {
            StationService = SimpleIoc.Default.GetInstance<IStationService>();
            StationNameService = SimpleIoc.Default.GetInstance<IStationNameService>();
            RitnummerService = SimpleIoc.Default.GetInstance<INSApiService>();

            DataLoader = new DataLoader();

            if (IsInDesignMode)
            {
                var list = new List<RitInfoStop>();
                list.Add(new RitInfoStop() { Station = "Delft", Arrival = DateTime.Now });
                list.Add(new RitInfoStop() { Station = "Delft", Arrival = DateTime.Now });
                list.Add(new RitInfoStop() { Station = "Delft", Arrival = DateTime.Now });
                list.Add(new RitInfoStop() { Station = "Delft", Arrival = DateTime.Now });

                RitStops = list;
            }


        }

        public async void Initialize(string ritId, string company, string trein, string richting, string stationCode)
        {
            PageName = trein;
            Richting = AppResources.RitInfoViewModelRichting + " " + richting;
            RitStops = new List<RitInfoStop>();

            RitStops = await DataLoader.LoadAsync(async () =>
                {

                    var serviceInfo = await RitnummerService.GetRit(ritId, company, DateTime.Now);

                    List<RitInfoStop> stops = null;

                    if (serviceInfo.FirstOrDefault() != null)
                    {
                      stops = serviceInfo.First().Stops;

                      //Fill station name for each stop
                      foreach (var stop in stops)
                      {
                        var station = StationNameService.GetStationByCode(stop.Code);
                        if (station != null)
                            stop.Station = station.Name;

                        if (stop.Code.ToLower() == stationCode.ToLower())
                            stop.IsCurrent = true;
                      }
                    }

                    return stops;
                });

            if (RitStops != null && RitInfoAvailable != null)
                RitInfoAvailable(null, null);

        }
    }
}
