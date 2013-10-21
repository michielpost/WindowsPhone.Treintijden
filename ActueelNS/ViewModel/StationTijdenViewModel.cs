using System;
using ActueelNS.Services.Interfaces;
using System.Collections.ObjectModel;
using ActueelNS.Services;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Command;
using System.Threading.Tasks;
using System.Linq;
using ActueelNS.Resources;
using Treintijden.Shared.Services.Interfaces;
using Treintijden.PCL.Api.Models;
using Treintijden.PCL.Api.Interfaces;
using Q42.WinRT.Portable.Data;

namespace ActueelNS.ViewModel
{

    public class StationTijdenViewModel : CustomViewModelBase
    {

        public INavigationService NavigationService { get; set; }
        public IStationService StationService { get; set; }
        public IStationNameService StationNameService { get; set; }
        public INSApiService NSApiService { get; set; }
        //public IStoringenService StoringenService { get; set; }
        //public IVertrektijdenService VertrektijdenService { get; set; }
        public ILiveTileService LiveTileService { get; set; }

        public DataLoader DataLoader { get; set; }

        private Station _currentStation;

        public Station CurrentStation
        {
            get { return _currentStation; }
            set { _currentStation = value;
            }
        }


        private string _pageName;

        public string PageName
        {
            get { return _pageName.ToLower(); }
            set { _pageName = value;
            RaisePropertyChanged(() => PageName);
            }
        }


        public RelayCommand DeleteCommand { get; private set; }
        public RelayCommand AddCommand { get; private set; }
        public RelayCommand PlanCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }
        public RelayCommand MapCommand { get; private set; }
        public RelayCommand PinCommand { get; private set; }
        public RelayCommand ToListCommand { get; private set; }
        public RelayCommand StoringenCommand { get; private set; }
        public RelayCommand<Vertrektijd> RitInfoCommand { get; private set; }


        private ObservableCollection<Vertrektijd> _tijdList;

        public ObservableCollection<Vertrektijd> TijdList
        {
            get { return _tijdList; }
            set { _tijdList = value;
            RaisePropertyChanged(() => TijdList);
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

        private bool _inMyStations;

        public bool InMyStations
        {
            get { return _inMyStations; }
            set { _inMyStations = value;
            RaisePropertyChanged(() => InMyStations);
            }
        }
        


        public StationTijdenViewModel()
        {
            //TijdList = new ObservableCollection<Vertrektijd>();

            DataLoader = new DataLoader();

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.

                var list = new List<Vertrektijd>();

                list.Add(new Vertrektijd()
                {
                    Tijd = DateTime.Now,
                    Eindbestemming = "Amsterdam",
                    IsVertrekspoorWijziging = false,
                    Ritnummer = 123,
                    Route = "Delft, Den Haag",
                    TreinSoort = "Sprinter",
                    Vertrekspoor = "2"
                });

                list.Add(new Vertrektijd()
                {
                    Tijd = DateTime.Now,
                    Eindbestemming = "Delft",
                    IsVertrekspoorWijziging = false,
                    Ritnummer = 123,
                    Route = "Delft, Den Haag",
                    TreinSoort = "Intercity",
                    Vertrekspoor = "2b"
                });


                list.Add(new Vertrektijd()
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

                list.ForEach(x => TijdList.Add(x));
                //TijdList = list;
                PageName = "Amsterdam";
            }
            else
            {
              
            }

            StationService = SimpleIoc.Default.GetInstance<IStationService>();
            StationNameService = SimpleIoc.Default.GetInstance<IStationNameService>();
            NSApiService = SimpleIoc.Default.GetInstance<INSApiService>();
            //VertrektijdenService = SimpleIoc.Default.GetInstance<IVertrektijdenService>();
            NavigationService = SimpleIoc.Default.GetInstance<INavigationService>();
            LiveTileService = SimpleIoc.Default.GetInstance<ILiveTileService>();
            //StoringenService = SimpleIoc.Default.GetInstance<IStoringenService>();

            RefreshCommand = new RelayCommand(() => LoadTijden());
            DeleteCommand = new RelayCommand(async () => await DeleteStation());
            AddCommand = new RelayCommand(async () => await AddStationAsync());
            MapCommand = new RelayCommand(() => MapAction());
            PinCommand = new RelayCommand(() => PinStation());
            PlanCommand = new RelayCommand(() => PlanStation());
            ToListCommand = new RelayCommand(() => ToList());
            StoringenCommand = new RelayCommand(() => ToStoringen());
            RitInfoCommand = new RelayCommand<Vertrektijd>(x => ToRitInfo(x));


            ViewModelLocator.StoringenStatic.CurrentStoringen.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(CurrentStoringen_CollectionChanged);
            InitStoringen();

        }

        private async void LoadTijden()
        {
            var result = await DataLoader.LoadAsync(() => NSApiService.GetVertrektijden(this.CurrentStation.Code));

            TijdList.Clear();
            if (result != null)
            {
              foreach (var s in result)
                TijdList.Add(s);
            }
        }

       

        void CurrentStoringen_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            InitStoringen();
        }

        private void InitStoringen()
        {
            if (ViewModelLocator.StoringenStatic.CurrentStoringen.Count == 0)
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

        private void ToStoringen()
        {
            string name = CurrentStation.Name;

            NavigationService.NavigateTo(new Uri(string.Format("/Views/Storingen.xaml?id={0}", name), UriKind.Relative));

        }

        private void ToRitInfo(Vertrektijd tijd)
        {
            string trein = string.Format("{0} ({1})", tijd.TreinSoort, tijd.Ritnummer);
            NavigationService.NavigateTo(new Uri(string.Format("/Views/RitInfoPage.xaml?id={0}&company={1}&trein={2}&richting={3}&stationCode={4}", tijd.Ritnummer, tijd.Vervoerder, trein, tijd.Eindbestemming, this.CurrentStation.Code), UriKind.Relative));

        }

        private void MapAction()
        {
            string id = CurrentStation.Code;

            NavigationService.NavigateTo(new Uri(string.Format("/Views/MapPage.xaml?id={0}", id), UriKind.Relative));

        }


        private void ToList()
        {
            //if (NavigationService.CanGoBack)
            //    NavigationService.GoBack();
            //else
                NavigationService.NavigateTo(new Uri("/MainPage.xaml", UriKind.Relative));
        }

               

        private async Task DeleteStation()
        {
            string name = CurrentStation.Name;

            await StationService.DeleteStationAsync(name);

            if(NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                NavigationService.NavigateTo(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private async Task AddStationAsync()
        {

            await StationService.AddStationAsync(CurrentStation);

            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                NavigationService.NavigateTo(new Uri("/MainPage.xaml", UriKind.Relative));
        }


        private void PinStation()
        {
            string name = CurrentStation.Name;
            LiveTileService.CreateStation(name, CurrentStation.Code);
        }

        private void PlanStation()
        {
            string name = CurrentStation.Name;

            NavigationService.NavigateTo(new Uri(string.Format("/Views/Planner.xaml?from={0}", name), UriKind.Relative));
        }

        internal async Task LoadStation(string station)
        {
          PageName = station;
          CurrentStation = StationNameService.GetStationByName(station);

          TijdList = null;


          var list = await DataLoader.LoadAsync(() => NSApiService.GetVertrektijden(this.CurrentStation.Code));

          if (list != null)
          {
            TijdList = new ObservableCollection<Vertrektijd>(list);
          }

          var mystations = await StationService.GetMyStationsAsync();
          InMyStations = mystations.Where(x => x.Code == CurrentStation.Code).Any();

        }


        public bool IsPinned
        {
            get
            {
                if (CurrentStation != null)
                    return LiveTileService.ExistsStation(CurrentStation.Name);
                else
                    return false;
            }
        }
    }
}
