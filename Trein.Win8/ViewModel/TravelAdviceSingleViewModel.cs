using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Windows.ApplicationModel.Resources;
using ActueelNS.Services.Interfaces;
using Q42.WinRT.Data;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Treintijden.PCL.Api.Models;
using Q42.WinRT.Portable.Data;
using Treintijden.PCL.Api.Interfaces;
using Treintijden.Shared.Services.Models;

namespace Trein.Win8.ViewModel
{
    public class TravelAdviceSingleViewModel : CustomViewModelBase
    {
        public DataLoader PrijsLoader { get; set; }
        private readonly IMyTrajectService _trajectService;
        private readonly INSApiService _prijsService;


        private ResourceLoader _resourceLoader = new ResourceLoader("Resources");


        private ReisMogelijkheid _reisMogelijkheid;

        public ReisMogelijkheid ReisMogelijkheid
        {
            get { return _reisMogelijkheid; }
            set
            {
                _reisMogelijkheid = value;
                RaisePropertyChanged(() => ReisMogelijkheid);
                RaisePropertyChanged(() => PageTitle);
            }
        }

        public PlannerSearch CurrentSearch
        {
            get
            {
              //TODO
              return null;
                //return ReisMogelijkheid.PlannerSearch;
            }
        }

        private ReisPrijs _prijs;

        public ReisPrijs Prijs
        {
            get { return _prijs; }
            set { _prijs = value;
            RaisePropertyChanged(() => Prijs);
            }
        }


        public string PageTitle
        {
            get
            {
                if (ReisMogelijkheid != null)
                    return string.Format(_resourceLoader.GetString("TravelAdviceSingleViewModelTitle"), ReisMogelijkheid.VertrekDisplayTijd, ReisMogelijkheid.AankomstDisplayTijd);
                else
                    return string.Empty;
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

        public TravelAdviceSingleViewModel(IMyTrajectService trajectService, INSApiService prijsService)
        {
            _trajectService = trajectService;
            _prijsService = prijsService;

            PrijsLoader = new DataLoader(true);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                
                   this.ReisMogelijkheid = new ReisMogelijkheid() {
                         Optimaal = true,
                          AantalOverstappen = 2,
                           GeplandeAankomstTijd = DateTime.Now,
                            GeplandeVertrekTijd = DateTime.Now,
                             GeplandeReisTijd = "2:05"
                    };

               
            }

            AddCommand = new RelayCommand(AddCommandAction);
            DeleteCommand = new RelayCommand(DeleteCommandAction);
        }

        public void Initialize(ReisMogelijkheid reisMogelijkheid)
        {
            this.ReisMogelijkheid = reisMogelijkheid;

            this.Prijs = null;

          //TODO
            //GetPrijs(reisMogelijkheid.PlannerSearch);
        }

        private async Task GetPrijs(PlannerSearch search)
        {
            ReisPrijs result = await PrijsLoader.LoadAsync(() => _prijsService.GetPrijs(search));
                this.Prijs = result;
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


        internal void SetPlannerSearch()
        {
            PlannerViewModel planner = SimpleIoc.Default.GetInstance<PlannerViewModel>();
            planner.NaarStation = this.CurrentSearch.NaarStation;
            planner.VanStation = this.CurrentSearch.VanStation;
            planner.ViaStation = this.CurrentSearch.ViaStation;
        }
    }
}
