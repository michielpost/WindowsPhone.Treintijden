using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ActueelNS.Views.Base;
using ActueelNS.ViewModel;

namespace ActueelNS.Views
{
    public partial class RitInfoPage : ViewBase
    {
        public RitInfoPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var vm = (RitInfoViewModel)DataContext;

            if (this.NavigationContext.QueryString.ContainsKey("id"))
            {
                string station = this.NavigationContext.QueryString["id"];
                string company = this.NavigationContext.QueryString["company"];
                string trein = this.NavigationContext.QueryString["trein"];
                string richting = this.NavigationContext.QueryString["richting"];

                vm.Initialize(station, company, trein, richting);

            }

        }

        private void CloseGeenDataPanel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                GeenDataPanel.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}