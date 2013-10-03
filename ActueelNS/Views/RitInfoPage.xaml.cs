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
        RitInfoViewModel _vm;

        public RitInfoPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            _vm = (RitInfoViewModel)DataContext;
            _vm.RitInfoAvailable += _vm_RitInfoAvailable;

            if (this.NavigationContext.QueryString.ContainsKey("id"))
            {
                string station = this.NavigationContext.QueryString["id"];
                string company = this.NavigationContext.QueryString["company"];
                string trein = this.NavigationContext.QueryString["trein"];
                string richting = this.NavigationContext.QueryString["richting"];
                string stationCode = this.NavigationContext.QueryString["stationCode"];

                _vm.Initialize(station, company, trein, richting, stationCode);

            }

        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            _vm.RitInfoAvailable -= _vm_RitInfoAvailable;

            base.OnNavigatedFrom(e);
        }

        void _vm_RitInfoAvailable(object sender, EventArgs e)
        {
            //Scroll to current station
            var currentItem = _vm.RitStops.Where(x => x.IsCurrent).FirstOrDefault();
            if (currentItem != null)
                MainListBox.ScrollTo(currentItem);
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