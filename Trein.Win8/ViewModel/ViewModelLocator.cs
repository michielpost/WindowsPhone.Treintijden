using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treintijden.PCL.Api;
using Treintijden.PCL.Api.Interfaces;
using Treintijden.Shared.Services;
using Treintijden.Shared.Services.Interfaces;
using Treintijden.Shared.Services.WP;

namespace Trein.Win8.ViewModel
{
  /// <summary>
  /// This class contains static references to all the view models in the
  /// application and provides an entry point for the bindings.
  /// </summary>
  public class ViewModelLocator
  {
    /// <summary>
    /// Initializes a new instance of the ViewModelLocator class.
    /// </summary>
    public ViewModelLocator()
    {
      ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

      ////if (ViewModelBase.IsInDesignModeStatic)
      ////{
      ////    // Create design time view services and models
      ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
      ////}
      ////else
      ////{
      ////    // Create run time view services and models
      ////    SimpleIoc.Default.Register<IDataService, DataService>();
      ////}

      SimpleIoc.Default.Register<IPlannerService, PlannerService>();
      SimpleIoc.Default.Register<INSApiService>(() => new CachedNSApiService(new NSApiService(), SimpleIoc.Default.GetInstance<IPlannerService>()));


      SimpleIoc.Default.Register<IStationService, StationService>();
      SimpleIoc.Default.Register<IStationNameService, StationNameService>();
      SimpleIoc.Default.Register<ISettingService, SettingService>();



      SimpleIoc.Default.Register<MainViewModel>();
      //SimpleIoc.Default.Register<TravelAdviceSingleViewModel>();
      //SimpleIoc.Default.Register<DepartureTimesViewModel>();
      //SimpleIoc.Default.Register<TravelAdviceViewModel>();
      //SimpleIoc.Default.Register<StoringenViewModel>();
      //SimpleIoc.Default.Register<SearchResultsViewModel>();
      //SimpleIoc.Default.Register<MapViewModel>();
      //SimpleIoc.Default.Register<DonateViewModel>();
      //SimpleIoc.Default.Register<PlannerViewModel>();
      //SimpleIoc.Default.Register<GpsWatcherModel>();

    }

    public MainViewModel Main
    {
      get
      {
        return ServiceLocator.Current.GetInstance<MainViewModel>();
      }
    }

    public static void Cleanup()
    {
      // Clear the ViewModels
      SimpleIoc.Default.GetInstance<MainViewModel>().Cleanup();
      //SimpleIoc.Default.GetInstance<DepartureTimesViewModel>().Cleanup();
    }
  }
}