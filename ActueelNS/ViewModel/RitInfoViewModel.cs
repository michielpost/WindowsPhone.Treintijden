using ActueelNS.Services.Interfaces;
using ActueelNS.Services.Models;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActueelNS.ViewModel
{
    public class RitInfoViewModel : CustomViewModelBase
    {
        public IStationService StationService { get; set; }
        public IRitnummerService RitnummerService { get; set; }


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

        public RitInfoViewModel()
        {
            StationService = SimpleIoc.Default.GetInstance<IStationService>();
            RitnummerService = SimpleIoc.Default.GetInstance<IRitnummerService>();

        }

        public async void Initialize(string ritId, string company)
        {
            PageName = ritId;
            RitStops = new List<RitInfoStop>();

            var stops = await RitnummerService.GetRit(ritId, company, DateTime.Now);

            //Fill station name for each stop
            foreach (var stop in stops)
            {
                var station = StationService.GetStationByCode(stop.Code);
                if (station != null)
                    stop.Station = station.Name;
            }

            RitStops = stops;
          


        }
    }
}
