using Microsoft.Phone.Controls;
using ActueelNS.Services.Models;
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
    public partial class Planner : PhoneApplicationPage
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
                currentSelector = null;
                _waitingForKeuze = PlannerKeuze.None;

                e.Cancel = true;
            }
            else
            {

                base.OnBackKeyPress(e);
            }
        }
        

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            _loading = true;

            _vm = (PlannerViewModel)DataContext;
            _vm.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_vm_PropertyChanged);

           

            bool keepValues = (e.NavigationMode == System.Windows.Navigation.NavigationMode.Back);

            string from = null;
            string to = null;
            string via = null;

            if (this.NavigationContext.QueryString.ContainsKey("from"))
                from = this.NavigationContext.QueryString["from"];
            if (this.NavigationContext.QueryString.ContainsKey("to"))
                to = this.NavigationContext.QueryString["to"];
            if (this.NavigationContext.QueryString.ContainsKey("via"))
                via = this.NavigationContext.QueryString["via"];

            _vm.InitValues(from, to, via, keepValues);

            

            _loading = false;
            SetPin();

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
                currentSelector = null;
                _waitingForKeuze = PlannerKeuze.None;
            }
        }
       

        private void DatePicker_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            if (e.NewDateTime.HasValue)
                _vm.Date = e.NewDateTime.Value;
            else
                _vm.Date = DateTime.Now;
        }

        private void TimePicker_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            if (e.NewDateTime.HasValue)
                _vm.Time = e.NewDateTime.Value;
            else
                _vm.Time = DateTime.Now;

        }

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
            _vm.StationList.Clear();
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


        #region LongList

        //private void LongListSelector_GroupViewOpened(object sender, GroupViewOpenedEventArgs e)
        //{
        //    //Hold a reference to the active long list selector.
        //    currentSelector = sender as LongListSelector;

        //    //Construct and begin a swivel animation to pop in the group view.
        //    IEasingFunction quadraticEase = new QuadraticEase { EasingMode = EasingMode.EaseOut };
        //    Storyboard _swivelShow = new Storyboard();
        //    ItemsControl groupItems = e.ItemsControl;

        //    foreach (var item in groupItems.Items)
        //    {
        //        UIElement container = groupItems.ItemContainerGenerator.ContainerFromItem(item) as UIElement;
        //        if (container != null)
        //        {
        //            Border content = VisualTreeHelper.GetChild(container, 0) as Border;
        //            if (content != null)
        //            {
        //                DoubleAnimationUsingKeyFrames showAnimation = new DoubleAnimationUsingKeyFrames();

        //                EasingDoubleKeyFrame showKeyFrame1 = new EasingDoubleKeyFrame();
        //                showKeyFrame1.KeyTime = TimeSpan.FromMilliseconds(0);
        //                showKeyFrame1.Value = -60;
        //                showKeyFrame1.EasingFunction = quadraticEase;

        //                EasingDoubleKeyFrame showKeyFrame2 = new EasingDoubleKeyFrame();
        //                showKeyFrame2.KeyTime = TimeSpan.FromMilliseconds(85);
        //                showKeyFrame2.Value = 0;
        //                showKeyFrame2.EasingFunction = quadraticEase;

        //                showAnimation.KeyFrames.Add(showKeyFrame1);
        //                showAnimation.KeyFrames.Add(showKeyFrame2);

        //                Storyboard.SetTargetProperty(showAnimation, new PropertyPath(PlaneProjection.RotationXProperty));
        //                Storyboard.SetTarget(showAnimation, content.Projection);

        //                _swivelShow.Children.Add(showAnimation);
        //            }
        //        }
        //    }

        //    _swivelShow.Begin();
        //}

        //private void LongListSelector_GroupViewClosing(object sender, GroupViewClosingEventArgs e)
        //{
        //    //Cancelling automatic closing and scrolling to do it manually.
        //    e.Cancel = true;
        //    if (e.SelectedGroup != null)
        //    {
        //        currentSelector.ScrollToGroup(e.SelectedGroup);
        //    }

        //    //Dispatch the swivel animation for performance on the UI thread.
        //    Dispatcher.BeginInvoke(() =>
        //    {
        //        //Construct and begin a swivel animation to pop out the group view.
        //        IEasingFunction quadraticEase = new QuadraticEase { EasingMode = EasingMode.EaseOut };
        //        Storyboard _swivelHide = new Storyboard();
        //        ItemsControl groupItems = e.ItemsControl;

        //        foreach (var item in groupItems.Items)
        //        {
        //            UIElement container = groupItems.ItemContainerGenerator.ContainerFromItem(item) as UIElement;
        //            if (container != null)
        //            {
        //                Border content = VisualTreeHelper.GetChild(container, 0) as Border;
        //                if (content != null)
        //                {
        //                    DoubleAnimationUsingKeyFrames showAnimation = new DoubleAnimationUsingKeyFrames();

        //                    EasingDoubleKeyFrame showKeyFrame1 = new EasingDoubleKeyFrame();
        //                    showKeyFrame1.KeyTime = TimeSpan.FromMilliseconds(0);
        //                    showKeyFrame1.Value = 0;
        //                    showKeyFrame1.EasingFunction = quadraticEase;

        //                    EasingDoubleKeyFrame showKeyFrame2 = new EasingDoubleKeyFrame();
        //                    showKeyFrame2.KeyTime = TimeSpan.FromMilliseconds(125);
        //                    showKeyFrame2.Value = 90;
        //                    showKeyFrame2.EasingFunction = quadraticEase;

        //                    showAnimation.KeyFrames.Add(showKeyFrame1);
        //                    showAnimation.KeyFrames.Add(showKeyFrame2);

        //                    Storyboard.SetTargetProperty(showAnimation, new PropertyPath(PlaneProjection.RotationXProperty));
        //                    Storyboard.SetTarget(showAnimation, content.Projection);

        //                    _swivelHide.Children.Add(showAnimation);
        //                }
        //            }
        //        }

        //        _swivelHide.Completed += _swivelHide_Completed;
        //        _swivelHide.Begin();

        //    });
        //}

        private void _swivelHide_Completed(object sender, EventArgs e)
        {
            //Close group view.
            if (currentSelector != null)
            {
                //currentSelector.CloseGroupView();
                currentSelector = null;
            }
        }

        #endregion

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

        
    }
}