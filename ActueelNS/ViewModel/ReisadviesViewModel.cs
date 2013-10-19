using GalaSoft.MvvmLight;
using ActueelNS.Services;
using GalaSoft.MvvmLight.Ioc;
using ActueelNS.Services.Interfaces;
using System.Collections.ObjectModel;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Command;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using ActueelNS.Resources;
using Microsoft.Phone.Tasks;
using Treintijden.Shared.Services.Interfaces;
using Treintijden.PCL.Api.Models;
using Treintijden.PCL.Api.Interfaces;
using Q42.WinRT.Portable.Data;

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
    public class ReisadviesViewModel : CustomViewModelBase
    {
        public IPlannerService PlannerService { get; set; }
        public INSApiService NSApiService { get; set; }
        public INavigationService NavigationService { get; set; }
        public ILiveTileService LiveTileService { get; set; }

        public bool IsInit { get; set; }

        private int? _tempIndex;

        private ObservableCollection<PlannerSearch> _searchHistory;

        public ObservableCollection<PlannerSearch> SearchHistory
        {
            get { return _searchHistory; }
            set { _searchHistory = value;
            RaisePropertyChanged(() => SearchHistory);
            RaisePropertyChanged(() => SearchHistorySmall);

            }
        }


        public List<PlannerSearch> SearchHistorySmall
        {
            get { return _searchHistory.Take(5).ToList(); }
        }


        private PlannerSearch _selectedSearch;

        public PlannerSearch SelectedSearch
        {
            get { return _selectedSearch; }
            set { _selectedSearch = value;
            RaisePropertyChanged(() => SelectedSearch);
            }
        }

        private List<ReisMogelijkheid> _reisMogelijkheden;

        public List<ReisMogelijkheid> ReisMogelijkheden
        {
            get { return _reisMogelijkheden; }
            set { _reisMogelijkheden = value;
            RaisePropertyChanged(() => ReisMogelijkheden);
            }
        }

        private ReisMogelijkheid _selectedReisMogelijkheid;

        public ReisMogelijkheid SelectedReisMogelijkheid
        {
            get { return _selectedReisMogelijkheid; }
            set { _selectedReisMogelijkheid = value;
            RaisePropertyChanged(() => SelectedReisMogelijkheid);
            }
        }

        private ReisMogelijkheid _mainReisMogelijkheid;

        public ReisMogelijkheid MainReisMogelijkheid
        {
            get { return _mainReisMogelijkheid; }
            set
            {
                _mainReisMogelijkheid = value;
                RaisePropertyChanged(() => MainReisMogelijkheid);
            }
        }



        public DataLoader DataLoader { get; set; }


        public string PageName
        {
            get
            {
                return AppResources.reisadvies;
            }
        }

        public RelayCommand EerderCommand { get; private set; }
        public RelayCommand LaterCommand { get; private set; }
        public RelayCommand TerugreisCommand { get; private set; }
        public RelayCommand RepeatSearchCommand { get; private set; }
        public RelayCommand DoSearchCommand { get; private set; }
        public RelayCommand DeleteHistoryCommand { get; private set; }
        public RelayCommand VertrektijdenCommand { get; private set; }
        public RelayCommand PinCommand { get; private set; }
        public RelayCommand AddReminderCommand { get; private set; }
        public RelayCommand PrijsCommand { get; private set; }
        public RelayCommand MijnStationsCommand { get; private set; }
        public RelayCommand<Guid> DeleteCommand { get; private set; }
        public RelayCommand<Guid> AdviceTapCommand { get; private set; }
        public RelayCommand<PlannerSearch> ReplanCommand { get; private set; }



        /// <summary>
        /// Initializes a new instance of the ReisadviesViewModel class.
        /// </summary>
        public ReisadviesViewModel()
        {
            DataLoader = new DataLoader();

            SearchHistory = new ObservableCollection<PlannerSearch>();
            PlannerService = SimpleIoc.Default.GetInstance<IPlannerService>();
            NSApiService = SimpleIoc.Default.GetInstance<INSApiService>();
            NavigationService = SimpleIoc.Default.GetInstance<INavigationService>();
            LiveTileService = SimpleIoc.Default.GetInstance<ILiveTileService>();

            this.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(ReisadviesViewModel_PropertyChanged);

            EerderCommand = new RelayCommand(() => DoEerder(), () => CanDoEerder());
            LaterCommand = new RelayCommand(() => DoLater(), () => CanDoLater());
            TerugreisCommand = new RelayCommand(() => PlanTerugreis());
            RepeatSearchCommand = new RelayCommand(() => RepeatSearch());
            DoSearchCommand = new RelayCommand(() => DoSearch());
            MijnStationsCommand = new RelayCommand(() => DoMijnStations());
            PinCommand = new RelayCommand(() => PinSearch());
            AddReminderCommand = new RelayCommand(() => AddReminder());
            PrijsCommand = new RelayCommand(() => GoPrijs());
            VertrektijdenCommand = new RelayCommand(() => GoVertrektijden());
            DeleteHistoryCommand = new RelayCommand(() => DeleteHistory());
            DeleteCommand = new RelayCommand<Guid>(x => DeleteSingleHistory(x));
            AdviceTapCommand = new RelayCommand<Guid>(x => AdviceTapCommandAction(x));
            ReplanCommand = new RelayCommand<PlannerSearch>(x => RepeatSearch(x));

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
                SearchHistory.Add(new PlannerSearch()
                {
                    VanStation = new Station() { Name = "Amsterdam" },
                    NaarStation = new Station() { Name = "Delft" },
                    ViaStation = new Station() { Name = "Almere" }
                     
                });

                SearchHistory.Add(new PlannerSearch()
                {
                    VanStation = new Station() { Name = "Maastricht" },
                    NaarStation = new Station() { Name = "Groningen" }

                });


                SelectedSearch = new PlannerSearch()
                {
                    VanStation = new Station() { Name = "Amsterdam" },
                    NaarStation = new Station() { Name = "Delft" }
                     
                };


                ReisMogelijkheden = new List<ReisMogelijkheid>()
                {
                    new ReisMogelijkheid() {
                         Optimaal = true,
                          AantalOverstappen = 2,
                           GeplandeAankomstTijd = DateTime.Now,
                            GeplandeVertrekTijd = DateTime.Now,
                             GeplandeReisTijd = "2:05"
                    },
                     new ReisMogelijkheid() {
                         Optimaal = true,
                          AantalOverstappen = 2,
                           GeplandeAankomstTijd = DateTime.Now,
                            GeplandeVertrekTijd = DateTime.Now,
                             GeplandeReisTijd = "2:05"
                    }
                };

                SelectedReisMogelijkheid = new ReisMogelijkheid()
                {
                    Optimaal = true,
                    AantalOverstappen = 2,
                    GeplandeAankomstTijd = DateTime.Now,
                    GeplandeVertrekTijd = DateTime.Now,
                    GeplandeReisTijd = "2:05",
                    ReisDelen = new List<ReisDeel>()
                    {
                        new ReisDeel() { 
                            VervoerType = "Intercity", 
                            ReisStops = new List<ReisStop>()
                            {
                              new ReisStop() { Naam = "Delft", Vertrekspoor = "1a"}  ,
                              new ReisStop() { Naam = "Rotterdam", Vertrekspoor = "1a"}  
                            }
                        },
                        new ReisDeel() { 
                            VervoerType = "Intercity", 
                            ReisStops = new List<ReisStop>()
                            {
                              new ReisStop() { Naam = "Delft", Vertrekspoor = "1a"}  ,
                            }
                        }
                    }
                };
            }
            else
            {
                // Code runs "for real": Connect to service, etc...
                RefreshSearchHistory();
            }


          

           
        }

        private void AdviceTapCommandAction(Guid x)
        {
            NavigationService.NavigateTo(new Uri(string.Format("/Views/Reisadvies.xaml?id={0}", x), UriKind.Relative));

        }

        private void DoMijnStations()
        {
            NavigationService.NavigateTo(new Uri("/MainPage.xaml", UriKind.Relative));

        }

        private void AddReminder()
        {
            try
            {
                if (SelectedSearch != null
                    && ReisMogelijkheden != null
                    && SelectedReisMogelijkheid != null)
                {
                    //int index = ReisMogelijkheden.IndexOf(SelectedReisMogelijkheid);
                    //NavigationService.NavigateTo(new Uri(string.Format("/Views/Reminder.xaml?id={0}&index={1}&time={2}&spoor={3}", SelectedSearch.Id, index, SelectedReisMogelijkheid.GeplandeVertrekTijd, SelectedReisMogelijkheid.ReisDelen.First().ReisStops.First().Vertrekspoor), UriKind.Relative));

                    SaveAppointmentTask saveAp = new SaveAppointmentTask();
                    saveAp.StartTime = SelectedReisMogelijkheid.GeplandeVertrekTijd;
                    saveAp.EndTime = SelectedReisMogelijkheid.GeplandeAankomstTijd;
                    saveAp.Subject = SelectedSearch.DisplayFull;
                    saveAp.Details = SelectedReisMogelijkheid.GetAsText();
                    saveAp.Location = SelectedSearch.VanStation.Name;
                    saveAp.IsAllDayEvent = false;

                    saveAp.Show();

                }
            }
            catch { }
        }

        private void PinSearch()
        {
            if (SelectedSearch != null
                && ReisMogelijkheden != null)
            {
                int? index = null;

                var mogelijkheid = SelectedReisMogelijkheid;
                if (mogelijkheid == null)
                {
                    mogelijkheid = ReisMogelijkheden.Where(x => x.Optimaal).FirstOrDefault();
                }

                index = ReisMogelijkheden.IndexOf(mogelijkheid);

                if (index.HasValue && mogelijkheid != null)
                {
                    PlannerService.AddPermSearch(SelectedSearch, ReisMogelijkheden);

                    LiveTileService.CreateAdvies(SelectedSearch, index.Value, mogelijkheid.GeplandeVertrekTijd);
                }

            }
        }

        private void GoPrijs()
        {
            if (SelectedSearch != null)
            {
                NavigationService.NavigateTo(new Uri(string.Format("/Views/Prijs.xaml?id={0}", SelectedSearch.Id), UriKind.Relative));

            }
        }

        private void GoVertrektijden()
        {
            if (SelectedSearch != null)
            {
                string from = null;

                if (SelectedSearch.VanStation != null)
                    from = SelectedSearch.VanStation.Name;
               
                NavigationService.NavigateTo(new Uri(string.Format("/Views/StationTijden.xaml?id={0}", from), UriKind.Relative));
            }
        }

        private void DeleteSingleHistory(Guid id)
        {
            PlannerService.DeleteSearch(id);
            RefreshSearchHistory();
        }

        private void DeleteHistory()
        {
            PlannerService.DeleteSearchHistory();
            SearchHistory.Clear();
            RaisePropertyChanged(() => SearchHistory);

            SelectedReisMogelijkheid = null;
            SelectedSearch = null;
        }

        private void PlanTerugreis()
        {
            if (SelectedSearch != null)
            {
                string from = null;
                string to = null;
                string via = null;

                if (SelectedSearch.VanStation != null)
                    from = SelectedSearch.VanStation.Name;
                if (SelectedSearch.NaarStation != null)
                    to = SelectedSearch.NaarStation.Name;
                if (SelectedSearch.ViaStation != null)
                    via = SelectedSearch.ViaStation.Name;


                NavigationService.NavigateTo(new Uri(string.Format("/Views/Planner.xaml?from={0}&to={1}&via={2}", to, from, via), UriKind.Relative));
            }
        }

        private void RepeatSearch()
        {
            if (SelectedSearch != null)
            {
                RepeatSearch(SelectedSearch);
            }
        }

        private void RepeatSearch(PlannerSearch search)
        {
            string from = null;
            string to = null;
            string via = null;

            if (search.VanStation != null)
                from = search.VanStation.Name;
            if (search.NaarStation != null)
                to = search.NaarStation.Name;
            if (search.ViaStation != null)
                via = search.ViaStation.Name;


            NavigationService.NavigateTo(new Uri(string.Format("/Views/Planner.xaml?from={0}&to={1}&via={2}", from, to, via), UriKind.Relative));
        }

        private void DoSearch()
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                NavigationService.NavigateTo(new Uri("/Views/Planner.xaml", UriKind.Relative));
        }


        private bool CanDoEerder()
        {
            if (ReisMogelijkheden != null
               && SelectedReisMogelijkheid != null)
            {
                int index = ReisMogelijkheden.IndexOf(SelectedReisMogelijkheid);

                if (index > 0)
                    return true;
            }

            return false;
        }

        private bool CanDoLater()
        {
            if (ReisMogelijkheden != null
                && SelectedReisMogelijkheid != null)
            {
                int index = ReisMogelijkheden.IndexOf(SelectedReisMogelijkheid);

                if (ReisMogelijkheden.Count > index + 1)
                    return true;
            }

            return false;

        }





        private async void DoEerder()
        {

            try
            {
                var item = await Task.Run<ReisMogelijkheid>(() =>
                    {
                        if (ReisMogelijkheden != null
                            && SelectedReisMogelijkheid != null)
                        {
                            int index = ReisMogelijkheden.IndexOf(SelectedReisMogelijkheid);

                            if (index > 0)
                                return ReisMogelijkheden[index - 1];
                        }

                        return null;
                    });

                if (item != null)
                {
                    SelectedReisMogelijkheid = item;

                }
            }
            catch
            {
            }


        }

        private async void DoLater()
        {
            try
            {

                var item = await Task.Run<ReisMogelijkheid>(() =>
                    {
                        if (ReisMogelijkheden != null
                            && SelectedReisMogelijkheid != null)
                        {
                            int index = ReisMogelijkheden.IndexOf(SelectedReisMogelijkheid);

                            if (ReisMogelijkheden.Count > index + 1)
                            {
                                return ReisMogelijkheden[index + 1];



                            }
                        }
                        return null;

                    });

                if (item != null)
                {
                    SelectedReisMogelijkheid = item;

                }
            }
            catch { }


        }



        async void ReisadviesViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedSearch" && SelectedSearch != null)
                GetSearchResult(this.SelectedSearch);

            if (e.PropertyName == "SelectedReisMogelijkheid")
            {

                int delay = 30;

                LaterCommand.RaiseCanExecuteChanged();
                EerderCommand.RaiseCanExecuteChanged();

                //Does not work. Sometimes shows blank.
                try
                {
                    if (MainReisMogelijkheid != null)
                    {

                        MainReisMogelijkheid = null;
                        try
                        {
                            await Task.Delay(TimeSpan.FromMilliseconds(delay));
                        }
                        catch { }
                        finally{
                        MainReisMogelijkheid = SelectedReisMogelijkheid;
                            }
                    }
                }
                catch (Exception ex)
                {
                }
                finally
                {
                }



            }
        }


        ////public override void Cleanup()
        ////{
        ////    // Clean own resources if needed

        ////    base.Cleanup();
        ////}

        internal void Initialize(Guid? id, int? index)
        {
            IsInit = true;

            SelectedSearch = null;
            SelectedReisMogelijkheid = null;

            RefreshSearchHistory();

            if (id.HasValue)
            {
                var search = PlannerService.GetSearch(id.Value);

                if (search != null)
                {
                    _tempIndex = index;
                    SelectedSearch = search;
                }
                
            }
        }

        private async void GetSearchResult(PlannerSearch search)
        {
            List<ReisMogelijkheid> reisMogelijkheden = await DataLoader.LoadAsync(() => NSApiService.GetSearchResult(search));

            if (reisMogelijkheden != null)
            {
              //Set color
              bool useAlternate = false;

              foreach (var mogelijkheid in reisMogelijkheden)
              {
                //Set background color here, for performance
                mogelijkheid.IsAlternate = useAlternate;

                useAlternate = !useAlternate;
              }


              ReisMogelijkheden = reisMogelijkheden;

              if (_tempIndex.HasValue && ReisMogelijkheden.Count > _tempIndex.Value)
              {
                SelectedReisMogelijkheid = ReisMogelijkheden[_tempIndex.Value];
                _tempIndex = null;
              }
              else
              {
                SelectedReisMogelijkheid = reisMogelijkheden.Where(x => x.Optimaal).FirstOrDefault();
              }
            }
            else
              ReisMogelijkheden = reisMogelijkheden;

        }

        private void RefreshSearchHistory()
        {
            var items = PlannerService.GetListFromStore();

            SearchHistory.Clear();
            foreach (var item in items)
            {
                    SearchHistory.Add(item);
            }

            RaisePropertyChanged(() => SearchHistory);
            RaisePropertyChanged(() => SearchHistorySmall);
        }

        internal bool CanPin()
        {
            if (SelectedSearch != null
                && ReisMogelijkheden != null)
            {
                int? index = null;

                var mogelijkheid = SelectedReisMogelijkheid;
                if (mogelijkheid == null)
                {
                    mogelijkheid = ReisMogelijkheden.Where(x => x.Optimaal).FirstOrDefault();
                }

                index = ReisMogelijkheden.IndexOf(mogelijkheid);

                if (index.HasValue && mogelijkheid != null)
                {

                    bool exists = LiveTileService.ExistsCreateAdvies(SelectedSearch, index.Value, mogelijkheid.GeplandeVertrekTijd);

                    if (!exists)
                        return true;
                }

            }

            return false;
        }
    }
}