using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ActueelNS.Services.Interfaces;
using GalaSoft.MvvmLight.Ioc;
using Treintijden.PCL.Api.Models.Realtime;

namespace ActueelNS.UserControls
{
  public partial class TreinMapControl : UserControl
  {
    public Trein Trein { get; set; }

    public TreinMapControl(Trein trein)
    {
      InitializeComponent();

      this.Trein = trein;
      TypeTextBox.Text = trein.Type + " " + trein.TreinNummer;
    }

    private void LayoutRoot_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      var navigationService = SimpleIoc.Default.GetInstance<INavigationService>();
      string trein = string.Format("{0} ({1})", Trein.Type, Trein.TreinNummer);
      navigationService.NavigateTo(new Uri(string.Format("/Views/RitInfoPage.xaml?id={0}&company={1}&trein={2}&richting={3}&stationCode={4}", Trein.TreinNummer, "NS", trein, string.Empty, string.Empty), UriKind.Relative));

    }
  }
}
