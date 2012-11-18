using ActueelNS.Services.Interfaces;
using ActueelNS.Services.Models;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActueelNS.ViewModel
{
    public class MapViewModel : CustomViewModelBase
    {
        private Station _currentStation;

        public Station CurrentStation
        {
            get { return _currentStation; }
            set
            {
                _currentStation = value;
                RaisePropertyChanged(() => CurrentStation);
            }
        }

        //private List<Station> _stations;

        //public List<Station> Stations
        //{
        //    get { return _stations; }
        //    set { _stations = value;
        //    RaisePropertyChanged(() => Stations);
        //    }
        //}
        

        public IStationService StationService { get; set; }

        /// <summary>
        /// Initializes a new instance of the AboutViewModel class.
        /// </summary>
        public MapViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real": Connect to service, etc...
            ////}

            StationService = SimpleIoc.Default.GetInstance<IStationService>();

            //Stations = StationService.GetStations("NL");

        }


       

        ////public override void Cleanup()
        ////{
        ////    // Clean own resources if needed

        ////    base.Cleanup();
        ////}

        internal void LoadStation(string stationCode)
        {
            CurrentStation = StationService.GetStationByCode(stationCode);

            //if(CurrentStation != null)
            //    Messenger.Default.Send<Station>(CurrentStation, "showmap");

        }
    }
}