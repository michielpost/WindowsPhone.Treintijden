using GalaSoft.MvvmLight.Ioc;
using Treintijden.PCL.Api.Models;
using Treintijden.Shared.Services.Interfaces;

namespace Trein.ViewModel
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
        public IStationNameService StationNameService { get; set; }

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
            StationNameService = SimpleIoc.Default.GetInstance<IStationNameService>();

            //Stations = StationService.GetStations("NL");

        }


       

        ////public override void Cleanup()
        ////{
        ////    // Clean own resources if needed

        ////    base.Cleanup();
        ////}

        internal void LoadStation(string stationCode)
        {
            CurrentStation = StationNameService.GetStationByCode(stationCode);

            //if(CurrentStation != null)
            //    Messenger.Default.Send<Station>(CurrentStation, "showmap");

        }
    }
}