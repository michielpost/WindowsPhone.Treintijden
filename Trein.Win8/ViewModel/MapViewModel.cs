using ActueelNS.Services.Interfaces;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treintijden.PCL.Api.Models;
using Treintijden.Shared.Services.Interfaces;

namespace Trein.Win8.ViewModel
{

    public class GeoStation : CustomViewModelBase//, IGeoCluster
    {
        public Station Station { get; set; }

        private double _latitude;
        public double Latitude
        {
            get { return _latitude; }
            set { _latitude = value; RaisePropertyChanged(() => Latitude); }
        }

        private double _longitude;
        public double Longitude
        {
            get { return _longitude; }
            set { _longitude = value; RaisePropertyChanged(() => Longitude); }
        }

        private int _childrenCount;
        public int ChildrenCount
        {
            get { return _childrenCount; }
            set { _childrenCount = value; RaisePropertyChanged(() => ChildrenCount); }
        }

    }

    public class MapViewModel : CustomViewModelBase
    {

        public GpsWatcherModel Gps
        {
            get
            {
                return SimpleIoc.Default.GetInstance<GpsWatcherModel>();
            }
        }

        private List<GeoStation> _stations;


        public List<GeoStation> Stations
        {
            get { return _stations; }
            set { _stations = value;
            RaisePropertyChanged(() => Stations);
            }
        }
        

         /// <summary>
        /// Initializes a new instance of the viewmodel class.
        /// </summary>
        public MapViewModel()
        {
            LoadData();
           
        }

        private async void LoadData()
        {
            var list = SimpleIoc.Default.GetInstance<IStationNameService>().GetStations();

            Stations = list.Select(x => new GeoStation() { Latitude = x.Lat, Longitude = x.Long, Station = x }).ToList();
        }

    
    }
}
