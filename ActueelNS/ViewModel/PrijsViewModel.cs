﻿using GalaSoft.MvvmLight;
using ActueelNS.Services.Models;
using ActueelNS.Services.Interfaces;
using GalaSoft.MvvmLight.Ioc;
using System;
using AgFx;
using ActueelNS.Services.ViewModels;
using ActueelNS.Services.ViewModels.Context;
using ActueelNS.Resources;

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
    public class PrijsViewModel : CustomViewModelBase
    {
        public INavigationService NavigationService { get; set; }
        //public IPrijsService PrijsService { get; set; }
        public IPlannerService PlannerService { get; set; }


        public string PageName
        {
            get { return AppResources.prijs; }

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
            NavigationService = SimpleIoc.Default.GetInstance<INavigationService>();
            //PrijsService = SimpleIoc.Default.GetInstance<IPrijsService>();
            PlannerService = SimpleIoc.Default.GetInstance<IPlannerService>();

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

            DataManager.Current.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Current_PropertyChanged);

        }

        void Current_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsLoading")
                IsBusy = DataManager.Current.IsLoading;
        }
       


        ////public override void Cleanup()
        ////{
        ////    // Clean own resources if needed

        ////    base.Cleanup();
        ////}

        internal void Initialize(Guid? id)
        {
            PlannerSearch = null;

            if (id.HasValue)
            {
                var search = PlannerService.GetSearch(id.Value);

                if (search != null)
                {
                    PlannerSearch = search;

                    DataManager.Current.Load<PrijsDataModel>(new PrijsLoadContext(PlannerSearch), (vm) =>
                    {
                        ShowError = false;
                        ReisPrijs = vm.Prijs;

                    }, ex => { ShowError = true; });
                }

            }
        }

       
    }
}