using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Trein.Services.Interfaces
{
  public interface INavigationService
  {
    void Navigate(Type sourcePageType);
    
    void Navigate(Type sourcePageType, object parameter);

    bool CanGoBack();
    void GoBack();


    void NavigateToStationPicker();

    void NavigateToStationTijden(string id);

    void NavigateToStoringen();

    void NavigateToPlanner();

    void NavigateToAbout();

    void NavigateToSettings();

    void NavigateToDonate();

    void NavigateToMainPage();

    void NavigateToReisadvies(Guid id);

    void NavigateToStoringen(string name);

    void NavigateToRitInfoPage(string queryString);

    void NavigateToReisadvies();

    void NavigateToMapPage(string stationCode);

    void NavigateToPlanner(string from = null, string to = null);

    void NavigateToPrijs(Guid guid);
  }
}
