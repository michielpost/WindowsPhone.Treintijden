using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActueelNS.Services.Interfaces;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Windows.ApplicationModel.Resources;
using Windows.UI.ApplicationSettings;
using Windows.ApplicationModel.Store;
using Treintijden.PCL.Api.Models;
using Treintijden.Shared.Services.Interfaces;

namespace Trein.Win8.ViewModel
{
    
    public enum InputType
    {
        None,
        Van,
        Naar,
        Via
    }

    public class PlannerViewModel : CustomViewModelBase
    {
        private ResourceLoader _resourceLoader = new ResourceLoader("Resources");

      //TODO
        //public WebServiceTextSearchProvider FromStationProvider;
        //public WebServiceTextSearchProvider ToStationProvider;
        //public WebServiceTextSearchProvider ViaStationProvider;
        //public WebServiceTextSearchProvider SearchStationProvider;


        public DonateViewModel Donate
        {
            get
            {
                return SimpleIoc.Default.GetInstance<DonateViewModel>();
            }
        }

        private ObservableCollection<Station> _allStations = new ObservableCollection<Station>();

        public ObservableCollection<Station> AllStations
        {
            get { return _allStations; }
            set
            {
                _allStations = value;
                RaisePropertyChanged(() => AllStations);
            }
        }

       

        private Station _vanStation;

        public Station VanStation
        {
            get {

                return _vanStation; }
            set
            {
                _vanStation = value;
                RaisePropertyChanged(() => VanStation);

                if (value != null)
                    ShowFromError = false;
            }
        }

        private Station _naarStation;

        public Station NaarStation
        {
            get
            {
                return _naarStation;
            }
            set
            {
                _naarStation = value;
                RaisePropertyChanged(() => NaarStation);

                if (value != null)
                    ShowToError = false;
            }
        }

        private Station _viaStation;

        public Station ViaStation
        {
            get
            {
                return _viaStation;
            }
            set
            {
                _viaStation = value;
                RaisePropertyChanged(() => ViaStation);
                RaisePropertyChanged(() => IsViaVisible);
            }
        }

        private bool _isViaVisible;

        public bool IsViaVisible
        {
            get
            {

                if (ViaStation != null)
                    return true;
                else
                    return _isViaVisible;
            }
            set
            {
                _isViaVisible = value;
                RaisePropertyChanged(() => IsViaVisible);
                RaisePropertyChanged("IsViaCustomVisible");
            }
        }

        private DateTime? _planDateTime;

        public DateTime? PlanDateTime
        {
            get { return _planDateTime; }
            set
            {
                _planDateTime = value;
                RaisePropertyChanged(() => PlanDateTime);
            }
        }

        private bool _showHslMessage;

        public bool ShowHslMessage
        {
            get { return _showHslMessage; }
            set { _showHslMessage = value;
            RaisePropertyChanged(() => ShowHslMessage);
            }
        }
        



        public ILastStationService LastStationService { get; set; }
        public IStationService StationService { get; set; }
        public ISettingService SettingService { get; set; }
       


        private string _type;

        public string Type
        {
            get { return _type; }
            set {
                if (value != null)
                {
                    _type = value;
                    RaisePropertyChanged(() => Type);
                }
            }
        }


        private bool _showFromError;

        public bool ShowFromError
        {
            get { return _showFromError; }
            set { _showFromError = value;
            RaisePropertyChanged(() => ShowFromError);
            }
        }

        private bool _showToError;

        public bool ShowToError
        {
            get { return _showToError; }
            set { _showToError = value;
            RaisePropertyChanged(() => ShowToError);
            }
        }

        private bool _showErrorDifferent;

        public bool ShowErrorDifferent
        {
            get { return _showErrorDifferent; }
            set { _showErrorDifferent = value;
            RaisePropertyChanged(() => ShowErrorDifferent);
            }
        }

        private bool _showUpgrade;

        public bool ShowUpgrade
        {
            get { return _showUpgrade; }
            set { _showUpgrade = value;
            RaisePropertyChanged(() => ShowUpgrade);
            }
        }
        

        public RelayCommand SearchCommand { get; private set; }
        public RelayCommand NoHslMessageCommand { get; private set; }
        public RelayCommand YesHslMessageCommand { get; private set; }
        public RelayCommand UpgradeCommand { get; private set; }



        public PlannerViewModel(ILastStationService lastStationService, IStationService stationService, ISettingService settingService)
        {
            LastStationService = lastStationService;
            StationService = stationService;
            SettingService = settingService;

            Type = "vertrek";

            

            Messenger.Default.Register<Station>(this, "SetGpsStation", SetGpsStation);


            SearchCommand = new RelayCommand(() => DoSearch());
          
          //TODO
          //NoHslMessageCommand = new RelayCommand(() => NoHslMessageAction());
            //UpgradeCommand = new RelayCommand(() => UpgradeAction());
            //YesHslMessageCommand = new RelayCommand(() => YesHslMessageAction());
            

            LoadAllStations();

          //TODO
            //ShowHslMessage = settingService.GetShowHslMessage();

            //this.FromStationProvider = new WebServiceTextSearchProvider();
            //this.FromStationProvider.InputChanged += stationProvider_InputChanged;

            //this.ToStationProvider = new WebServiceTextSearchProvider();
            //this.ToStationProvider.InputChanged += stationProvider_InputChanged;

            //this.ViaStationProvider = new WebServiceTextSearchProvider();
            //this.ViaStationProvider.InputChanged += stationProvider_InputChanged;

            //this.SearchStationProvider = new WebServiceTextSearchProvider();
            //this.SearchStationProvider.InputChanged += stationProvider_InputChanged;
        }

        void stationProvider_InputChanged(object sender, EventArgs e)
        {
            //TODO
          //WebServiceTextSearchProvider provider = (WebServiceTextSearchProvider)sender;

            //string inputString = provider.InputString;
            //if (!string.IsNullOrEmpty(inputString))
            //{

            //    string p = inputString.ToLower();

            //    var stations = this.AllStations.Where(x => x.Name.ToLower().StartsWith(p)).Take(5);

            //    if (stations.Count() < 5)
            //    {
            //        var extraStations = AllStations.Where(x => x.StartsWith(p)).Take(5 - stations.Count());

            //        stations = stations.Union(extraStations);
            //    }

            //    provider.LoadItems(stations);
            //}
            //else
            //{
            //    provider.Reset();
            //}
        }

       
      //TODO
        //private void UpgradeAction()
        //{
        //    App.RootFrame.Navigate(typeof(DonatePage));
        //}

      //TODO
        //private void NoHslMessageAction()
        //{
        //    ShowHslMessage = false;
        //    SettingService.SaveShowHslMessage(false);
        //}

      //TODO
        //private void YesHslMessageAction()
        //{
        //    ShowHslMessage = false;
        //    SettingService.SaveShowHslMessage(false);
        //    SettingsPane.Show();
        //}

        private async Task LoadAllStations()
        {
            var list = await StationService.GetMyStationsAsync();
            AllStations = new ObservableCollection<Station>(list);
        }

        public void SetGpsStation(Station input)
        {
            if (this.VanStation == null)
                VanStation = input;
        }

        private async void DoSearch()
        {
            if (!PlanDateTime.HasValue || PlanDateTime.Value == DateTime.MinValue)
                PlanDateTime = DateTime.Now;
          
            if (string.IsNullOrEmpty(Type))
                Type = "vertrek";

          //TODO
            //bool isHsl = SettingService.GetShowHsl();
            //bool isYearCard = SettingService.GetOvCard();

            //Create planner object with GUID
            PlannerSearch search = new PlannerSearch()
            {
                Id = Guid.NewGuid(),
                SearchDateTime = DateTime.Now,
                VanStation = VanStation,
                NaarStation = NaarStation,
                ViaStation = ViaStation,
                //IsHogesnelheid = isHsl, TODO
                //IsYearCard = isYearCard, TODO
                Type = Type,
                Date = PlanDateTime.Value,
                Time = PlanDateTime.Value
            };

            if (search.VanStation == null)
                ShowFromError = true;

            if (search.NaarStation == null)
                ShowToError = true;

            if (search.VanStation != null
                && search.NaarStation != null)
            {
               //TODO Save default settings
                if (search.VanStation == search.NaarStation)
                {
                    this.ShowErrorDifferent = true;
                }
                else
                {

                    //Save last used
                    var list = await LastStationService.Add(VanStation, NaarStation, ViaStation);
                    //LastStations = new ObservableCollection<Station>(list);

                    //LoadLastStations();

                    //Navigate to new page 
                  //TODO
                    //App.RootFrame.Navigate(typeof(TravelAdvicePage), search);
                }
            }


        }


        internal void SetDateNow()
        {
            PlanDateTime = DateTime.Now;

        }


    

      

        internal void FinishAutoComplete(InputType type, string value)
        {

            var selectedStation = GetStationByName(value);


            if (type == InputType.Van)
            {
                this.VanStation = selectedStation;
            }
            else if (type == InputType.Naar)
            {
                this.NaarStation = selectedStation;
              
            }
            else if (type == InputType.Via)
            {
                this.ViaStation = selectedStation;

            }


        }

        public Station GetStationByName(string value)
        {
            var selectedStation = AllStations.Where(x => x.Name.ToLower() == value.ToLower()).FirstOrDefault();
            if (selectedStation == null)
            {
                selectedStation = AllStations.Where(x => x.NamesExtra.Select(y => y.ToLower()).Contains(value.ToLower())).FirstOrDefault();
            }
            return selectedStation;
        }



        internal void Initialize()
        {
            //this.PlanDateTime = null;

            this.ShowErrorDifferent = false;
            this.ShowToError = false;
            this.ShowFromError = false;

        }

       

        public void WisselVanNaar()
        {
            var tempNaar = this._naarStation;

            this.NaarStation = this._vanStation;
            this.VanStation = tempNaar;

            if (VanStation != null)
                Messenger.Default.Send(VanStation.Name, "setVan");
            if (NaarStation != null)
                Messenger.Default.Send(NaarStation.Name, "setNaar");
        }
    }
}
