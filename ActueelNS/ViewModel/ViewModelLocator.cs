/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:ActueelNS.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
  
  OR (WPF only):
  
  xmlns:vm="clr-namespace:ActueelNS.ViewModel"
  DataContext="{Binding Source={x:Static vm:ViewModelLocatorTemplate.ViewModelNameStatic}}"
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using ActueelNS.Services.Interfaces;
using ActueelNS.Services;
using Treintijden.Shared.Services.Interfaces;
using Treintijden.Shared.Services;
using Treintijden.PCL.Api.Interfaces;
using Treintijden.Shared.Services.WP;
using Treintijden.PCL.Api;
namespace ActueelNS.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// Use the <strong>mvvmlocatorproperty</strong> snippet to add ViewModels
    /// to this locator.
    /// </para>
    /// <para>
    /// In Silverlight and WPF, place the ViewModelLocatorTemplate in the App.xaml resources:
    /// </para>
    /// <code>
    /// &lt;Application.Resources&gt;
    ///     &lt;vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:ActueelNS.ViewModel"
    ///                                  x:Key="Locator" /&gt;
    /// &lt;/Application.Resources&gt;
    /// </code>
    /// <para>
    /// Then use:
    /// </para>
    /// <code>
    /// DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
    /// </code>
    /// <para>
    /// You can also use Blend to do all this with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// <para>
    /// In <strong>*WPF only*</strong> (and if databinding in Blend is not relevant), you can delete
    /// the Main property and bind to the ViewModelNameStatic property instead:
    /// </para>
    /// <code>
    /// xmlns:vm="clr-namespace:ActueelNS.ViewModel"
    /// DataContext="{Binding Source={x:Static vm:ViewModelLocatorTemplate.ViewModelNameStatic}}"
    /// </code>
    /// </summary>
    public class ViewModelLocator
    {
        private static MainViewModel _main;
        private static StationPickerViewModel _stationPicker;
        private static StationTijdenViewModel _stationTijden;
        private static StoringenViewModel _storingen;
        private static ReisadviesViewModel _reisadvies;
        private static PlannerViewModel _planner;
        private static AboutViewModel _about;
        private static PrijsViewModel _prijs;
        private static SettingsViewModel _settings;
        private static ReminderViewModel _reminder;
        private static DonateViewModel _donate;
        private static MapViewModel _map;
        private static RitInfoViewModel _ritInfo;

        private static GpsWatcherModel _gpsWatcher;


        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            if (ViewModelBase.IsInDesignModeStatic)
            {
                // Create design time view models
                //SimpleIoc.Default.Register<IStationService, StationServiceMock>();
                //SimpleIoc.Default.Register<IStoringenService, StoringenServiceMock>();
                //SimpleIoc.Default.Register<IVertrektijdenService, VertrektijdenServiceMock>();
                
            }
            else
            {
                // Create run time view models
                SimpleIoc.Default.Register<IStationService, StationService>();
                SimpleIoc.Default.Register<IStationNameService, StationNameService>();
                //SimpleIoc.Default.Register<IStoringenService, StoringenService>();
                //SimpleIoc.Default.Register<IVertrektijdenService, VertrektijdenService>();
            }

            SimpleIoc.Default.Register<IPlannerService, PlannerService>();
            SimpleIoc.Default.Register<INSApiService>(() => new CachedNSApiService(new NSApiService()));

            SimpleIoc.Default.Register<INavigationService, NavigationService>();
            SimpleIoc.Default.Register<ILiveTileService, LiveTileService>();
            SimpleIoc.Default.Register<IReminderService, ReminderService>();
            SimpleIoc.Default.Register<ISettingService, SettingService>();


            CreateMain();
            //CreateStationPicker();
            //CreateStationTijden();
        }

        private static void CreatGpsWatcher()
        {
            if (_gpsWatcher == null)
            {
                _gpsWatcher = new GpsWatcherModel();
            }
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        public static GpsWatcherModel GpsWatcherStatic
        {
            get
            {
                if (_gpsWatcher == null)
                {
                    CreatGpsWatcher();
                }

                return _gpsWatcher;
            }
        }

        /// <summary>
        /// Gets the StationPicker property.
        /// </summary>
        public GpsWatcherModel GpsWatcherViewModel
        {
            get
            {
                return GpsWatcherStatic;
            }
        }


        private static void CreateSettings()
        {
            if (_settings == null)
            {
                _settings = new SettingsViewModel();
            }
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        public static SettingsViewModel SettingsStatic
        {
            get
            {
                if (_settings == null)
                {
                    CreateSettings();
                }

                return _settings;
            }
        }

        /// <summary>
        /// Gets the StationPicker property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public SettingsViewModel SettingsViewModel
        {
            get
            {
                return SettingsStatic;
            }
        }


        private static void CreateReminder()
        {
            if (_reminder == null)
            {
                _reminder = new ReminderViewModel();
            }
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        public static ReminderViewModel ReminderStatic
        {
            get
            {
                if (_reminder == null)
                {
                    CreateReminder();
                }

                return _reminder;
            }
        }

        /// <summary>
        /// Gets the StationPicker property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ReminderViewModel ReminderViewModel
        {
            get
            {
                return ReminderStatic;
            }
        }


        private static void CreatePrijs()
        {
            if (_prijs == null)
            {
                _prijs = new PrijsViewModel();
            }
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        public static PrijsViewModel PrijsStatic
        {
            get
            {
                if (_prijs == null)
                {
                    CreatePrijs();
                }

                return _prijs;
            }
        }

        /// <summary>
        /// Gets the StationPicker property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public PrijsViewModel PrijsViewModel
        {
            get
            {
                return PrijsStatic;
            }
        }






        private static void CreateAbout()
        {
            if (_about == null)
            {
                _about = new AboutViewModel();
            }
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        public static AboutViewModel AboutStatic
        {
            get
            {
                if (_about == null)
                {
                    CreateAbout();
                }

                return _about;
            }
        }

        /// <summary>
        /// Gets the StationPicker property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public AboutViewModel AboutViewModel
        {
            get
            {
                return AboutStatic;
            }
        }



        private static void CreateDonate()
        {
            if (_donate == null)
            {
                _donate = new DonateViewModel();
            }
        }

        /// <summary>
        /// Gets the DonateStatic property.
        /// </summary>
        public static DonateViewModel DonateStatic
        {
            get
            {
                if (_donate == null)
                {
                    CreateDonate();
                }

                return _donate;
            }
        }

        /// <summary>
        /// Gets the StationPicker property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public DonateViewModel DonateViewModel
        {
            get
            {
                return DonateStatic;
            }
        }


        private static void CreateMap()
        {
            if (_map == null)
            {
                _map = new MapViewModel();
            }
        }

        /// <summary>
        /// Gets the MapViewModel property.
        /// </summary>
        public static MapViewModel MapStatic
        {
            get
            {
                if (_map == null)
                {
                    CreateMap();
                }

                return _map;
            }
        }

        /// <summary>
        /// Gets the MapViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MapViewModel MapViewModel
        {
            get
            {
                return MapStatic;
            }
        }





        private static void CreateReisadvies()
        {
            if (_reisadvies == null)
            {
                _reisadvies = new ReisadviesViewModel();
            }
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        public static ReisadviesViewModel ReisadviesStatic
        {
            get
            {
                if (_reisadvies == null)
                {
                    CreateReisadvies();
                }

                return _reisadvies;
            }
        }

        /// <summary>
        /// Gets the StationPicker property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ReisadviesViewModel ReisadviesViewModel
        {
            get
            {
                return ReisadviesStatic;
            }
        }



        private static void CreatePlanner()
        {
            if (_planner == null)
            {
                _planner = new PlannerViewModel();
            }
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        public static PlannerViewModel PlannerStatic
        {
            get
            {
                if (_planner == null)
                {
                    CreatePlanner();
                }

                return _planner;
            }
        }

        /// <summary>
        /// Gets the StationPicker property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public PlannerViewModel PlannerViewModel
        {
            get
            {
                return PlannerStatic;
            }
        }



        private static void CreateRitInfo()
        {
            if (_ritInfo == null)
            {
                _ritInfo = new RitInfoViewModel();
            }
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        public static RitInfoViewModel RitInfoStatic
        {
            get
            {
                if (_ritInfo == null)
                {
                    CreateRitInfo();
                }

                return _ritInfo;
            }
        }

        /// <summary>
        /// Gets the RitInfoViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public RitInfoViewModel RitInfoViewModel
        {
            get
            {
                return RitInfoStatic;
            }
        }


       
        private static void CreateStoringen()
        {
            if (_storingen == null)
            {
                _storingen = new StoringenViewModel();
            }
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        public static StoringenViewModel StoringenStatic
        {
            get
            {
                if (_storingen == null)
                {
                    CreateStoringen();
                }

                return _storingen;
            }
        }

        /// <summary>
        /// Gets the StationPicker property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public StoringenViewModel StoringenViewModel
        {
            get
            {
                return StoringenStatic;
            }
        }


        private static void CreateStationTijden()
        {
            if (_stationTijden == null)
            {
                _stationTijden = new StationTijdenViewModel();
            }
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        public static StationTijdenViewModel StationTijdenStatic
        {
            get
            {
                if (_stationTijden == null)
                {
                    CreateStationTijden();
                }

                return _stationTijden;
            }
        }

        /// <summary>
        /// Gets the StationPicker property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public StationTijdenViewModel StationTijden
        {
            get
            {
                return StationTijdenStatic;
            }
        }


        private static void CreateStationPicker()
        {
            if (_stationPicker == null)
            {
                _stationPicker = new StationPickerViewModel();
            }
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        public static StationPickerViewModel StationPickerStatic
        {
            get
            {
                if (_stationPicker == null)
                {
                    CreateStationPicker();
                }

                return _stationPicker;
            }
        }

        /// <summary>
        /// Gets the StationPicker property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public StationPickerViewModel StationPicker
        {
            get
            {
                return StationPickerStatic;
            }
        }


        /// <summary>
        /// Gets the Main property.
        /// </summary>
        public static MainViewModel MainStatic
        {
            get
            {
                if (_main == null)
                {
                    CreateMain();
                }

                return _main;
            }
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel Main
        {
            get
            {
                return MainStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the Main property.
        /// </summary>
        public static void ClearMain()
        {
            _main.Cleanup();
            _main = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the Main property.
        /// </summary>
        public static void CreateMain()
        {
            if (_main == null)
            {
                _main = new MainViewModel();
            }
        }

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
            ClearMain();
        }
    }
}