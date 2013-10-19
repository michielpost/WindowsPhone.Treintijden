using Microsoft.Phone.Controls;
using ActueelNS.ViewModel;
using Microsoft.Phone.Shell;
using System.Windows.Controls;
using System.Windows.Media;
using ActueelNS.Resources;
using ActueelNS.Views.Base;
using Treintijden.PCL.Api.Models;

namespace ActueelNS.Views
{
    /// <summary>
    /// Description for StationTijden.
    /// </summary>
    public partial class StationTijden : ViewBase
    {
        private StationTijdenViewModel _vm;

        /// <summary>
        /// Initializes a new instance of the StationTijden class.
        /// </summary>
        public StationTijden()
        {
            InitializeComponent();

            (ApplicationBar.Buttons[0] as ApplicationBarIconButton).Text = AppResources.update;
            (ApplicationBar.Buttons[1] as ApplicationBarIconButton).Text = AppResources.favoriet;
            (ApplicationBar.Buttons[2] as ApplicationBarIconButton).Text = AppResources.vastpinnen;
            (ApplicationBar.Buttons[3] as ApplicationBarIconButton).Text = AppResources.planner;

            (ApplicationBar.MenuItems[0] as ApplicationBarMenuItem).Text = AppResources.StationTijdenPlanReisButton;
            (ApplicationBar.MenuItems[1] as ApplicationBarMenuItem).Text = AppResources.StationTijdenDeleteButton;
            (ApplicationBar.MenuItems[2] as ApplicationBarMenuItem).Text = AppResources.StationTijdenMapButton;
            (ApplicationBar.MenuItems[3] as ApplicationBarMenuItem).Text = AppResources.StationTijdenMijnStationsButton;
           

        }

        protected override async void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            _vm = (StationTijdenViewModel)DataContext;
            _vm.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_vm_PropertyChanged);
            
            
            bool keepValues = (e.NavigationMode == System.Windows.Navigation.NavigationMode.Back);

            MainListBox.SelectedItem = null;
            

            if (this.NavigationContext.QueryString.ContainsKey("id"))
            {
                string station = this.NavigationContext.QueryString["id"];

                if (!keepValues || _vm.CurrentStation == null)
                {
                    await _vm.LoadStation(station);
                }

                (ApplicationBar.Buttons[2] as ApplicationBarIconButton).IsEnabled = !_vm.IsPinned;
            }
           
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            _vm.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(_vm_PropertyChanged);

            base.OnNavigatedFrom(e);
        }

        void _vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "StoringenVisible")
            {
                if (_vm.StoringenVisible)
                    StoringenVisible.Begin();
            }
            else if (e.PropertyName == "InMyStations")
            {
                if (_vm.InMyStations)
                {
                    (ApplicationBar.MenuItems[1] as ApplicationBarMenuItem).Text = AppResources.StationTijdenDeleteButton;
                    (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = false;
                }
                else
                {
                    (ApplicationBar.MenuItems[1] as ApplicationBarMenuItem).Text = AppResources.StationTijdenAddButton;
                }

            }
        }


        private void RefreshButton_Click(object sender, System.EventArgs e)
        {
            _vm.RefreshCommand.Execute(null);
        }

        private void MapButton_Click(object sender, System.EventArgs e)
        {
            _vm.MapCommand.Execute(null);
        }

        private void DeleteButton_Click(object sender, System.EventArgs e)
        {
            if (_vm.InMyStations)
                _vm.DeleteCommand.Execute(null);
            else
                _vm.AddCommand.Execute(null);

        }

        private void PinButton_Click(object sender, System.EventArgs e)
        {
            _vm.PinCommand.Execute(null);

        }

        private void BackButton_Click(object sender, System.EventArgs e)
        {
            _vm.ToListCommand.Execute(null);
        }

        private void PlanButton_Click(object sender, System.EventArgs e)
        {
            _vm.PlanCommand.Execute(null);
        }

        private void CloseGeenDataPanel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                GeenDataPanel.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void FavButton_Click(object sender, System.EventArgs e)
        {
            if (_vm.InMyStations)
                _vm.DeleteCommand.Execute(null);
            else
                _vm.AddCommand.Execute(null);
        }

        private void MainListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Vertrektijd selected = MainListBox.SelectedItem as Vertrektijd;

            if (selected != null)
            {
                _vm.RitInfoCommand.Execute(selected);
            }
        }

    }
}