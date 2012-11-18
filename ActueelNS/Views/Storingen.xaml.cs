using Microsoft.Phone.Controls;
using ActueelNS.ViewModel;
using ActueelNS.Services.Models;
using ActueelNS.Resources;
using Microsoft.Phone.Shell;
using GalaSoft.MvvmLight.Ioc;
using ActueelNS.Services.Interfaces;

namespace ActueelNS.Views
{
    /// <summary>
    /// Description for Storingen.
    /// </summary>
    public partial class Storingen : PhoneApplicationPage
    {

        private StoringenViewModel _vm;

        private ProgressIndicator _progressIndicator;

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

            _vm.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_vm_PropertyChanged);
            CheckBusy();
            CheckError();
            CheckShowGeenStoringen();

            if (SimpleIoc.Default.GetInstance<ILiveTileService>().ExistsCreateStoringen())
                (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;

            (ApplicationBar.MenuItems[0] as ApplicationBarMenuItem).Text = AppResources.StationTijdenMijnStationsButton;

            //if (this.NavigationContext.QueryString.ContainsKey("id"))
            //{
            //    string station = this.NavigationContext.QueryString["id"];
            //}

            base.OnNavigatedTo(e);
        }

        private void CheckError()
        {
            if (_vm.ShowError)
                GeenDataPanel.Visibility = System.Windows.Visibility.Visible;
            else
                GeenDataPanel.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void CheckBusy()
        {
            if (null == _progressIndicator)
            {
                _progressIndicator = new ProgressIndicator();
                _progressIndicator.IsVisible = true;
                SystemTray.ProgressIndicator = _progressIndicator;

            }

            if (_vm.IsBusy)
            {
                _progressIndicator.IsVisible = true;
                _progressIndicator.IsIndeterminate = true;

                _performanceProgressBar.IsEnabled = true;
                _performanceProgressBar.IsIndeterminate = true;
                LoaderStackPanel.Visibility = System.Windows.Visibility.Visible;


            }
            else
            {
                LoaderStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                _performanceProgressBar.IsIndeterminate = false;
                _performanceProgressBar.IsEnabled = false;

                _progressIndicator.IsIndeterminate = false;
                _progressIndicator.IsVisible = false;

                CheckShowGeenStoringen();


            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            _vm.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(_vm_PropertyChanged);

            base.OnNavigatedFrom(e);
        }

        void _vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == "CurrentStoringen")
            //{
            //    CheckShowGeenStoringen();
            //}

            if (e.PropertyName == "IsBusy")
            {
                CheckBusy();
            }
            else if (e.PropertyName == "ShowError")
            {
                CheckError();
            }

        }

        private void CheckShowGeenStoringen()
        {
            if (_vm.IsBusy || _vm.ShowError)
            {
                GeenStoringenPanel.Visibility = System.Windows.Visibility.Collapsed;
                return;
            }


            if (_vm.CurrentStoringen == null || _vm.CurrentStoringen.Count == 0)
            {
                GeenStoringenPanel.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                GeenStoringenPanel.Visibility = System.Windows.Visibility.Collapsed;

            }
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