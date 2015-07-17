using ActueelNS.Services.Interfaces;
using System.Collections.ObjectModel;
using ActueelNS.Services;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Command;
using System;
using GalaSoft.MvvmLight.Ioc;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using Treintijden.Shared.Services.Interfaces;
using Treintijden.PCL.Api.Models;
using Treintijden.Shared.Services.Models;

namespace Trein.ViewModel
{

    public class AlphaKeyGroup<T> : List<T>
    {
        const string GlobeGroupKey = "\uD83C\uDF10";

        /// <summary>
        /// The Key of this group.
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Public ctor.
        /// </summary>
        /// <param name="key">The key for this group.</param>
        public AlphaKeyGroup(string key)
        {
            Key = key;
        }

        /// <summary>
        /// Create a list of AlphaGroup<T> with keys set by a SortedLocaleGrouping.
        /// </summary>
        /// <param name="slg">The </param>
        /// <returns>Theitems source for a LongListSelector</returns>
        //private static List<AlphaKeyGroup<T>> CreateDefaultGroups(SortedLocaleGrouping slg)
        //{
      //TODO
        //    List<AlphaKeyGroup<T>> list = new List<AlphaKeyGroup<T>>();

        //    foreach (string key in slg.GroupDisplayNames)
        //    {
        //        if (key == "...")
        //        {
        //            list.Add(new AlphaKeyGroup<T>(GlobeGroupKey));
        //        }
        //        else
        //        {
        //            list.Add(new AlphaKeyGroup<T>(key));
        //        }
        //    }

        //    return list;
        //}

        /// <summary>
        /// Create a list of AlphaGroup<T> with keys set by a SortedLocaleGrouping 
        /// using the current threads culture to determine which alpha keys to
        /// include.
        /// </summary>
        /// <param name="items">The items to place in the groups.</param>
        /// <param name="getKey">A delegate to get the key from an item.</param>
        /// <param name="sort">Will sort the data if true.</param>
        /// <returns>An items source for a LongListSelector</returns>
        public static List<AlphaKeyGroup<T>> CreateGroups(IEnumerable<T> items, Func<T, string> keySelector, bool sort)
        {
            return CreateGroups(
                items,
                CultureInfo.CurrentUICulture,
                keySelector,
                sort);
        }

        /// <summary>
        /// Create a list of AlphaGroup<T> with keys set by a SortedLocaleGrouping.
        /// </summary>
        /// <param name="items">The items to place in the groups.</param>
        /// <param name="ci">The CultureInfo to group and sort by.</param>
        /// <param name="getKey">A delegate to get the key from an item.</param>
        /// <param name="sort">Will sort the data if true.</param>
        /// <returns>An items source for a LongListSelector</returns>
        public static List<AlphaKeyGroup<T>> CreateGroups(IEnumerable<T> items, CultureInfo ci, Func<T, string> keySelector, bool sort)
        {
          //TODO
          throw new Exception();
            //SortedLocaleGrouping slg = new SortedLocaleGrouping(ci);
            //List<AlphaKeyGroup<T>> list = CreateDefaultGroups(slg);

            //foreach (T item in items)
            //{
            //    int index = 0;
            //    //if (slg.SupportsPhonetics)
            //    //{
            //    //check if your database has yomi string for item
            //    //if it does not, then do you want to generate Yomi or ask the user for this item.
            //    //index = slg.GetGroupIndex(getKey(Yomiof(item)));
            //    //}
            //    //else
            //    {
            //        index = slg.GetGroupIndex(keySelector(item));
            //    }

            //    if (index >= 0 && index < list.Count)
            //    {
            //        list[index].Add(item);
            //    }
            //}

            //if (sort)
            //{
            //    foreach (AlphaKeyGroup<T> group in list)
            //    {
            //        group.Sort((c0, c1) => { return ci.CompareInfo.Compare(keySelector(c0), keySelector(c1)); });
            //    }
            //}

            //return list;
        }
    }

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
    public class StationPickerViewModel : CustomViewModelBase
    {

        public IStationService StationService { get; set; }
        public IStationNameService StationNameService { get; set; }
        public ISettingService SettingService { get; set; }

        private List<AlphaKeyGroup<Station>> _stations;
        public List<AlphaKeyGroup<Station>> Stations
        {
            get
            {
                return _stations;
            }
            set
            {
                _stations = value;
                RaisePropertyChanged(() => Stations);

            }
        }

        private ObservableCollection<Station> _stationList = new ObservableCollection<Station>();
        public ObservableCollection<Station> StationList
        {
            get
            {
                return _stationList;
            }
            set
            {
                _stationList = value;
                //RaisePropertyChanged(() => StationList);
            }
        }

        private AppSetting _settings;

        public AppSetting Settings
        {
            get { return _settings; }
            set
            {
                _settings = value;
            }
        }

       

        public string PageName
        {
            get
            {
                return _resourceLoader.GetString("stationkeuze");
            }
        }


        public RelayCommand<Station> StationAddCommand { get; private set; }
        public RelayCommand<Station> VertrektijdenCommand { get; private set; }


        /// <summary>
        /// Initializes a new instance of the StationPickerViewModel class.
        /// </summary>
        public StationPickerViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                // Code runs "for real"
                
            }

            StationService = SimpleIoc.Default.GetInstance<IStationService>();
            StationNameService = SimpleIoc.Default.GetInstance<IStationNameService>();
            SettingService = SimpleIoc.Default.GetInstance<ISettingService>();






            StationAddCommand = new RelayCommand<Station>(async x => await AddStation(x));
            VertrektijdenCommand = new RelayCommand<Station>(x => GoVertrektijdenStation(x));
           
        }

        private async Task AddStation(Station station)
        {
            await StationService.AddStationAsync(station);

            if (NavigationService.CanGoBack())
              NavigationService.GoBack();
            else
              NavigationService.NavigateToMainPage();

        }

        private void GoVertrektijdenStation(Station station)
        {
                NavigationService.NavigateToStationTijden(station.Name);

        }


        ////public override void Cleanup()
        ////{
        ////    // Clean own resources if needed

        ////    base.Cleanup();
        ////}

        internal void Load()
        {
            Settings = SettingService.GetSettings();

            StationList.Clear();

           
        }

        internal void LoadForPicker()
        {
            if (Stations == null)
            {
                var all = StationNameService.GetStations();

                Stations = AlphaKeyGroup<Station>.CreateGroups(
                   all,
                   (Station s) => { return s.Name; },
                   true);
            }
        }

        internal void SeachStation(string p)
        {
          if (string.IsNullOrEmpty(p))
            StationList.Clear();
          else
          {
            p = p.ToLower();


            var stations = StationNameService.GetStations().Where(x => x.Name.ToLower().StartsWith(p)).OrderBy(x => x.Sort).Take(7);

            if (stations.Count() < 7)
            {
              var extraStations = StationNameService.GetStations().Where(x => x.StartsWith(p)).OrderBy(x => x.Sort).Take(7 - stations.Count());

              stations = stations.Union(extraStations);
            }

            StationList.Clear();

            foreach (var s in stations)
              StationList.Add(s);

          }


        }
    }
}