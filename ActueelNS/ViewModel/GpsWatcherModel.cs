﻿using System;
using System.Windows;
using System.Device.Location;
using GalaSoft.MvvmLight.Ioc;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Treintijden.Shared.Services.Interfaces;
using Treintijden.PCL.Api.Models;

namespace ActueelNS.ViewModel
{
    public class GpsWatcherModel : CustomViewModelBase
    {
        public IStationNameService StationNameService { get; set; }
        public ISettingService SettingService { get; set; }


        private GeoCoordinateWatcher _instance;
        public GeoCoordinateWatcher Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GeoCoordinateWatcher(GeoPositionAccuracy.Default); // using high accuracy
                    _instance.MovementThreshold = 250; // use MovementThreshold to ignore noise in the signal

                    _instance.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcher_StatusChanged);
                    _instance.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);

                    //StartWatcher();
                }

                return _instance;
            }
        }

        public GeoCoordinate CurrentLocation { get; set; }


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

        public void StartWatcher()
        {
            try
            {
              var settings = SettingService.GetSettings();
              if (settings.AllowGps)
                {
                    Instance.Start();
                }
                else
                    Instance.Stop();
            }
            catch { }
        }

        public void StopWatcher()
        {
            try
            {
                Instance.Stop();
            }
            catch { }
        }


        void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            bool isBusy = false;

            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:
                    // The Location Service is disabled or unsupported.
                    // Check to see whether the user has disabled the Location Service.
                    if (Instance.Permission == GeoPositionPermission.Denied)
                    {
                        // The user has disabled the Location Service on their device.
                        //statusTextBlock.Text = "you have this application access to location.";
                    }
                    else
                    {
                        //statusTextBlock.Text = "location is not functioning on this device";
                    }

                    
                    break;

                case GeoPositionStatus.Initializing:
                    // The Location Service is initializing.
                    // Disable the Start Location button.
                    //startLocationButton.IsEnabled = false;

                    isBusy = true;

                    break;

                case GeoPositionStatus.NoData:
                    // The Location Service is working, but it cannot get location data.
                    // Alert the user and enable the Stop Location button.
                    //statusTextBlock.Text = "location data is not available.";
                    //stopLocationButton.IsEnabled = true;
                    break;

                case GeoPositionStatus.Ready:
                    // The Location Service is working and is receiving location data.
                    // Show the current position and enable the Stop Location button.
                    //statusTextBlock.Text = "location data is available.";
                    //stopLocationButton.IsEnabled = true;

                    break;
            }

            Deployment.Current.Dispatcher.BeginInvoke(() => IsBusy = isBusy);
        }

        async void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            //Console.WriteLine(e.Position.Location.Latitude.ToString("0.000"));
            //Console.WriteLine(e.Position.Location.Longitude.ToString("0.000"));

            try
            {
                var list = await Task.Run(() =>
                {
                    CurrentLocation = e.Position.Location;

                    var stationList = StationNameService.GetStations();

                    //int take = TakeLimit.HasValue ? TakeLimit.Value : 2;

                    
                    stationList.ForEach(x => CalcDistance(x));
                    return stationList.OrderBy(x => x.Distance).Take(10);
                });

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        Stations = list;
                    });
            }
            catch { }

        }

        private void CalcDistance(Station station)
        {
            GeoCoordinate stationGps = new GeoCoordinate(station.Lat, station.Long);

            station.SetDistance(CurrentLocation.GetDistanceTo(stationGps));
        }


      
    }
}
