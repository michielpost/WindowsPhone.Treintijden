using ActueelNS.Services.Models;
using System.Collections.Generic;
using System;
using ActueelNS.Services.Interfaces;
using GalaSoft.MvvmLight.Ioc;
using System.Collections.ObjectModel;
using AgFx;
using ActueelNS.Services.ViewModels;
using Microsoft.Phone.Net.NetworkInformation;
using ActueelNS.Resources;
using GalaSoft.MvvmLight.Command;

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
    public class StoringenViewModel : CustomViewModelBase
    {

        //public IStoringenService StoringenService { get; set; }
        public ILiveTileService LiveTileService { get; set; }
        public INavigationService NavigationService { get; set; }

        public StoringDataModel StoringDataModel { get; set; }

        public ObservableCollection<Storing> CurrentStoringen
        {
            get { return StoringDataModel.Storingen; }

        }

        public ObservableCollection<Werkzaamheden> Werkzaamheden
        {
            get { return StoringDataModel.Werkzaamheden; }

        }

        private bool _showError;

        public bool ShowError
        {
            get { return _showError; }
            set
            {
                _showError = value;
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


        private string _pageName;

        public string PageName
        {
            get { return _pageName.ToLower(); }
            set
            {
                _pageName = value;
                RaisePropertyChanged(() => PageName);
            }
        }

        public RelayCommand PinCommand { get; private set; }
        public RelayCommand ToListCommand { get; private set; }


        /// <summary>
        /// Initializes a new instance of the StoringenViewModel class.
        /// </summary>
        public StoringenViewModel()
        {
            LiveTileService = SimpleIoc.Default.GetInstance<ILiveTileService>();
            NavigationService = SimpleIoc.Default.GetInstance<INavigationService>();

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.

                var list = new ObservableCollection<Storing>();

                list.Add(new Storing()
                {
                    Bericht = "Dit is een hele lange storing. Dit is een hele lange storing. Dit is een hele lange storing. Dit is een hele lange storing. Dit is een hele lange storing. Dit is een hele lange storing. ",
                     Datum = DateTime.Now,
                    Reden = "seinstoring",
                      Traject = "Delft - Amsterdam vv",
                      
                });

                list.Add(new Storing()
                {
                    Bericht = "Dit is een hele lange storing. Dit is een hele lange storing. Dit is een hele lange storing. Dit is een hele lange storing. Dit is een hele lange storing. Dit is een hele lange storing. ",
                    Datum = DateTime.Now,
                    Reden = "seinstoring",
                    Traject = "Delft - Amsterdam vv"
                });

                //CurrentStoringen = list;



                var werkList = new ObservableCollection<Werkzaamheden>();

                werkList.Add(new Werkzaamheden()
                {
                    Advies = "Maak gebruik van de bus.... ",
                    Periode = DateTime.Now.ToString(),
                    Reden = "seinstoring",
                    Traject = "Delft - Amsterdam vv",
                    Vertraging = "ga eerder op reis"
                });

                werkList.Add(new Werkzaamheden()
                {
                    Advies = "Dit is een hele lange storing. Dit is een hele lange storing. Dit is een hele lange storing. Dit is een hele lange storing. Dit is een hele lange storing. Dit is een hele lange storing. ",
                    Periode = DateTime.Now.ToString(),
                    Reden = "seinstoring",
                    Traject = "Delft - Amsterdam vv",
                    Vertraging = "ga eerder op reis"
                });

                //Werkzaamheden = werkList;



            }
            else
            {
                ShowError = false;
                IsBusy = true;
                
                StoringDataModel = DataManager.Current.Load<StoringDataModel>(string.Empty, (vm) =>
                {
                    IsBusy = false;

                    RaisePropertyChanged(() => CurrentStoringen);
                    RaisePropertyChanged(() => Werkzaamheden);

                }, ex => {
                    ShowError = true;
                    IsBusy = false;

                });
                
            }

            PinCommand = new RelayCommand(() => DoPin());
            ToListCommand = new RelayCommand(() => ToList());

            PageName = AppResources.storingen;

        }


        private void DoPin()
        {
            LiveTileService.CreateStoringen();
        }

        private void ToList()
        {
            NavigationService.NavigateTo(new Uri("/MainPage.xaml", UriKind.Relative));
        }



    }
}