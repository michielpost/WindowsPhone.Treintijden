using ActueelNS.Services;
using ActueelNS.Services.Interfaces;
using GalaSoft.MvvmLight.Command;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Phone.Tasks;
using System.Linq;
using ActueelNS.Resources;
using Treintijden.Shared.Services.Interfaces;
using Treintijden.PCL.Api.Models;
using Treintijden.PCL.Api.Interfaces;

namespace ActueelNS.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// </summary>
    public class MainViewModel : CustomViewModelBase
    {
        public INavigationService NavigationService { get; set; }
        public IStationService StationService { get; set; }
        public IStationNameService StationNameService { get; set; }
       // public IStoringenService StoringenService { get; set; }
        public ILiveTileService LiveTileService { get; set; }
        public ISettingService SettingService { get; set; }

        private bool _isLoaded = false;
        private bool _isInit = false;

        public int TakeLimit { get; set; }


        public string PageName
        {
            get
            {
                return AppResources.stations;
            }
        }

        public IEnumerable<Station> GpsStationList
        {
            get
            {
                return ViewModelLocator.GpsWatcherStatic.Stations;
            }
        }

        private IEnumerable<Station> _stationList;

        public IEnumerable<Station> StationList
        {
            get {
              
              //TODO: Eenmalig ophalen en luisteren naar wijzigingen....
              //var settings = SettingService.GetSettingsAsync().Result;

              ////No Gps allowed?
              //if (!_isLoaded
              //    || !settings.ShowList
              //    || !settings.AllowGps)
              //  return _stationList;

                if (GpsStationList == null && _stationList == null)
                    return _stationList;
                else if (_stationList != null && GpsStationList == null)
                    return _stationList;
                else if (_stationList == null && GpsStationList != null)
                    return GpsStationList.Take(TakeLimit);
                else
                    return GpsStationList.Take(TakeLimit).Union(_stationList); 
            
            }
            set { _stationList = value;
            RaisePropertyChanged(() => StationList);
            RaisePropertyChanged(() => IsNewTextVisible);
            }
        }


        public bool IsNewTextVisible
        {
            get {

                if (_isInit && (StationList == null || StationList.Count() == 0))
                    return true;
                else
                    return false;
            }
           
        }

        private bool _storingenVisible;

        public bool StoringenVisible
        {
            get { return _storingenVisible; }
            set
            {
                _storingenVisible = value;
                RaisePropertyChanged(() => StoringenVisible);
            }
        }

        private string _storingTekst;

        public string StoringTekst
        {
            get { return _storingTekst; }
            set
            {
                _storingTekst = value;
                RaisePropertyChanged(() => StoringTekst);
            }
        }
        


        public RelayCommand AddStationCommand { get; private set; }
        public RelayCommand ReviewCommand { get; private set; }
        public RelayCommand DonateCommand { get; private set; }
        public RelayCommand StoringenCommand { get; private set; }
        public RelayCommand PlannerCommand { get; private set; }
        public RelayCommand AboutCommand { get; private set; }
        public RelayCommand SettingsCommand { get; private set; }
        public RelayCommand<string> TapCommand { get; private set; }
        public RelayCommand<string> DeleteCommand { get; private set; }
        public RelayCommand<string> PinCommand { get; private set; }
        public RelayCommand<string> PlanCommand { get; private set; }
        public RelayCommand<string> MapCommand { get; private set; }


        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {

            StationService = SimpleIoc.Default.GetInstance<IStationService>();
            StationNameService = SimpleIoc.Default.GetInstance<IStationNameService>();
            //StoringenService = SimpleIoc.Default.GetInstance<IStoringenService>();
            NavigationService = SimpleIoc.Default.GetInstance<INavigationService>();
            LiveTileService = SimpleIoc.Default.GetInstance<ILiveTileService>();
            SettingService = SimpleIoc.Default.GetInstance<ISettingService>();
           

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
                StationList = StationService.GetMyStationsAsync().Result;
            }
            else
            {
                ViewModelLocator.GpsWatcherStatic.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(GpsWatcherStatic_PropertyChanged);

                ViewModelLocator.StoringenStatic.CurrentStoringen.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(CurrentStoringen_CollectionChanged);
                InitStoringen();

            }


            AddStationCommand = new RelayCommand(() => NavigationService.NavigateTo(new Uri("/Views/StationPicker.xaml", UriKind.Relative)));
            //AddStationCommand = new RelayCommand(() => StationService.Test());

            //TapCommand = new RelayCommand<string>(x => LoadTijden(x));
            TapCommand = new RelayCommand<string>(x => NavigationService.NavigateTo(new Uri(string.Format("/Views/StationTijden.xaml?id={0}", x), UriKind.Relative)));

            DeleteCommand = new RelayCommand<string>(async x => await DeleteStationAsync(x));
            PinCommand = new RelayCommand<string>(x => PinStation(x), x => CanExecute(x));
            PlanCommand = new RelayCommand<string>(x => PlanStation(x));
            MapCommand = new RelayCommand<string>(x => MapAction(x));

            StoringenCommand = new RelayCommand(() => NavigationService.NavigateTo(new Uri("/Views/Storingen.xaml?id=", UriKind.Relative)));
            PlannerCommand = new RelayCommand(() => NavigationService.NavigateTo(new Uri("/Views/Planner.xaml", UriKind.Relative)));
            AboutCommand = new RelayCommand(() => NavigationService.NavigateTo(new Uri("/Views/About.xaml", UriKind.Relative)));
            SettingsCommand = new RelayCommand(() => NavigationService.NavigateTo(new Uri("/Views/Settings.xaml", UriKind.Relative)));
            DonateCommand = new RelayCommand(() => NavigationService.NavigateTo(new Uri("/Views/Donate.xaml", UriKind.Relative)));

            ReviewCommand = new RelayCommand(() =>
            {
                MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
                marketplaceReviewTask.Show();
            });


           
        }

        void CurrentStoringen_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            InitStoringen();
        }

        private void InitStoringen()
        {
            if (ViewModelLocator.StoringenStatic.CurrentStoringen == null || ViewModelLocator.StoringenStatic.CurrentStoringen.Count == 0)
            {
                StoringenVisible = false;
            }
            else
            {
                StoringenVisible = true;

                string storingenTekst = AppResources.StoringenFormat;
                if (ViewModelLocator.StoringenStatic.CurrentStoringen.Count == 1)
                    storingenTekst = AppResources.StoringFormat;

                StoringTekst = string.Format(storingenTekst, ViewModelLocator.StoringenStatic.CurrentStoringen.Count);
            }
        }

        void GpsWatcherStatic_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Stations")
            {
                RaisePropertyChanged(() => IsNewTextVisible);
                RaisePropertyChanged(() => StationList);

            }
           
           
        }

        private bool CanExecute(string name)
        {
            if (name != null)
            {
                return !LiveTileService.ExistsStation(name);
            }

            return false;
        }

        private async Task DeleteStationAsync(string x)
        {
            await StationService.DeleteStationAsync(x);
            await Update();
        }

        private void PinStation(string name)
        {
            var station = StationNameService.GetStationByName(name);
            LiveTileService.CreateStation(name, station.Code);
        }

        private void PlanStation(string name)
        {
            NavigationService.NavigateTo(new Uri(string.Format("/Views/Planner.xaml?from={0}", name), UriKind.Relative));
        }

        private void MapAction(string code)
        {
            NavigationService.NavigateTo(new Uri(string.Format("/Views/MapPage.xaml?id={0}", code), UriKind.Relative));
        }

        public async Task Update()
        {
            try
            {
              var settings = await SettingService.GetSettingsAsync();
              TakeLimit = settings.GpsListCount.HasValue ? settings.GpsListCount.Value : 2;
                 
                var list = await StationService.GetMyStationsAsync();

                _isInit = true;


                StationList = list;
            }
            catch { }
            finally
            {
                _isInit = true;
            }

          
        }


        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}

       

        internal void EnableGpsAsync()
        {

            if (!_isLoaded)
            {
                _isLoaded = true;

                //Already loaded
                if (ViewModelLocator.GpsWatcherStatic.Stations != null)
                {
                  RaisePropertyChanged(() => StationList);
                  RaisePropertyChanged(() => IsNewTextVisible);
                }
                else
                {



                  ViewModelLocator.GpsWatcherStatic.StartWatcherAsync();


                }
            }

        }
    }
}