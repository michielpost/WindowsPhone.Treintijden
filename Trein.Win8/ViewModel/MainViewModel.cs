using GalaSoft.MvvmLight;
using ActueelNS.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Data;
using GalaSoft.MvvmLight.Ioc;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Threading.Tasks;
using Treintijden.Shared.Services.Interfaces;
using Treintijden.PCL.Api.Models;
using Treintijden.Shared.Services.Models;

namespace Trein.Win8.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// </summary>
    public class MainViewModel : CustomViewModelBase
    {
        private readonly IStationNameService _stationService;
        private readonly IStationService _favStationService;
        private readonly IMyTrajectService _trajectService;
        private readonly IPlannerService _searchHistoryService;


        public PlannerViewModel Planner
        {
            get
            {
                return SimpleIoc.Default.GetInstance<PlannerViewModel>();
            }
        }

        public GpsWatcherModel Gps
        {
            get
            {
                return SimpleIoc.Default.GetInstance<GpsWatcherModel>();
            }
        }

        public StoringenViewModel Storingen
        {
            get
            {
                return SimpleIoc.Default.GetInstance<StoringenViewModel>();
            }
        }

     

        private ObservableCollection<Station> _favStations = new ObservableCollection<Station>();

        public ObservableCollection<Station> FavStations
        {
            get { return _favStations; }
            set
            {
                _favStations = value;
                RaisePropertyChanged(() => FavStations);
            }
        }



        private ObservableCollection<Traject> _trajecten = new ObservableCollection<Traject>();

        public ObservableCollection<Traject> Trajecten
        {
            get { return _trajecten; }
            set
            {
                _trajecten = value;
                RaisePropertyChanged(() => Trajecten);
            }
        }

        private ObservableCollection<PlannerSearch> _searchHistory = new ObservableCollection<PlannerSearch>();

        public ObservableCollection<PlannerSearch> SearchHistory
        {
            get { return _searchHistory; }
            set
            {
                _searchHistory = value;
                RaisePropertyChanged(() => SearchHistory);
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IStationNameService stationService, IStationService favStationService, IMyTrajectService ts, IPlannerService sh)
        {
            _stationService = stationService;
            _favStationService = favStationService;
            _trajectService = ts;
          _searchHistoryService = sh;

            if (ViewModelBase.IsInDesignModeStatic)
            {
                List<Station> stationDesignList = new List<Station>();
                stationDesignList.Add(new Station() { Name = "Amsterdam Centraal" });
                stationDesignList.Add(new Station() { Name = "Delft" });

                //FavStations = stationDesignList;

            }

            //Messenger.Default.Register<string>(this, "FavChanged", FavChangedReceived);

            LoadFavStations();

            //Task.Run(() =>
               // {
                    this.Gps.Initialize();
               // });


        }

        public void FavChangedReceived(string x)
        {
            LoadFavStations();
        }

        private async void LoadFavStations()
        {
            //await Task.Delay(2000);

            //Task.Run(async () =>
                //{
                    var favList = await _favStationService.GetMyStationsAsync();

                    SetFavStations(favList);

                    Trajecten = new ObservableCollection<Traject>(await _trajectService.GetAll());
                    SearchHistory = new ObservableCollection<PlannerSearch>(await _searchHistoryService.GetListFromStoreAsync());
               // });

        }

        private void SetFavStations(List<Station> favList)
        {
            var codeList = favList.Select(x => x.Code).ToList();
            var currentStations = FavStations.Select(x => x.Code).ToList();

            var removed = FavStations.Where(x => !codeList.Contains(x.Code)).ToList();

            var added = favList.Where(x => !currentStations.Contains(x.Code)).ToList();

            foreach (var del in removed)
            {
                FavStations.Remove(del);
            }

            foreach (var add in added)
            {
                //var vm = new DepartureTimesViewModel(SimpleIoc.Default.GetInstance<IStationService>()
                //    , SimpleIoc.Default.GetInstance<IVertrektijdenService>()
                //    , SimpleIoc.Default.GetInstance<IFavStationService>());
                //vm.Initialize(add.Code);
                FavStations.Add(add);
            }

        }

        internal async void AddFavStation(Station station)
        {
            await _favStationService.AddStationAsync(station);

          //TODO
          //  SetFavStations(favList);

        }

        internal void Initialize()
        {
            this.Planner.Initialize();

        }

        public void SoftCleanup()
        {
            base.Cleanup();

        }

        public override void Cleanup()
        {
            base.Cleanup();

            this.Gps.Cleanup();

            //if (this.FavStations != null)
            //{
            //    foreach (var fav in this.FavStations)
            //    {
            //        fav.SoftCleanup();
            //    }
            //}
        }



        internal void PickTraject(Traject t)
        {
            this.Planner.VanStation = t.From;
            this.Planner.NaarStation = t.To;
            this.Planner.ViaStation = t.Via;

            if(t.From != null)
                Messenger.Default.Send(t.From.Name, "setVan");
            if (t.To != null)
                Messenger.Default.Send(t.To.Name, "setNaar");

            if (t.Via != null)
                Messenger.Default.Send(t.Via.Name, "setVia");
            else
                Messenger.Default.Send(string.Empty, "setVia");


        }

        

        internal void DeleteTrajecten(List<Traject> list)
        {
            foreach (var t in list)
            {
                this.Trajecten.Remove(t);

            }
            _trajectService.SaveList(this.Trajecten.ToList());
        }

        internal async void DeleteStations(List<Station> list)
        {
            foreach (var t in list)
            {
                this.FavStations.Remove(t);
              await _favStationService.DeleteStationAsync(t.Name);

            }
            
          //_favStationService.(this.FavStations.ToList());
        }

        internal void DeleteSearchHistory(List<PlannerSearch> list)
        {
            foreach (var t in list)
            {
                this.SearchHistory.Remove(t);

            }
            _searchHistoryService.DeleteSearchHistoryAsync();
        }
    }
}