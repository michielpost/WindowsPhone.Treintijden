using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Phone.Controls;
using ActueelNS.Services.Models;
using ActueelNS.ViewModel;

namespace ActueelNS.Views
{
    /// <summary>
    /// Description for StationPicker.
    /// </summary>
    public partial class StationPicker : PhoneApplicationPage
    {

        private LongListSelector currentSelector;
        private StationPickerViewModel _vm;


        /// <summary>
        /// Initializes a new instance of the StationPicker class.
        /// </summary>
        public StationPicker()
        {
            InitializeComponent();

            stations.SelectionChanged += new SelectionChangedEventHandler(stations_SelectionChanged);

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

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            //Station station = StationAutoComplete.SelectedItem as Station;

            //if (station != null)
            //{
            //    _vm.StationAddCommand.Execute(station);
            //}

        }

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

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _vm.SeachStation(StationAutoComplete.Text);
        }

        private void ExpandPickerImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            _vm.LoadForPicker();

            KeyboardPanel.Visibility = System.Windows.Visibility.Collapsed;
            stations.Visibility = System.Windows.Visibility.Visible;


            //stations.DisplayGroupView();
        }

       

        
    }
}