using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
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
using Treintijden.UWP.Views;

namespace Treintijden.UWP.ViewModel
{
  public class ViewModelLocator
  {
    //From Sample download:
    //https://onedrive.live.com/?id=40CFFDE85F1AB56A%212115&cid=40CFFDE85F1AB56A&group=0&parId=40CFFDE85F1AB56A%212103&action=locate

    public const string AboutPageKey = "AboutPage";

    static ViewModelLocator()
    {
      ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

      var nav = new NavigationService();
      nav.Configure(AboutPageKey, typeof(AboutPage));

      SimpleIoc.Default.Register<INavigationService>(() => nav);
      SimpleIoc.Default.Register<IDialogService, DialogService>();

      SimpleIoc.Default.Register<IStationService, StationService>();
      SimpleIoc.Default.Register<IStationNameService, StationNameService>();

      SimpleIoc.Default.Register<IPlannerService, PlannerService>();
      SimpleIoc.Default.Register<INSApiService>(() => new CachedNSApiService(new NSApiService(), SimpleIoc.Default.GetInstance<IPlannerService>()));

      SimpleIoc.Default.Register<ISettingService, SettingService>();

      //if (ViewModelBase.IsInDesignModeStatic)
      //{
      //  SimpleIoc.Default.Register<IDataService, Design.DesignDataService>();
      //}
      //else
      //{
      //  SimpleIoc.Default.Register<IDataService, DataService>();
      //}

      SimpleIoc.Default.Register<MainViewModel>();
      SimpleIoc.Default.Register<AboutViewModel>();
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        "CA1822:MarkMembersAsStatic",
        Justification = "This non-static member is needed for data binding purposes.")]
    public MainViewModel Main
    {
      get
      {
        return ServiceLocator.Current.GetInstance<MainViewModel>();
      }
    }

    public AboutViewModel About
    {
      get
      {
        return ServiceLocator.Current.GetInstance<AboutViewModel>();
      }
    }
  }
}
