using System.Collections.Generic;
using System;
using ActueelNS.Services.Interfaces;
using GalaSoft.MvvmLight.Ioc;
using System.Collections.ObjectModel;
using Microsoft.Phone.Net.NetworkInformation;
using ActueelNS.Resources;
using GalaSoft.MvvmLight.Command;
using Treintijden.PCL.Api.Models;
using Treintijden.PCL.Api.Interfaces;
using System.Threading.Tasks;
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
    public class StoringenViewModel : CustomViewModelBase
    {

        //public IStoringenService StoringenService { get; set; }
        public ILiveTileService LiveTileService { get; set; }
        public INavigationService NavigationService { get; set; }
        public INSApiService NSApiService { get; set; }



        private StoringenEnWerkzaamheden _storingenDataModel;

        public StoringenEnWerkzaamheden StoringDataModel
        {
            get { return _storingenDataModel; }
            set
            {
                _storingenDataModel = value;
                RaisePropertyChanged(() => StoringDataModel);
                RaisePropertyChanged(() => Werkzaamheden);

                if (value != null)
                {
                    foreach (var s in value.Storingen)
                        CurrentStoringen.Add(s);
                }
            }
        }


        private ObservableCollection<Storing> _currentStoringe = new ObservableCollection<Storing>();
        public ObservableCollection<Storing> CurrentStoringen
        {
            get
            {
                return _currentStoringe;
            }

        }

        public List<Werkzaamheden> Werkzaamheden
        {
            get {
                if (StoringDataModel != null)
                    return StoringDataModel.Werkzaamheden;

                return null; 
            }

        }

        public DataLoader DataLoader { get; set; }

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
            NSApiService = SimpleIoc.Default.GetInstance<INSApiService>();

            DataLoader = new DataLoader();


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
                LoadStoringen();
            }

            PinCommand = new RelayCommand(() => DoPin());
            ToListCommand = new RelayCommand(() => ToList());

            PageName = AppResources.storingen;

        }

        private async Task LoadStoringen()
        {
            StoringDataModel = await DataLoader.LoadAsync(() => NSApiService.GetStoringenEnWerkzaamheden(string.Empty));
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