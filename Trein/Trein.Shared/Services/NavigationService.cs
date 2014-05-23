using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Trein.Services.Interfaces
{
  public class NavigationService : INavigationService
  {

    //TODO:
    //http://www.dzone.com/articles/navigationservice-winrt-0

    public void Navigate(Type sourcePageType)
    {
      ((Frame)Window.Current.Content).Navigate(sourcePageType);
    }

    public void Navigate(Type sourcePageType, object parameter)
    {
      ((Frame)Window.Current.Content).Navigate(sourcePageType, parameter);

    }

    public void GoBack()
    {
      ((Frame)Window.Current.Content).GoBack();
    }

    public bool CanGoBack()
    {
      return ((Frame)Window.Current.Content).CanGoBack;
    }


    public void NavigateToStationPicker()
    {
      throw new NotImplementedException();
    }

    public void NavigateToStationTijden(string id)
    {
      //TODO: Checken naam of ID
      throw new NotImplementedException();
    }

    public void NavigateToStoringen()
    {
      throw new NotImplementedException();
    }

    public void NavigateToPlanner()
    {
      throw new NotImplementedException();
    }

    public void NavigateToAbout()
    {
      throw new NotImplementedException();
    }

    public void NavigateToSettings()
    {
      throw new NotImplementedException();
    }

    public void NavigateToDonate()
    {
      throw new NotImplementedException();
    }

    public void NavigateToMainPage()
    {
      throw new NotImplementedException();
    }


    public void NavigateToReisadvies(Guid id)
    {
      throw new NotImplementedException();
    }


    public void NavigateToStoringen(string name)
    {
      throw new NotImplementedException();
    }

    public void NavigateToRitInfoPage(string queryString)
    {
      //TODO: Querystring typed maken
      throw new NotImplementedException();
    }


    public void NavigateToReisadvies()
    {
      throw new NotImplementedException();
    }

    public void NavigateToMapPage(string stationCode)
    {
      throw new NotImplementedException();
    }


    public void NavigateToPlanner(string from = null, string to = null)
    {
      throw new NotImplementedException();
    }


    public void NavigateToPrijs(Guid guid)
    {
      throw new NotImplementedException();
    }
  }
}
