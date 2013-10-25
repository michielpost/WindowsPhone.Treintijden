using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActueelNS.Services.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Treintijden.Shared.Services.Interfaces;
using Treintijden.PCL.Api.Interfaces;
using Treintijden.PCL.Api.Models;
using Q42.WinRT.Portable.Data;

namespace Trein.Win8.ViewModel
{
    public class DepartureTimesViewModel : CustomViewModelBase
    {
        private readonly IStationNameService _stationService;
        private readonly INSApiService _vertrektijdenService;
        private readonly IStationService _favStationService;

        public DataLoader DataLoader { get; set; }

        private DispatcherTimer _refreshTimer = new DispatcherTimer();


        private bool _isFav;

        public bool IsFav
        {
            get { return _isFav; }
            set { _isFav = value;
            RaisePropertyChanged(() => IsFav);
            }
        }

        private bool _isPinned;

        public bool IsPinned
        {
            get { return _isPinned; }
            set { _isPinned = value;
            RaisePropertyChanged(() => IsPinned);
            }
        }

        public string TileId
        {
            get
            {
                if (CurrentStation == null)
                    return null;
                else
                    return "DepartureTimesPage=" + this.CurrentStation.Code;
                     
            }
           
        }
        



        private Station _currentStation;

        public Station CurrentStation
        {
            get { return _currentStation; }
            set
            {
                _currentStation = value;
                RaisePropertyChanged(() => CurrentStation);
                RaisePropertyChanged(() => IsPinned);

            }
        }

        private ObservableCollection<Vertrektijd> _tijdList = new ObservableCollection<Vertrektijd>();

        public ObservableCollection<Vertrektijd> TijdList
        {
            get { return _tijdList; }
            set
            {
                _tijdList = value;
                RaisePropertyChanged(() => TijdList);
            }
        }

        public RelayCommand FavCommand { get; private set; }
        public RelayCommand DeleteFavCommand { get; private set; }
        public RelayCommand UpdateCommand { get; private set; }



         /// <summary>
        /// Initializes a new instance of the viewmodel class.
        /// </summary>
        public DepartureTimesViewModel(IStationNameService stationService, INSApiService vertrektijdenService, IStationService favStationService)
        {
            _stationService = stationService;
            _vertrektijdenService = vertrektijdenService;
            _favStationService = favStationService;

            DataLoader = new DataLoader();

            if (ViewModelBase.IsInDesignModeStatic)
            {
                // Code runs in Blend --> create design time data.
                CurrentStation = new Station() { Name = "Leeuwarden" };

                _tijdList.Add(new Vertrektijd()
                {
                    Tijd = DateTime.Now,
                    Eindbestemming = "Amsterdam",
                    IsVertrekspoorWijziging = false,
                    Ritnummer = 123,
                    Route = "Delft, Den Haag",
                    TreinSoort = "Sprinter",
                    Vertrekspoor = "2"
                });

                _tijdList.Add(new Vertrektijd()
                {
                    Tijd = DateTime.Now,
                    Eindbestemming = "Delft",
                    IsVertrekspoorWijziging = false,
                    Ritnummer = 123,
                    Route = "Delft, Den Haag",
                    TreinSoort = "Intercity",
                    Vertrekspoor = "2b"
                });


                _tijdList.Add(new Vertrektijd()
                {
                    Tijd = DateTime.Now,
                    Eindbestemming = "Groningen",
                    IsVertrekspoorWijziging = true,
                    Ritnummer = 123,
                    Route = "Amsterdam, Den Haag",
                    TreinSoort = "Intercity",
                    Vertrekspoor = "2b",
                    VertragingTekst = "+5 min"

                });

            }

            FavCommand = new RelayCommand(DoFavCommand);
            DeleteFavCommand = new RelayCommand(DoDeleteFavCommand);
            UpdateCommand = new RelayCommand(DoUpdate);

            _refreshTimer.Interval = new TimeSpan(0, 2, 0);
            _refreshTimer.Tick += _refreshTimer_Tick;
            _refreshTimer.Start();
        }

        public override void Cleanup()
        {
            base.Cleanup();

            if (_refreshTimer.IsEnabled)
                _refreshTimer.Stop();

        }

        internal void SoftCleanup()
        {
            if (_refreshTimer.IsEnabled)
                _refreshTimer.Stop();
        }

        internal void SoftInitialize()
        {
            if(!_refreshTimer.IsEnabled)
                _refreshTimer.Start();
        }

        void _refreshTimer_Tick(object sender, object e)
        {
            DoUpdate();
        }


        private async void DoUpdate()
        {
            await LoadDepartureTimes(false);
        }

        private async void DoFavCommand()
        {
            if (this.CurrentStation != null)
            {
                IsFav = true;

                await _favStationService.AddStationAsync(this.CurrentStation);

                //Messenger.Default.Send<string>("FavChanged", "FavChanged");
                SimpleIoc.Default.GetInstance<MainViewModel>().FavChangedReceived(string.Empty);

            }
        }

        private async void DoDeleteFavCommand()
        {
            if (this.CurrentStation != null)
            {
                IsFav = false;

                await _favStationService.DeleteStationAsync(this.CurrentStation.Name);

                //Messenger.Default.Send<string>("FavChanged", "FavChanged");
                SimpleIoc.Default.GetInstance<MainViewModel>().FavChangedReceived(string.Empty);

                //if(App.RootFrame.CanGoBack)
                //    App.RootFrame.GoBack();
            }
        }


        public async void Initialize(string stationCode, bool full = false)
        {
            _refreshTimer.Start();

            this.CurrentStation = _stationService.GetStationByCode(stationCode);

            if (full)
            {
                IsPinned = Windows.UI.StartScreen.SecondaryTile.Exists(this.TileId);

                var allFav = await _favStationService.GetMyStationsAsync();
                IsFav = allFav.Select(x => x.Code.ToLower()).Contains(stationCode.ToLower());
            }

            TijdList.Clear();

            await LoadDepartureTimes(true);
        }

        private async Task LoadDepartureTimes(bool first)
        {
          var vertrektijden = await DataLoader.LoadAsync(() => _vertrektijdenService.GetVertrektijden(this.CurrentStation.Code));

          TijdList.Clear();

          bool alternative = false;
          foreach (var item in vertrektijden)
          {
            item.IsAlternate = alternative;

            this.TijdList.Add(item);

            alternative = !alternative;
          }

        }

        internal SecondaryTile CreateTileForStation()
        {
            Uri logo = new Uri("ms-appx:///Assets/Tile.scale-100.png");

            // During creation of secondary tile, an application may set additional arguments on the tile that will be passed in during activation. 
            // These arguments should be meaningful to the application. In this sample, we'll pass in the date and time the secondary tile was pinned.

            // Create a 1x1 Secondary tile
            SecondaryTile secondaryTile = new SecondaryTile(this.TileId,
                                                            this.CurrentStation.Name,
                                                            "Treintijden " + this.CurrentStation.Name,
                                                            "DepartureTimesPage=" + this.CurrentStation.Code,
                                                            TileOptions.ShowNameOnLogo,
                                                            logo);

           

            // Specify a foreground text value. 
            // The tile background color is inherited from the parent unless a separate value is specified.
            secondaryTile.ForegroundText = ForegroundText.Light;

            // Like the background color, the small tile logo is inherited from the parent application tile by default. Let's override it, just to see how that's done.
            //secondaryTile.SmallLogo = smallLogo;

            return secondaryTile;
                     
        }




        internal void SetPinned(bool isPinned)
        {
            this.IsPinned = isPinned;
        }





        internal void SetStationForPlanner()
        {
            PlannerViewModel planner = SimpleIoc.Default.GetInstance<PlannerViewModel>();
            planner.VanStation = this.CurrentStation;
            Messenger.Default.Send(planner.VanStation.Name, "setVan");
        }
    }
}
