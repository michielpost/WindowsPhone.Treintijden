using Microsoft.Phone.Controls;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows;
using System.Windows.Media;
using System;
using ActueelNS.ViewModel;
using Microsoft.Phone.Shell;
using System.Threading.Tasks;
using ActueelNS.Resources;
using ActueelNS.Services.Interfaces;
using GalaSoft.MvvmLight.Ioc;
using System.Collections;
using ActueelNS.Views.Base;
using Treintijden.PCL.Api.Models;
using System.Globalization;

namespace ActueelNS.Views
{

    public enum PlannerKeuze
    {
         None,
        Van,
        Naar,
        Via
    }

    /// <summary>
    /// Description for Planner.
    /// </summary>
    public partial class Planner : ViewBase
    {
        private LongListSelector currentSelector;
        private PlannerKeuze _waitingForKeuze;

        private PlannerViewModel _vm;

        private bool _loading = false;


        /// <summary>
        /// Initializes a new instance of the Planner class.
        /// </summary>
        public Planner()
        {
            InitializeComponent();

            stations.SelectionChanged += new SelectionChangedEventHandler(stations_SelectionChanged);

            (ApplicationBar.Buttons[0] as ApplicationBarIconButton).Text = AppResources.historie;
            (ApplicationBar.Buttons[1] as ApplicationBarIconButton).Text = AppResources.vastpinnen;
            (ApplicationBar.Buttons[2] as ApplicationBarIconButton).Text = AppResources.terugreis;

            (ApplicationBar.MenuItems[0] as ApplicationBarMenuItem).Text = AppResources.PlannerMijnStationsButton;

            ListPicker.Items.Add(AppResources.PlannerVertrekOption);
            ListPicker.Items.Add(AppResources.PlannerAankomstOption);


        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (currentSelector != null)
            {
                base.OnBackKeyPress(e);
            }
            else if (StationSelectorGrid.Visibility == System.Windows.Visibility.Visible
                || KeyboardPanel.Visibility == System.Windows.Visibility.Visible)
            {
                KeyboardPanel.Visibility = System.Windows.Visibility.Collapsed;
                StationSelectorGrid.Visibility = System.Windows.Visibility.Collapsed;
                PlannerPanel.Visibility = System.Windows.Visibility.Visible;
                ApplicationBar.IsVisible = true;
                currentSelector = null;
                _waitingForKeuze = PlannerKeuze.None;

                e.Cancel = true;
            }
            else
            {

                base.OnBackKeyPress(e);
            }
        }
        

        protected override async void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            _loading = true;

            _vm = (PlannerViewModel)DataContext;
            _vm.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_vm_PropertyChanged);

           

            bool keepValues = (e.NavigationMode == System.Windows.Navigation.NavigationMode.Back);

            string from = null;
            string to = null;
            string via = null;
            DateTime? dateTime = null;

            if (this.NavigationContext.QueryString.ContainsKey("from"))
                from = this.NavigationContext.QueryString["from"];
            if (this.NavigationContext.QueryString.ContainsKey("to"))
                to = this.NavigationContext.QueryString["to"];
            if (this.NavigationContext.QueryString.ContainsKey("via"))
                via = this.NavigationContext.QueryString["via"];
            if (this.NavigationContext.QueryString.ContainsKey("dateTime"))
            {
              string d = this.NavigationContext.QueryString["dateTime"];
              DateTime tryDate;
              if(DateTime.TryParse(d,CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out tryDate))
              {
                dateTime = DateTime.Parse(d, CultureInfo.InvariantCulture);
              }
            }

            _vm.InitValues(from, to, via, keepValues, dateTime);

            

            _loading = false;
            SetPin();

            try
            {
                ViewModelLocator.MainStatic.Update();
            }
            catch { }

            base.OnNavigatedTo(e);
        }

        void _vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "VanStation" || e.PropertyName == "NaarStation")
            {
                if (_vm.NaarStation != null || _vm.VanStation != null)
                {
                    (ApplicationBar.Buttons[2] as ApplicationBarIconButton).IsEnabled = true;
                }
                else
                {
                    (ApplicationBar.Buttons[2] as ApplicationBarIconButton).IsEnabled = false;

                }

            }

            SetPin();

        }

        private async void SetPin()
        {
            if (!_loading)
            {
                try
                {
                    bool setPin = await Task.Run(() =>
                    {
                        return (_vm.CanPin());
                    });

                    if (setPin)
                    {
                        (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = true;

                    }
                    else
                    {
                        (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = false;

                    }
                }
                catch { }
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            _vm.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(_vm_PropertyChanged);

            base.OnNavigatedFrom(e);
        }

        void stations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Station station = stations.SelectedItem as Station;

            SelectStation(station);


        }

        private void SelectStation(Station station)
        {
            if (station != null)
            {
                //Add station
                switch (_waitingForKeuze)
                {
                    case PlannerKeuze.None:
                        break;
                    case PlannerKeuze.Van:
                        _vm.VanStation = station;
                        break;
                    case PlannerKeuze.Naar:
                        _vm.NaarStation = station;

                        break;
                    case PlannerKeuze.Via:
                        _vm.ViaStation = station;
                        break;
                    default:
                        break;
                }

                StationSelectorGrid.Visibility = System.Windows.Visibility.Collapsed;
                KeyboardPanel.Visibility = System.Windows.Visibility.Collapsed;
                PlannerPanel.Visibility = System.Windows.Visibility.Visible;
                ApplicationBar.IsVisible = true;
                currentSelector = null;
                _waitingForKeuze = PlannerKeuze.None;
            }
        }
       

        //private void DatePicker_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        //{
        //    if (e.NewDateTime.HasValue)
        //        _vm.Date = e.NewDateTime.Value;
        //    else
        //        _vm.Date = DateTime.Now;
        //}

        //private void TimePicker_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        //{
        //    if (e.NewDateTime.HasValue)
        //        _vm.Time = e.NewDateTime.Value;
        //    else
        //        _vm.Time = DateTime.Now;

        //}

        private void VanTextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            _waitingForKeuze = PlannerKeuze.Van;

            MakePickerVisible();
            
        }

      
        private void NaarTextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            _waitingForKeuze = PlannerKeuze.Naar;

            MakePickerVisible();

        }

        private void ViaTextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            _waitingForKeuze = PlannerKeuze.Via;

            MakePickerVisible();

        }

        private void MakePickerVisible()
        {
            _vm.InitForNewPick();
            StationAutoComplete.Text = string.Empty;

            switch (_waitingForKeuze)
                {
                    case PlannerKeuze.None:
                        break;
                    case PlannerKeuze.Van:
                        CurrentSelectionMode.Text = AppResources.PlannerVertrek;
                        CurrentSelectionMode2.Text = AppResources.PlannerVertrek;
                        break;
                    case PlannerKeuze.Naar:
                        CurrentSelectionMode.Text = AppResources.PlannerAankomst;
                        CurrentSelectionMode2.Text = AppResources.PlannerAankomst;

                        break;
                    case PlannerKeuze.Via:
                        CurrentSelectionMode.Text = AppResources.PlannerVia;
                        CurrentSelectionMode2.Text = AppResources.PlannerVia;

                        break;
                    default:
                        break;
                }

            

            StationAutoComplete.Focus();

            KeyboardPanel.Visibility = Visibility.Visible;
            PlannerPanel.Visibility = System.Windows.Visibility.Collapsed;
            ApplicationBar.IsVisible = false;

           
        }


        private void ListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_vm != null && e.AddedItems.Count > 0)
            {
                if ((string)e.AddedItems[0] == AppResources.PlannerAankomstOption)
                {
                    _vm.Type = "aankomst";
                }
                else
                    _vm.Type = "vertrek";
            }
        }

        private void SearchHistory_Click(object sender, EventArgs e)
        {
            _vm.SearchHistoryCommand.Execute(null);
        }

      

        private void PinButton_Click(object sender, System.EventArgs e)
        {
            _vm.PinCommand.Execute(null);
        }

        private void MijnStationsButton_Click(object sender, EventArgs e)
        {
            _vm.MijnStationsCommand.Execute(null);
        }


        private void AutoCompleteBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var control = (AutoCompleteBox)sender;

            if (control.SelectedItem != null)
            {
                Station s = control.SelectedItem as Station;
                if (s != null)
                {
                    control.Text = s.Name;
                }
            }
            else
            {
                control.Text = null;
            }

        }

        private void SwitchButton_Click(object sender, EventArgs e)
        {
            _vm.SwitchCommand.Execute(null);

           
        }

        private void ExpandPickerImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            _vm.LoadForPicker();

            KeyboardPanel.Visibility = System.Windows.Visibility.Collapsed;
            StationSelectorGrid.Visibility = System.Windows.Visibility.Visible;


            //stations.DisplayGroupView();
        }

        private void StationAutoComplete_TextChanged(object sender, TextChangedEventArgs e)
        {
            _vm.SeachStation(StationAutoComplete.Text);
        }

        private void MainListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Station station = MainListBox.SelectedItem as Station;

            SelectStation(station);

            MainListBox.SelectedItem = null;
        }

        private void StationAutoComplete_LostFocus(object sender, RoutedEventArgs e)
        {
            KeyboardCover.Visibility = System.Windows.Visibility.Collapsed;
            //KeyboardCover.Height = 0;

        }

        private void StationAutoComplete_GotFocus(object sender, RoutedEventArgs e)
        {
            KeyboardCover.Visibility = System.Windows.Visibility.Visible;
            //KeyboardCover.Height = 340;
        }

        
    }
}