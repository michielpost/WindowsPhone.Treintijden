using System;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Toolkit;
using Windows.Devices.Geolocation;
using ActueelNS.ViewModel;
using ActueelNS.Views.Base;
using Treintijden.PCL.Api.Models;

namespace ActueelNS.Views
{
    public partial class MapPage : ViewBase
    {
        public MapViewModel ViewModel
        {
            get
            {
                return (MapViewModel)this.DataContext;
            }
        }


        public MapPage()
        {
            InitializeComponent();

            this.MapExtensionsSetup(this.Map);

            //Messenger.Default.Register<Station>(this, "showmap", OnShowStation);

            //NavigationCacheMode = System.Windows.Navigation.NavigationCacheMode.Enabled;
        }

       

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);


            bool keepValues = (e.NavigationMode == System.Windows.Navigation.NavigationMode.Back);

            if (this.NavigationContext.QueryString.ContainsKey("id"))
            {
                string station = this.NavigationContext.QueryString["id"];

                ViewModel.LoadStation(station);

            }

            //Is geo available?
            try
            {
                Geolocator geolocator = new Geolocator();

                if (geolocator.LocationStatus == PositionStatus.Disabled
                    || geolocator.LocationStatus == PositionStatus.NotAvailable)
                {
                    this.ApplicationBar.IsVisible = false;
                }
                else
                    this.ApplicationBar.IsVisible = true;
            }
            catch { }


        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            this.StationPushPin.Visibility = Visibility.Collapsed;

            base.OnNavigatedFrom(e);

        }

        private void Map_Loaded_1(object sender, RoutedEventArgs e)
        {
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.ApplicationId = "a9ebc3af-a0c2-4780-9da3-8ee664981d13";
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.AuthenticationToken = "XDaMoiN6mJDC8NWal4LaWg";

            OnShowStation(ViewModel.CurrentStation);
        }

        private void OnShowStation(Station station)
        {
            this.StationPushPin.GeoCoordinate = new GeoCoordinate(station.Lat, station.Long);

            this.StationPushPin.Content = station.Name;
            this.StationPushPin.Visibility = Visibility.Visible;

            this.Map.SetView(this.StationPushPin.GeoCoordinate, 16);

        }

        /// <summary>
        /// Event handler for the Me button. It will show the user location marker and set the view on the map
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        private async void OnMe(object sender, EventArgs e)
        {
            await this.ShowUserLocation();

            if(this.UserLocationMarker.GeoCoordinate != null)
                this.Map.SetView(this.UserLocationMarker.GeoCoordinate, 16);
        }

        /// <summary>
        /// Show the user location in the map
        /// </summary>
        /// <returns>Task that can used to await</returns>
        private async Task ShowUserLocation()
        {
            Geolocator geolocator;
            Geoposition geoposition;

            this.UserLocationMarker = (UserLocationMarker)this.FindName("UserLocationMarker");

            try
            {
                geolocator = new Geolocator();

                if (geolocator.LocationStatus != PositionStatus.Disabled
                    && geolocator.LocationStatus != PositionStatus.NotAvailable)
                {
                    geoposition = await geolocator.GetGeopositionAsync();

                    if (geoposition != null)
                    {

                        this.UserLocationMarker.GeoCoordinate = geoposition.Coordinate.ToGeoCoordinate();
                        this.UserLocationMarker.Visibility = System.Windows.Visibility.Visible;
                    }
                }
            }
            catch(Exception e) 
            { 
            }
        }

        /// <summary>
        /// Setup the map extensions objects.
        /// All named objects inside the map extensions will have its references properly set
        /// </summary>
        /// <param name="map">The map that uses the map extensions</param>
        private void MapExtensionsSetup(Map map)
        {
            ObservableCollection<DependencyObject> children = MapExtensions.GetChildren(map);
            var runtimeFields = this.GetType().GetRuntimeFields();

            foreach (DependencyObject i in children)
            {
                var info = i.GetType().GetProperty("Name");

                if (info != null)
                {
                    string name = (string)info.GetValue(i);

                    if (name != null)
                    {
                        foreach (FieldInfo j in runtimeFields)
                        {
                            if (j.Name == name)
                            {
                                j.SetValue(this, i);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}