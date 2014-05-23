using ActueelNS.Services.Interfaces;
using GalaSoft.MvvmLight.Ioc;
using System;
using Treintijden.Shared.Services.Interfaces;
using Treintijden.PCL.Api.Models;
using Treintijden.PCL.Api.Interfaces;
using Q42.WinRT.Portable.Data;
using System.Threading.Tasks;

namespace Trein.ViewModel
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
    public class PrijsViewModel : CustomViewModelBase
    {
        public IPlannerService PlannerService { get; set; }
        public INSApiService NSApiService { get; set; }


        public string PageName
        {
            get { return _resourceLoader.GetString("prijs"); }

        }

        public DataLoader DataLoader { get; set; }

        private ReisPrijs _reisPrijs;

        public ReisPrijs ReisPrijs
        {
            get { return _reisPrijs; }
            set
            {
                _reisPrijs = value;
                RaisePropertyChanged(() => ReisPrijs);
            }
           
        }


        private PlannerSearch _plannerSearch;

        public PlannerSearch PlannerSearch
        {
            get { return _plannerSearch; }
            set { _plannerSearch = value;
            RaisePropertyChanged(() => PlannerSearch);
            }
        }



        /// <summary>
        /// Initializes a new instance of the PrijsViewModel class.
        /// </summary>
        public PrijsViewModel()
        {
            PlannerService = SimpleIoc.Default.GetInstance<IPlannerService>();
            NSApiService = SimpleIoc.Default.GetInstance<INSApiService>();

            DataLoader = new DataLoader();

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
                PlannerSearch = new PlannerSearch()
                {
                    VanStation = new Station() { Name = "Delft Zuid" },
                    NaarStation = new Station() { Name = "Amsterdam Lange Naam" }
                };

                //ReisPrijs = new ReisPrijs()
                //{
                //     Dag_1_20 = "10,20",
                //     Dag_1_40 = "15,2",
                //     Dag_1_Vol = "10,2",
                //     Enkel_1_20 = "10,2",
                //     Enkel_1_Vol = "10,2",
                //     Dag_2_20 = "10,2",
                //     Dag_2_40 = "10,2",
                //     Dag_2_Vol = "10,2",
                //     Enkel_1_40 = "10,2",
                //     Enkel_2_20 = "10,2",
                //     Enkel_2_40 = "10,2",
                //     Enkel_2_Vol = "10,2"
                //};
            }
            else
            {
                // Code runs "for real": Connect to service, etc...
            }


        }

        ////public override void Cleanup()
        ////{
        ////    // Clean own resources if needed

        ////    base.Cleanup();
        ////}

        internal async Task Initialize(Guid? id)
        {
            PlannerSearch = null;
            ReisPrijs = null;

            if (id.HasValue)
            {
                var search = await PlannerService.GetSearchAsync(id.Value);

                if (search != null)
                {
                    PlannerSearch = search;

                    ReisPrijs = await DataLoader.LoadAsync(() => NSApiService.GetPrijs(PlannerSearch));
                }

            }
        }
       
    }
}