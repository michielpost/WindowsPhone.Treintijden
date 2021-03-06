﻿using ActueelNS.Services.Interfaces;
using GalaSoft.MvvmLight.Ioc;
using System;
using GalaSoft.MvvmLight.Command;
using ActueelNS.Services;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using ActueelNS.Resources;
using System.Collections.ObjectModel;
using Treintijden.Shared.Services.Interfaces;
using Treintijden.PCL.Api.Models;
using Treintijden.Shared.Services.Models;

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
        public IStationNameService StationNameService { get; set; }
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
            set
            {
                _vanStation = value;
                RaisePropertyChanged(() => VanStation);
                //SearchCommand.RaiseCanExecuteChanged();
            }
        }

        private Station _naarStation;

        public Station NaarStation
        {
            get { return _naarStation; }
            set
            {
                _naarStation = value;
                RaisePropertyChanged(() => NaarStation);
                //SearchCommand.RaiseCanExecuteChanged();
            }
        }

        private Station _viaStation;

        public Station ViaStation
        {
            get { return _viaStation; }
            set
            {
                _viaStation = value;
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
          StationNameService = SimpleIoc.Default.GetInstance<IStationNameService>();
          NavigationService = SimpleIoc.Default.GetInstance<INavigationService>();
          PlannerService = SimpleIoc.Default.GetInstance<IPlannerService>();
          LiveTileService = SimpleIoc.Default.GetInstance<ILiveTileService>();
          SettingService = SimpleIoc.Default.GetInstance<ISettingService>();


          SearchCommand = new RelayCommand(async () => await DoSearch());
          MijnStationsCommand = new RelayCommand(() => DoMijnStations());
          SwitchCommand = new RelayCommand(() => DoSwitch());
          PinCommand = new RelayCommand(() => DoPin());
          SearchHistoryCommand = new RelayCommand(() => NavigationService.NavigateTo(new Uri("/Views/Reisadvies.xaml", UriKind.Relative)));

          ViewModelLocator.GpsWatcherStatic.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(GpsWatcherStatic_PropertyChanged);

          ViewModelLocator.GpsWatcherStatic.StartWatcher();
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

        private async Task DoSearch()
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

            try
            {
              //Save settings
              var settings = SettingService.GetSettings();
              settings.HasYearCard = IsYearCard;
              settings.UseHsl = IsHogesnelheid;
              SettingService.SaveSettings(settings);
            }
            catch
            {
              //never allow it to crash, this is really important
            }

            //Save planner object
            await PlannerService.AddSearchAsync(search);

            //Navigate to new page and pass GUID
            string url = string.Format("/Views/Reisadvies.xaml?id={0}", search.Id);
            NavigationService.NavigateTo(new Uri(url, UriKind.Relative));
          }
        }

        internal void InitValues(string from, string to, string via, bool keepValues, DateTime? dateTime)
        {
            Settings = SettingService.GetSettings();

            if (!keepValues && !string.IsNullOrEmpty(from))
            {
              VanStation = StationNameService.GetStationByName(from);
              if (VanStation == null)
                VanStation = StationNameService.GetStationByCode(from);
            }
            else if (!keepValues)
            {
              VanStation = null;
              GetGpsStation();

            }

            if (!keepValues && !string.IsNullOrEmpty(to))
                NaarStation = StationNameService.GetStationByName(to);
            else if (!keepValues)
                NaarStation = null;

            if (!keepValues && !string.IsNullOrEmpty(via))
                ViaStation = StationNameService.GetStationByName(via);
            else if (!keepValues)
                ViaStation = null;

            if (!keepValues)
            {
              if (dateTime.HasValue)
              {
                Date = dateTime.Value.Date;
                Time = dateTime.Value;
              }
              else
              {
                Date = DateTime.Now;
                Time = DateTime.Now;
              }

                IsYearCard = Settings.HasYearCard;
                IsHogesnelheid = Settings.UseHsl;
                IsViaVisible = false;
            }

            StationList.Clear();

        }

      /// <summary>
      /// Check if this search can be pinned
      /// </summary>
      /// <returns></returns>
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
                var all = StationNameService.GetStations();

                Stations = AlphaKeyGroup<Station>.CreateGroups(
                   all,
                   (Station s) => { return s.Name; },
                   true);
            }
        }

      /// <summary>
      /// Search a station
      /// </summary>
      /// <param name="p"></param>
        internal void SeachStation(string p)
        {
            if (string.IsNullOrEmpty(p))
            {
                StationList.Clear();

                ShowFavoriteStations();
            }
            else
            {
                p = p.ToLower();

                //Do a normal starts with
                var stations = StationNameService.GetStations().Where(x => x.Name.ToLower().StartsWith(p)).OrderBy(x => x.Sort).Take(8);

                if (stations.Count() < 8)
                {
                  //Search extra names codes etc
                  var extraStations = StationNameService.GetStations().Where(x => x.StartsWith(p)).OrderBy(x => x.Sort).Take(8 - stations.Count());

                  stations = stations.Union(extraStations);
                }

                //No results? Search international 
                if (stations.Count() <= 2)
                {
                  //Search extra names codes etc
                    var extraStations = StationNameService.GetStations(true).Where(x => x.StartsWith(p)).OrderBy(x => x.Sort).Take(8);

                //Remove stations already found from extraStations
                  var dubbel = extraStations.Where(x => stations.Select(s => s.Code).Contains(x.Code)).ToList();
                  extraStations = extraStations.Except(dubbel);

                  stations = stations.Union(extraStations);
                }

                StationList.Clear();

                foreach (var station in stations)
                    StationList.Add(station);
            }
        }

      /// <summary>
      /// Show favorite stations
      /// </summary>
        private void ShowFavoriteStations()
        {
            //Show favoriete en gps stations
            var list = ViewModelLocator.MainStatic.StationList;
            if (list != null)
            {
                var stationCodes = list.Select(x => x.Code.ToLower());
                var stations = StationNameService.GetStationsByCode(stationCodes).OrderBy(x => x.Name);
                foreach (var station in stations)
                    StationList.Add(station);
            }
        }

      /// <summary>
      /// Reset before a new pick
      /// </summary>
        internal void InitForNewPick()
        {
            StationList.Clear();

            ShowFavoriteStations();

        }
    }
}