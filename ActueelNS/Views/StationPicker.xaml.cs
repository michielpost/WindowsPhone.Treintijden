using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Phone.Controls;
using ActueelNS.ViewModel;
using ActueelNS.Views.Base;
using Treintijden.PCL.Api.Models;

namespace ActueelNS.Views
{
    /// <summary>
    /// Description for StationPicker.
    /// </summary>
    public partial class StationPicker : ViewBase
    {

        private StationPickerViewModel _vm;


        /// <summary>
        /// Initializes a new instance of the StationPicker class.
        /// </summary>
        public StationPicker()
        {
            InitializeComponent();

            stations.SelectionChanged += new SelectionChangedEventHandler(stations_SelectionChanged);
            MainListBox.SelectionChanged += new SelectionChangedEventHandler(Main_SelectionChanged);

            this.Loaded += new RoutedEventHandler(StationPicker_Loaded);

        }

        void StationPicker_Loaded(object sender, RoutedEventArgs e)
        {
                StationAutoComplete.Focus();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            _vm = (StationPickerViewModel)DataContext;

            _vm.Load();

            
            stations.Visibility = System.Windows.Visibility.Collapsed;
            KeyboardPanel.Visibility = System.Windows.Visibility.Visible;
           

            base.OnNavigatedTo(e);
        }



        void stations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Station station = stations.SelectedItem as Station;

            if (station != null)
            {
                _vm.StationAddCommand.Execute(station);
            }
            
        }

        void Main_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Station station = MainListBox.SelectedItem as Station;

            if (station != null)
            {
                _vm.StationAddCommand.Execute(station);
            }

        }


        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _vm.SeachStation(StationAutoComplete.Text);
        }

        private void ExpandPickerImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            _vm.LoadForPicker();

            KeyboardPanel.Visibility = System.Windows.Visibility.Collapsed;
            stations.Visibility = System.Windows.Visibility.Visible;

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