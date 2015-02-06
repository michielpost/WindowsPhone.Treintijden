using System;
using System.Windows;
using GalaSoft.MvvmLight.Ioc;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Treintijden.Shared.Services.Interfaces;
using Treintijden.PCL.Api.Models;
using Windows.Devices.Geolocation;
using GalaSoft.MvvmLight.Threading;

namespace Trein.ViewModel
{
    public class GpsWatcherModel : CustomViewModelBase
    {
        public IStationNameService StationNameService { get; set; }
        public ISettingService SettingService { get; set; }


        private Geolocator _instance;
        public Geolocator Instance
        {
          get
          {
            if (_instance == null)
            {
              _instance = new Geolocator(); // 
              _instance.DesiredAccuracy = PositionAccuracy.Default;
              _instance.MovementThreshold = 250; // use MovementThreshold to ignore noise in the signal

              _instance.StatusChanged += _instance_StatusChanged;
              _instance.PositionChanged += _instance_PositionChanged;


            }

            return _instance;
          }
        }

        

        public Geocoordinate CurrentLocation { get; set; }


        private IEnumerable<Station> _stations;
        public IEnumerable<Station> Stations
        {
            get {
                return _stations;
            }
            set
            {
                _stations = value;
                RaisePropertyChanged(() => Stations);
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                RaisePropertyChanged(() => IsBusy);
            }
        }


        public GpsWatcherModel()
        {
            SettingService = SimpleIoc.Default.GetInstance<ISettingService>();
            StationNameService = SimpleIoc.Default.GetInstance<IStationNameService>();
            
        }

        //public void StartWatcher()
        //{
        //    try
        //    {
        //      var settings = SettingService.GetSettings();
        //      if (settings.AllowGps)
        //        {
        //            Instance.Start();
        //        }
        //        else
        //            Instance.Stop();
        //    }
        //    catch { }
        //}

        //public void StopWatcher()
        //{
        //    try
        //    {
        //        Instance.Stop();
        //    }
        //    catch { }
        //}

        
        

        void _instance_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            bool isBusy = false;

            switch (args.Status)
            {
              case PositionStatus.Disabled:
                //TODO
                    // The Location Service is disabled or unsupported.
                    // Check to see whether the user has disabled the Location Service.
                    //if (Instance.Permission == GeoPositionPermission.Denied)
                    //{
                    //    // The user has disabled the Location Service on their device.
                    //    //statusTextBlock.Text = "you have this application access to location.";
                    //}
                    //else
                    //{
                    //    //statusTextBlock.Text = "location is not functioning on this device";
                    //}

                    
                    break;

              case PositionStatus.Initializing:
                    // The Location Service is initializing.
                    // Disable the Start Location button.
                    //startLocationButton.IsEnabled = false;

                    isBusy = true;

                    break;

              case PositionStatus.NoData:
                    // The Location Service is working, but it cannot get location data.
                    // Alert the user and enable the Stop Location button.
                    //statusTextBlock.Text = "location data is not available.";
                    //stopLocationButton.IsEnabled = true;
                    break;

              case PositionStatus.Ready:
                    // The Location Service is working and is receiving location data.
                    // Show the current position and enable the Stop Location button.
                    //statusTextBlock.Text = "location data is available.";
                    //stopLocationButton.IsEnabled = true;

                    break;
            }

            DispatcherHelper.UIDispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => IsBusy = isBusy);
        }

        async void _instance_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            //Console.WriteLine(e.Position.Location.Latitude.ToString("0.000"));
            //Console.WriteLine(e.Position.Location.Longitude.ToString("0.000"));

            try
            {
                var list = await Task.Run(() =>
                {
                  CurrentLocation = args.Position.Coordinate;

                    var stationList = StationNameService.GetStations();

                    //int take = TakeLimit.HasValue ? TakeLimit.Value : 2;

                    foreach(var x in stationList)
                    {
                      CalcDistance(x);
                    }
                    return stationList.OrderBy(x => x.Distance).Take(10);
                });

                DispatcherHelper.UIDispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        Stations = list;
                    });
            }
            catch { }

        }

        public void StartWatcher()
        {
          try
          {
            var settings = SettingService.GetSettings();
            if (settings.AllowGps)
            {
              //TODO
              //Instance.Start();
            }
            else
            {
              //TODO
              //Instance.Stop();
            }
          }
          catch { }
        }

        public void StopWatcher()
        {
          try
          {
            //TODO
            //Instance.Stop();
          }
          catch { }
        }


        private void CalcDistance(Station station)
        {
          var dis = this.distance(CurrentLocation.Point.Position.Latitude, CurrentLocation.Point.Position.Longitude, station.Lat, station.Long, 'K');

            station.SetDistance(dis);
        }


        private double distance(double lat1, double lon1, double lat2, double lon2, char unit)
        {
          double theta = lon1 - lon2;
          double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
          dist = Math.Acos(dist);
          dist = rad2deg(dist);
          dist = dist * 60 * 1.1515;
          if (unit == 'K')
          {
            dist = dist * 1.609344;
          }
          else if (unit == 'N')
          {
            dist = dist * 0.8684;
          }
          return (dist);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts decimal degrees to radians             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private double deg2rad(double deg)
        {
          return (deg * Math.PI / 180.0);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts radians to decimal degrees             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private double rad2deg(double rad)
        {
          return (rad / Math.PI * 180.0);
        }


      
    }
}
