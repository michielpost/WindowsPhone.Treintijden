using Microsoft.Phone.Controls;
using ActueelNS.ViewModel;
using System.Windows;
using ActueelNS.Resources;
using ActueelNS.Views.Base;

namespace ActueelNS.Views
{
    /// <summary>
    /// Description for Settings.
    /// </summary>
    public partial class Settings : ViewBase
    {
        private SettingsViewModel _vm;


        /// <summary>
        /// Initializes a new instance of the Settings class.
        /// </summary>
        public Settings()
        {
            InitializeComponent();

        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            _vm = (SettingsViewModel)DataContext;

            _vm.Initialize();

            SetSwitches(true);

            if (_vm.Settings.Culture == "en-US")
                LanguageInputPicker.SelectedIndex = 1;
            else
                LanguageInputPicker.SelectedIndex = 0;

            if (_vm.Settings.GpsListCount.HasValue)
                GpsNumberInputPicker.SelectedItem = _vm.Settings.GpsListCount.Value;
           

        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            _vm.SaveCommand.Execute(null);

            base.OnNavigatedFrom(e);
        }
       

        private void UseGpsSwitch_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            SetSwitches(false);
        }

        private void SetSwitches(bool first)
        {
            if (first)
            {
                if (!_vm.Settings.AllowGps)
                {
                    AutoFillSwitch.Visibility = System.Windows.Visibility.Collapsed;
                    ShowListSwitch.Visibility = System.Windows.Visibility.Collapsed;

                }
                else
                {
                    AutoFillSwitch.Visibility = System.Windows.Visibility.Visible;
                    ShowListSwitch.Visibility = System.Windows.Visibility.Visible;
                }
            }
            else
            {
                if (UseGpsSwitch.IsChecked.HasValue && !UseGpsSwitch.IsChecked.Value)
                {
                    AutoFillSwitch.Visibility = System.Windows.Visibility.Collapsed;
                    ShowListSwitch.Visibility = System.Windows.Visibility.Collapsed;

                }
                else
                {
                    AutoFillSwitch.Visibility = System.Windows.Visibility.Visible;
                    ShowListSwitch.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }

       
        private void LanguageInputPicker_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (_vm != null && _vm.Settings != null && e.AddedItems.Count > 0)
            {
                bool notify = false;
                if ((string)e.AddedItems[0] == "English" && _vm.Settings.Culture != "en-US")
                {
                    _vm.Settings.Culture = "en-US";
                    notify = true;
                }
                else if ((string)e.AddedItems[0] == "Nederlands" && _vm.Settings.Culture != "nl-NL")
                {
                    _vm.Settings.Culture = "nl-NL";
                    notify = true;
                }

                if(notify)
                    MessageBox.Show("Please restart the application to apply the language changes.");
            }
        }

        private void GpsNumberInputPicker_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (_vm != null && _vm.Settings != null && e.AddedItems.Count > 0)
            {
                int value = (int)e.AddedItems[0];

                if (value > 0)
                    _vm.Settings.ShowList = true;
                else
                    _vm.Settings.ShowList = false;


                _vm.Settings.GpsListCount = value;


            }
        }

       
    }
}