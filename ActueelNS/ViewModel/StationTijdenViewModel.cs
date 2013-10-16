using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ActueelNS.Services.Interfaces;
using System.Collections.ObjectModel;
using ActueelNS.Services;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Command;
using System.Threading.Tasks;
using Microsoft.Phone.Shell;
using System.Linq;
using System.Threading;
using System.Windows.Media.Imaging;
using ActueelNS.Tile;
using AgFx;
using ActueelNS.Resources;
using Treintijden.Shared.Services.Interfaces;
using Treintijden.PCL.Api.Models;
using Treintijden.PCL.Api.Interfaces;

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

        private bool _sowError;

        public bool ShowError
        {
            get { return _sowError; }
            set
            {
                _sowError = value;
                RaisePropertyChanged(() => ShowError);
            }
        }


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
            DeleteCommand = new RelayCommand(() => DeleteStation());
            AddCommand = new RelayCommand(() => AddStation());
            MapCommand = new RelayCommand(() => MapAction());
            PinCommand = new RelayCommand(() => PinStation());
            PlanCommand = new RelayCommand(() => PlanStation());
            ToListCommand = new RelayCommand(() => ToList());
            StoringenCommand = new RelayCommand(() => ToStoringen());
            RitInfoCommand = new RelayCommand<Vertrektijd>(x => ToRitInfo(x));


            DataManager.Current.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Current_PropertyChanged);

            //TODO
            //ViewModelLocator.StoringenStatic.CurrentStoringen.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(CurrentStoringen_CollectionChanged);
            InitStoringen();

        }

        private void LoadTijden()
        {
            //TODO
            //DataManager.Current.Load<VertrektijdenDataModel>(new StationLoadContext(this.CurrentStation.Code)).Refresh();
        }

        void Current_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsLoading")
                IsBusy = DataManager.Current.IsLoading;
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

               

        private void DeleteStation()
        {
            string name = CurrentStation.Name;

            StationService.DeleteStation(name);

            if(NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                NavigationService.NavigateTo(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void AddStation()
        {

            StationService.AddStation(CurrentStation);

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

        internal async void LoadStation(string station)
        {
            PageName = station;
            CurrentStation = StationNameService.GetStationByName(station);

            TijdList = null;

            IsBusy = true;

            try
            {
                TijdList = new ObservableCollection<Vertrektijd>(await NSApiService.GetVertrektijden(this.CurrentStation.Code));
                IsBusy = false;
            }
            catch (Exception ex)
            {
                ShowError = true;
                IsBusy = false;
            }


            InMyStations = StationService.GetMyStations().Where(x => x.Code == CurrentStation.Code).Any();

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
