using Microsoft.Phone.Controls;
using ActueelNS.ViewModel;
using ActueelNS.Resources;
using Microsoft.Phone.Shell;
using GalaSoft.MvvmLight.Ioc;
using ActueelNS.Services.Interfaces;
using ActueelNS.Views.Base;
using Treintijden.PCL.Api.Models;

namespace ActueelNS.Views
{
    /// <summary>
    /// Description for Storingen.
    /// </summary>
    public partial class Storingen : ViewBase
    {
        private StoringenViewModel _vm;

        /// <summary>
        /// Initializes a new instance of the Storingen class.
        /// </summary>
        public Storingen()
        {
            InitializeComponent();

            (ApplicationBar.Buttons[0] as ApplicationBarIconButton).Text = AppResources.vastpinnen;

            
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            _vm = (StoringenViewModel)DataContext;

           
            if (SimpleIoc.Default.GetInstance<ILiveTileService>().ExistsCreateStoringen())
                (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;

            (ApplicationBar.MenuItems[0] as ApplicationBarMenuItem).Text = AppResources.StationTijdenMijnStationsButton;

            //if (this.NavigationContext.QueryString.ContainsKey("id"))
            //{
            //    string station = this.NavigationContext.QueryString["id"];
            //}

            base.OnNavigatedTo(e);
        }

        private void WerkMainListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Werkzaamheden w = WerkMainListBox.SelectedItem as Werkzaamheden;

            if (w != null)
            {
                w.IsExpanded = !w.IsExpanded;
            }

            WerkMainListBox.SelectedItem = null;
        }

        private void PinButton_Click(object sender, System.EventArgs e)
        {
            _vm.PinCommand.Execute(null);
        }

        private void BackButton_Click(object sender, System.EventArgs e)
        {
            _vm.ToListCommand.Execute(null);
        }

    }
}