using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActueelNS.Services.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Windows.UI.Xaml.Media;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using GalaSoft.MvvmLight.Ioc;
using Treintijden.PCL.Api.Models;
using Treintijden.Shared.Services.Interfaces;
using Treintijden.PCL.Api.Interfaces;
using Treintijden.Shared.Services.Models;
using Q42.WinRT.Portable.Data;

namespace Trein.Win8.ViewModel
{
    public class TravelAdviceViewModel : CustomViewModelBase
    {
        private readonly INSApiService _plannerService;
        private readonly IMyTrajectService _trajectService;
        private readonly IPlannerService _searchHistoryService;
        public DataLoader DataLoader { get; set; }



        private List<ReisMogelijkheid> _reisMogelijkheden = new List<ReisMogelijkheid>();

        public List<ReisMogelijkheid> ReisMogelijkheden
        {
            get { return _reisMogelijkheden; }
            set
            {
                _reisMogelijkheden = value;
                RaisePropertyChanged(() => ReisMogelijkheden);
                RaisePropertyChanged(() => ReisMogelijkheidOptimaal);
            }
        }


        public ReisMogelijkheid ReisMogelijkheidOptimaal
        {
            get
            {
                if (ReisMogelijkheden != null)
                {
                    return ReisMogelijkheden.Where(x => x.Optimaal).FirstOrDefault();
                }

                return null;
            }
            
        }
        

        private PlannerSearch _currentSearch;

        public PlannerSearch CurrentSearch
        {
            get { return _currentSearch; }
            set
            {
                _currentSearch = value;
                SetIsFav();
                RaisePropertyChanged(() => CurrentSearch);
            }
        }

      

        

        private bool _isFav;

        public bool IsFav
        {
            get { return _isFav; }
            set
            {
                _isFav = value;
                RaisePropertyChanged(() => IsFav);
            }
        }

        public RelayCommand AddCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }



        public TravelAdviceViewModel(INSApiService plannerService, IMyTrajectService trajectService, IPlannerService searchHistoryService)
        {
            _plannerService = plannerService;
            _trajectService = trajectService;
            _searchHistoryService = searchHistoryService;

            DataLoader = new DataLoader();

            if (ViewModelBase.IsInDesignModeStatic)
            {
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
                          AantalOverstappen = 1,
                           GeplandeAankomstTijd = DateTime.Now.AddHours(1),
                            GeplandeVertrekTijd = DateTime.Now,
                             GeplandeReisTijd = "7:05"
                    }
                };

                CurrentSearch = new PlannerSearch()
                {
                  Date = DateTime.Now,
                    Time = DateTime.Now,
                    NaarStation = new Station() { Name = "Delft" },
                    VanStation = new Station() { Name = "Amsterdam" },
                     Type = "Vertrek"
                };
            }


            AddCommand = new RelayCommand(AddCommandAction);
            DeleteCommand = new RelayCommand(DeleteCommandAction);
        }

        private async void DeleteCommandAction()
        {
            SimpleIoc.Default.GetInstance<MainViewModel>().Trajecten = new System.Collections.ObjectModel.ObservableCollection<Traject>(await _trajectService.Delete(new Traject()
            {
                From = this.CurrentSearch.VanStation,
                To = this.CurrentSearch.NaarStation,
                Via = this.CurrentSearch.ViaStation
            }));

            IsFav = false;

        }

        private async void AddCommandAction()
        {
            SimpleIoc.Default.GetInstance<MainViewModel>().Trajecten = new System.Collections.ObjectModel.ObservableCollection<Traject>(await _trajectService.Add(new Traject()
            {
                From = this.CurrentSearch.VanStation,
                To = this.CurrentSearch.NaarStation,
                Via = this.CurrentSearch.ViaStation
            }));

            IsFav = true;
        }

        private async void SetIsFav()
        {
            Traject t = new Traject()
            {
                From = this.CurrentSearch.VanStation,
                To = this.CurrentSearch.NaarStation,
                Via = this.CurrentSearch.ViaStation
            };

            var list = await _trajectService.GetAll();

            var found = list.Where(x => x.UniqueId == t.UniqueId).FirstOrDefault();

            IsFav = (found != null);
        }

       

      //TODO
        //public void Initialize(PlannerSearch search)
        //{
        //    ReisMogelijkheden.Clear();
        //    LoadingState = ViewModel.LoadingState.None;

        //    this.CurrentSearch = search;

        //    GetSearchResult(search);
        //}

        internal void Initialize(PlannerSearch searchHistory)
        {
            //ReisMogelijkheden.Clear();
          DataLoader.LoadingState = LoadingState.None;
            //TODO
          //this.CurrentSearch = searchHistory.PlannerSearch;
            //ReisMogelijkheden = new List<ReisMogelijkheid>(searchHistory.Reismogelijkheden);

            //foreach (var r in ReisMogelijkheden)
            //{
            //    //Add original search
            //    r.PlannerSearch = CurrentSearch;
            //}

            ////Event om selected te zetten
            //var optimaal = searchHistory.Reismogelijkheden.Where(x => x.Optimaal).FirstOrDefault();
            //Messenger.Default.Send<ReisMogelijkheid>(optimaal, "SetOptimaal");
        }

        private async void GetSearchResult(PlannerSearch search)
        {


                List<ReisMogelijkheid> reisMogelijkheden = await DataLoader.LoadAsync(() => _plannerService.GetSearchResult(search));

                SimpleIoc.Default.GetInstance<MainViewModel>().SearchHistory = new System.Collections.ObjectModel.ObservableCollection<PlannerSearch>(await _searchHistoryService.GetListFromStoreAsync());

                //Set color
                //SolidColorBrush backgroundColor = (SolidColorBrush)App.Current.Resources["BackgroundColor"];
                //SolidColorBrush alternateColor = (SolidColorBrush)App.Current.Resources["AlternateColor"];

                //foreach (var mogelijkheid in reisMogelijkheden)
                //{
                //    //Set background color here, for performance
                //    if (useAlternate)
                //        mogelijkheid.SetBackground(alternateColor);
                //    else
                //        mogelijkheid.SetBackground(backgroundColor);

                //    useAlternate = !useAlternate;
                //}


                ReisMogelijkheden = reisMogelijkheden;

              //TODO
                //foreach (var r in ReisMogelijkheden)
                //{
                //    //Add original search
                //    r.PlannerSearch = CurrentSearch;
                //}

               //Event om selected te zetten
                var optimaal = reisMogelijkheden.Where(x => x.Optimaal).FirstOrDefault();
                Messenger.Default.Send<ReisMogelijkheid>(optimaal, "SetOptimaal");

            
        }




        internal void SetPlannerSearch()
        {
            PlannerViewModel planner = SimpleIoc.Default.GetInstance<PlannerViewModel>();
            planner.NaarStation = this.CurrentSearch.NaarStation;
            planner.VanStation = this.CurrentSearch.VanStation;
            planner.ViaStation = this.CurrentSearch.ViaStation;
        }
    }
}
