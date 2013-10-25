/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:ActueelNS.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using ActueelNS.Services.Interfaces;
using ActueelNS.Services;
using Treintijden.Shared.Services.Interfaces;
using Treintijden.PCL.Api.Interfaces;
using Treintijden.Shared.Services;
using Treintijden.Shared.Services.WP;
using Treintijden.PCL.Api;

namespace Trein.Win8.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// Use the <strong>mvvmlocatorproperty</strong> snippet to add ViewModels
    /// to this locator.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// </summary>
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {

            //if (ViewModelBase.IsInDesignModeStatic)
            //{
            //}
            //else
            //{
            //}
          SimpleIoc.Default.Register<IPlannerService, PlannerService>();
          SimpleIoc.Default.Register<INSApiService>(() => new CachedNSApiService(new NSApiService(), SimpleIoc.Default.GetInstance<IPlannerService>()));



            SimpleIoc.Default.Register<IStationService, StationService>();
            SimpleIoc.Default.Register<IPlannerService, PlannerService>();
            SimpleIoc.Default.Register<ILastStationService, LastStationService>();
            SimpleIoc.Default.Register<ISettingService, SettingService>();
            SimpleIoc.Default.Register<IMyTrajectService, MyTrajectService>();


            SimpleIoc.Default.Register<PlannerViewModel>();
            SimpleIoc.Default.Register<GpsWatcherModel>();

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<TravelAdviceSingleViewModel>();
            SimpleIoc.Default.Register<DepartureTimesViewModel>();
            SimpleIoc.Default.Register<TravelAdviceViewModel>();
            SimpleIoc.Default.Register<StoringenViewModel>();
            SimpleIoc.Default.Register<SearchResultsViewModel>();
            SimpleIoc.Default.Register<MapViewModel>();
            SimpleIoc.Default.Register<DonateViewModel>();
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
                return SimpleIoc.Default.GetInstance<MainViewModel>();
            }
        }


        /// <summary>
        /// Gets the Main property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public DepartureTimesViewModel DepartureTimes
        {
            get
            {
                return SimpleIoc.Default.GetInstance<DepartureTimesViewModel>();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
      "CA1822:MarkMembersAsStatic",
      Justification = "This non-static member is needed for data binding purposes.")]
        public GpsWatcherModel Gps
        {
            get
            {
                return SimpleIoc.Default.GetInstance<GpsWatcherModel>();
            }
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public TravelAdviceViewModel TravelAdvice
        {
            get
            {
                return SimpleIoc.Default.GetInstance<TravelAdviceViewModel>();
            }
        }


        /// <summary>
        /// Gets the StoringenViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public StoringenViewModel Storingen
        {
            get
            {
                return SimpleIoc.Default.GetInstance<StoringenViewModel>();
            }
        }

        /// <summary>
        /// Gets the StoringenViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public DonateViewModel Donate
        {
            get
            {
                return SimpleIoc.Default.GetInstance<DonateViewModel>();
            }
        }

        /// <summary>
        /// Gets the TravelAdviceSingle property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public TravelAdviceSingleViewModel TravelAdviceSingle
        {
            get
            {
                return SimpleIoc.Default.GetInstance<TravelAdviceSingleViewModel>();
            }
        }

        /// <summary>
        /// Gets the PlannerViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public PlannerViewModel Planner
        {
            get
            {
                return SimpleIoc.Default.GetInstance<PlannerViewModel>();
            }
        }

        /// <summary>
        /// Gets the SearchResults property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public SearchResultsViewModel SearchResults
        {
            get
            {
                return SimpleIoc.Default.GetInstance<SearchResultsViewModel>();
            }
        }


        /// <summary>
        /// Gets the Map property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MapViewModel Map
        {
            get
            {
                return SimpleIoc.Default.GetInstance<MapViewModel>();
            }
        }

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
            SimpleIoc.Default.GetInstance<MainViewModel>().Cleanup();
            SimpleIoc.Default.GetInstance<DepartureTimesViewModel>().Cleanup();
        }
    }
}