using Microsoft.Phone.Controls;
using ActueelNS.ViewModel;
using Microsoft.Phone.Shell;
using System;
using ActueelNS.Views.Base;

namespace ActueelNS.Views
{
    /// <summary>
    /// Description for Prijs.
    /// </summary>
    public partial class Prijs : ViewBase
    {

        private ProgressIndicator _progressIndicator;

        private PrijsViewModel _vm;

        /// <summary>
        /// Initializes a new instance of the Prijs class.
        /// </summary>
        public Prijs()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            _vm = (PrijsViewModel)DataContext;
            _vm.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_vm_PropertyChanged);

            PrijsPanel.Visibility = System.Windows.Visibility.Collapsed;
           

                if (this.NavigationContext.QueryString.ContainsKey("id"))
                {
                    string guidIdString = this.NavigationContext.QueryString["id"];

                    Guid id = Guid.Parse(guidIdString);


                    _vm.Initialize(id);

                }
                else
                {
                    _vm.Initialize(null);

                }

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            _vm.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(_vm_PropertyChanged);

            base.OnNavigatedFrom(e);
        }

        void _vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsBusy")
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

                }

            }
            else if (e.PropertyName == "ShowError")
            {
                if (_vm.ShowError)
                    GeenDataPanel.Visibility = System.Windows.Visibility.Visible;
                else
                    GeenDataPanel.Visibility = System.Windows.Visibility.Collapsed;

            }
            else if (e.PropertyName == "ReisPrijs")
            {
                if (_vm.ReisPrijs != null)
                    PrijsPanel.Visibility = System.Windows.Visibility.Visible;
                else
                    PrijsPanel.Visibility = System.Windows.Visibility.Collapsed;

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