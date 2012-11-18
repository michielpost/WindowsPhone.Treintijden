using GalaSoft.MvvmLight;
using ActueelNS.Services.Interfaces;
using GalaSoft.MvvmLight.Ioc;
using ActueelNS.Services.Models;
using System;
using GalaSoft.MvvmLight.Command;
using ActueelNS.Services;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using ActueelNS.Resources;
using System.Collections.ObjectModel;

namespace ActueelNS.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
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
    public class PlannerViewModel : CustomViewModelBase
    {
        public IStationService StationService { get; set; }
        public INavigationService NavigationService { get; set; }
        public IPlannerService PlannerService { get; set; }
        public ILiveTileService LiveTileService { get; set; }
        public ISettingService SettingService { get; set; }

        private List<AlphaKeyGroup<Station>> _stations;
        public List<AlphaKeyGroup<Station>> Stations
        {
            get
            {
                return _stations;
            }
            set
            {
                _stations = value;
                RaisePropertyChanged(() => Stations);

            }
        }

       private ObservableCollection<Station> _stationList = new ObservableCollection<Station>();
        public ObservableCollection<Station> StationList
        {
            get
            {
                return _stationList;
            }
            set
            {
                _stationList = value;
                //RaisePropertyChanged(() => StationList);
            }
        }


        private Station _vanStation;

        public Station VanStation
        {
            get { return _vanStation; }
            set { _vanStation = value;
            RaisePropertyChanged(() => VanStation);
            //SearchCommand.RaiseCanExecuteChanged();
            }
        }

        private Station _naarStation;

        public Station NaarStation
        {
            get { return _naarStation; }
            set { _naarStation = value;
            RaisePropertyChanged(() => NaarStation);
            //SearchCommand.RaiseCanExecuteChanged();
            }
        }

        private Station _viaStation;

        public Station ViaStation
        {
            get { return _viaStation; }
            set { _viaStation = value;
            RaisePropertyChanged(() => ViaStation);
            RaisePropertyChanged(() => IsViaVisible);
            }
        }

        private AppSetting _settings;

        public AppSetting Settings
        {
            get { return _settings; }
            set
            {
                _settings = value;
            }
        }

        private bool _isViaVisible;

        public bool IsViaVisible
        {
            get
            {

                if (ViaStation != null)
                    return true;
                else
                    return _isViaVisible;
            }
            set
            {
                _isViaVisible = value;
                RaisePropertyChanged(() => IsViaVisible);
            }
        }
        


        public DateTime Date { get; set; }
        public DateTime Time { get; set; }

        public string Type { get; set; }
        public bool IsHogesnelheid { get; set; }
        public bool IsYearCard { get; set; }

        public RelayCommand PinCommand { get; private set; }
        public RelayCommand SwitchCommand { get; private set; }
        public RelayCommand SearchCommand { get; private set; }
        public RelayCommand MijnStationsCommand { get; private set; }
        public RelayCommand SearchHistoryCommand { get; private set; }
        


        public string PageName
        {
            get
            {
                return AppResources.reisplanner;
            }
        }

        /// <summary>
        /// Initializes a new instance of the PlannerViewModel class.
        /// </summary>
        public PlannerViewModel()
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
            NavigationService = SimpleIoc.Default.GetInstance<INavigationService>();
            PlannerService = SimpleIoc.Default.GetInstance<IPlannerService>();
            LiveTileService = SimpleIoc.Default.GetInstance<ILiveTileService>();
            SettingService = SimpleIoc.Default.GetInstance<ISettingService>();


            SearchCommand = new RelayCommand(() => DoSearch());
            MijnStationsCommand = new RelayCommand(() => DoMijnStations());
            SwitchCommand = new RelayCommand(() => DoSwitch());
            PinCommand = new RelayCommand(() => DoPin());
            SearchHistoryCommand = new RelayCommand(() => NavigationService.NavigateTo(new Uri("/Views/Reisadvies.xaml", UriKind.Relative)));

            ViewModelLocator.GpsWatcherStatic.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(GpsWatcherStatic_PropertyChanged);

            Task.Run(() =>
            {
                ViewModelLocator.GpsWatcherStatic.StartWatcher();
            });
        }

        private bool CanDoSearch()
        {
            if (VanStation != null && NaarStation != null)
                return true;

            return false;
        }

        private void DoMijnStations()
        {
            NavigationService.NavigateTo(new Uri("/MainPage.xaml", UriKind.Relative));

        }

        void GpsWatcherStatic_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Stations")
            {
                GetGpsStation();
            }
        }

        private void GetGpsStation()
        {
            if (VanStation == null
                && Settings.AllowGps
                && Settings.AutoFill
                && ViewModelLocator.GpsWatcherStatic.Stations != null)
                VanStation = ViewModelLocator.GpsWatcherStatic.Stations.FirstOrDefault();
        }

        private void DoPin()
        {
            string from = VanStation != null ? VanStation.Name : string.Empty;
            string to = NaarStation != null ? NaarStation.Name : string.Empty;
            string via = ViaStation != null ? ViaStation.Name : string.Empty;

            string fromCode = VanStation != null ? VanStation.Code : string.Empty;
            string toCode = NaarStation != null ? NaarStation.Code : string.Empty;
            string viaCode = ViaStation != null ? ViaStation.Code : string.Empty;

            LiveTileService.CreatePlanner(from, to, via, fromCode, toCode, viaCode);
        }

        private void DoSwitch()
        {
            if (NaarStation != null || VanStation != null)
            {
                var temp = NaarStation != null ? NaarStation.ShallowCopy() : null;

                NaarStation = VanStation;
                VanStation = temp;
            }
        }

        private void DoSearch()
        {
            if (Date == DateTime.MinValue)
                Date = DateTime.Now;
            if (Time == DateTime.MinValue)
                Time = DateTime.Now;
            if (string.IsNullOrEmpty(Type))
                Type = "vertrek";

            //Create planner object with GUID
            PlannerSearch search = new PlannerSearch()
            {
                Id = Guid.NewGuid(),
                SearchDateTime = DateTime.Now,
                VanStation = VanStation,
                NaarStation = NaarStation,
                ViaStation = ViaStation,
                IsHogesnelheid = IsHogesnelheid,
                IsYearCard = IsYearCard,
                Type = Type,
                Date = Date,
                Time = Time
            };

            if (search.VanStation != null
                && search.NaarStation != null)
            {
                //Save planner object
                PlannerService.AddSearch(search);

                var settings = SettingService.GetSettings();
                settings.HasYearCard = IsYearCard;
                settings.UseHsl = IsHogesnelheid;
                SettingService.SaveSettings(settings);

                //Navigate to new page and pass GUID
                string url = string.Format("/Views/Reisadvies.xaml?id={0}", search.Id);
                NavigationService.NavigateTo(new Uri(url, UriKind.Relative));
            }
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean own resources if needed

        ////    base.Cleanup();
        ////}

        internal void InitValues(string from, string to, string via, bool keepValues)
        {
            Settings = SettingService.GetSettings();

            if (!keepValues && !string.IsNullOrEmpty(from))
                VanStation = StationService.GetStationByName(from);
            else if (!keepValues)
            {
                VanStation = null;
                GetGpsStation();

            }

            if (!keepValues && !string.IsNullOrEmpty(to))
                NaarStation = StationService.GetStationByName(to);
            else if (!keepValues)
                NaarStation = null;

            if (!keepValues && !string.IsNullOrEmpty(via))
                ViaStation = StationService.GetStationByName(via);
            else if (!keepValues)
                ViaStation = null;

            if (!keepValues)
            {
                Date = DateTime.Now;
                Time = DateTime.Now;

                IsYearCard = Settings.HasYearCard;
                IsHogesnelheid = Settings.UseHsl;
                IsViaVisible = false;
            }

                       StationList.Clear();

          

        }

        internal bool CanPin()
        {
            string from = VanStation != null ? VanStation.Name : string.Empty;
            string to = NaarStation != null ? NaarStation.Name : string.Empty;
            string via = ViaStation != null ? ViaStation.Name : string.Empty;

           return !LiveTileService.ExistsCreatePlanner(from, to, via);
        }

        internal void LoadForPicker()
        {
            if (Stations == null)
            {
                var all = StationService.GetStations("NL");

                Stations = AlphaKeyGroup<Station>.CreateGroups(
                   all,
                   (Station s) => { return s.Name; },
                   true);
            }
        }

        internal void SeachStation(string p)
        {
            if (string.IsNullOrEmpty(p))
                StationList.Clear();
            else
            {
                p = p.ToLower();

                var stations = StationService.GetStations("NL").Where(x => x.Name.ToLower().StartsWith(p)).Take(8);

                if (stations.Count() < 8)
                {
                    var extraStations = StationService.GetStations("NL").Where(x => x.StartsWith(p)).Take(8 - stations.Count());

                    stations = stations.Union(extraStations);
                }

                StationList.Clear();

                foreach (var station in stations)
                    StationList.Add(station);
            }
        }
    }
}