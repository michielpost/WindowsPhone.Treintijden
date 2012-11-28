﻿using Microsoft.Phone.Controls;
using ActueelNS.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Phone.Shell;
using ActueelNS.Resources;
using ActueelNS.Services.Models;
using ActueelNS.Views.Base;

namespace ActueelNS.Views
{
    /// <summary>
    /// Description for Reisadvies.
    /// </summary>
    public partial class Reisadvies : ViewBase
    {
        private ProgressIndicator _progressIndicator;

        private ReisadviesViewModel _vm;

        private const  int HistoryIndex = 0;
        private const  int VertrektijdenIndex = 1;
        private const int PrijsIndex = 2;
        private const int PinIndex = 3;


        /// <summary>
        /// Initializes a new instance of the Reisadvies class.
        /// </summary>
        public Reisadvies()
        {
            InitializeComponent();

            ShowStep2Header.Completed += new EventHandler(ShowStep2Header_Completed);

            (ApplicationBar.Buttons[HistoryIndex] as ApplicationBarIconButton).Text = AppResources.historie;
            (ApplicationBar.Buttons[VertrektijdenIndex] as ApplicationBarIconButton).Text = AppResources.actueel;
            (ApplicationBar.Buttons[PrijsIndex] as ApplicationBarIconButton).Text = AppResources.prijs;
            (ApplicationBar.Buttons[PinIndex] as ApplicationBarIconButton).Text = AppResources.vastpinnen;

            (ApplicationBar.MenuItems[0] as ApplicationBarMenuItem).Text = AppResources.ReisadviesTerugreisButton;
            (ApplicationBar.MenuItems[1] as ApplicationBarMenuItem).Text = AppResources.ReisadviesRepeatButton;
            (ApplicationBar.MenuItems[2] as ApplicationBarMenuItem).Text = AppResources.ReisadviesRemindMe;
            (ApplicationBar.MenuItems[3] as ApplicationBarMenuItem).Text = AppResources.ReisadviesDeleteHistory;
            (ApplicationBar.MenuItems[4] as ApplicationBarMenuItem).Text = AppResources.ReisadviesMijnStationsButton;
            (ApplicationBar.MenuItems[5] as ApplicationBarMenuItem).Text = AppResources.ReisadviesTapShareButton;

        }

        void ShowStep2Header_Completed(object sender, EventArgs e)
        {
            _vm.MainReisMogelijkheid = _vm.SelectedReisMogelijkheid;
        }


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            _vm = (ReisadviesViewModel)DataContext;
            _vm.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_vm_PropertyChanged);


            bool keepValues = (e.NavigationMode == System.Windows.Navigation.NavigationMode.Back);

            if (!keepValues || !_vm.IsInit)
            {
                Step1InfoPanel.Visibility = System.Windows.Visibility.Collapsed;
                Step2InfoPanel.Visibility = System.Windows.Visibility.Collapsed;
                MogelijkhedenPanel.Visibility = System.Windows.Visibility.Collapsed;
                AdviesContentPanel.Visibility = System.Windows.Visibility.Collapsed;

                if (this.NavigationContext.QueryString.ContainsKey("id"))
                {
                    string guidIdString = this.NavigationContext.QueryString["id"];

                    Guid id = Guid.Parse(guidIdString);

                    int? index = null;
                    if (this.NavigationContext.QueryString.ContainsKey("index"))
                    {
                        string indexString = this.NavigationContext.QueryString["index"];

                        index = Int32.Parse(indexString);
                    }

                    SearchHistoryListBox.Visibility = System.Windows.Visibility.Collapsed;
                    _vm.Initialize(id, index);

                }
                else
                {
                    _vm.Initialize(null, null);

                }
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
            else if (e.PropertyName == "SelectedSearch")
            {
                if (_vm.SelectedSearch == null)
                {
                    //Show all search history
                    if(SearchHistoryListBox.Visibility == System.Windows.Visibility.Collapsed)
                        ShowStep0List.Begin();

                    if (Step1InfoPanel.Visibility == System.Windows.Visibility.Visible)
                    {
                        HideStep1Header.Begin();


                        (ApplicationBar.Buttons[VertrektijdenIndex] as ApplicationBarIconButton).IsEnabled = false;
                        (ApplicationBar.Buttons[PrijsIndex] as ApplicationBarIconButton).IsEnabled = false;
                       // (ApplicationBar.Buttons[3] as ApplicationBarIconButton).IsEnabled = false;
                        (ApplicationBar.MenuItems[0] as ApplicationBarMenuItem).IsEnabled = false;
                        (ApplicationBar.MenuItems[1] as ApplicationBarMenuItem).IsEnabled = false;
                        (ApplicationBar.MenuItems[2] as ApplicationBarMenuItem).IsEnabled = false;
                        (ApplicationBar.MenuItems[5] as ApplicationBarMenuItem).IsEnabled = false;
                    }

                    if (MogelijkhedenPanel.Visibility == System.Windows.Visibility.Visible)
                        HideStep1List.Begin();
                }
                else
                {
                    //if (SearchHistoryListBox.Visibility == System.Windows.Visibility.Visible)
                        HideStep0List.Begin();


                        if (Step1InfoPanel.Visibility == System.Windows.Visibility.Collapsed)
                        {
                            ShowStep1Header.Begin();

                            (ApplicationBar.Buttons[VertrektijdenIndex] as ApplicationBarIconButton).IsEnabled = true;
                            (ApplicationBar.Buttons[PrijsIndex] as ApplicationBarIconButton).IsEnabled = true;
                            //(ApplicationBar.Buttons[3] as ApplicationBarIconButton).IsEnabled = true;
                            (ApplicationBar.MenuItems[0] as ApplicationBarMenuItem).IsEnabled = true;
                            (ApplicationBar.MenuItems[1] as ApplicationBarMenuItem).IsEnabled = true;
                            (ApplicationBar.MenuItems[5] as ApplicationBarMenuItem).IsEnabled = true;

                        }
                   
                }
            }
            else if (e.PropertyName == "SelectedReisMogelijkheid")
            {
                if (_vm.SelectedReisMogelijkheid == null)
                {
                    VertragingPanel.Visibility = System.Windows.Visibility.Collapsed;

                    if (Step2InfoPanel.Visibility == System.Windows.Visibility.Visible)
                    {
                        HideStep2Header.Begin();
                        HideButtons.Begin();
                        HideFinalList.Begin();

                    }

                    //Show all mogelijkheden
                    if (_vm.SelectedSearch != null)
                    {
                        if (MogelijkhedenPanel.Visibility == System.Windows.Visibility.Collapsed)
                            ShowStep1List.Begin();
                       
                    }
                    else
                    {
                        if (MogelijkhedenPanel.Visibility == System.Windows.Visibility.Visible)
                            HideStep1List.Begin();
                    }

                    (ApplicationBar.MenuItems[2] as ApplicationBarMenuItem).IsEnabled = false; //Reminder

                }
                else
                {
                    if (MogelijkhedenPanel.Visibility == System.Windows.Visibility.Visible)
                        HideStep1List.Begin();

                    if (Step2InfoPanel.Visibility == System.Windows.Visibility.Collapsed)
                    {
                        ShowStep2Header.Begin();
                        ShowButtons.Begin();
                        ShowFinalList.Begin();
                    }

                    (ApplicationBar.MenuItems[2] as ApplicationBarMenuItem).IsEnabled = true; //Reminder

                    if (!string.IsNullOrEmpty(_vm.SelectedReisMogelijkheid.VertrekVertraging)
                        || !string.IsNullOrEmpty(_vm.SelectedReisMogelijkheid.AankomstVertraging))
                    {
                        VertragingPanel.Visibility = System.Windows.Visibility.Visible;

                        if (!string.IsNullOrEmpty(_vm.SelectedReisMogelijkheid.VertrekVertraging)
                        && !string.IsNullOrEmpty(_vm.SelectedReisMogelijkheid.AankomstVertraging))
                        {
                            //Allebei
                            VertragingTextBlock.Text = AppResources.ReisadviesVertraging;

                        }
                        else if (!string.IsNullOrEmpty(_vm.SelectedReisMogelijkheid.VertrekVertraging))
                        {
                            //Vertrek
                            VertragingTextBlock.Text = AppResources.ReisadviesVertragingVertrek;

                        }
                        else if (!string.IsNullOrEmpty(_vm.SelectedReisMogelijkheid.AankomstVertraging))
                        {
                            //Aankomst
                            VertragingTextBlock.Text = AppResources.ReisadviesVertragingAankomst;
                        }
                    }
                    else
                    {
                        VertragingPanel.Visibility = System.Windows.Visibility.Collapsed;
                    }

                }

                if (_vm.CanPin())
                {
                    //vastpinnen
                    (ApplicationBar.Buttons[PinIndex] as ApplicationBarIconButton).IsEnabled = true;

                }
                else
                {
                    //vastpinnen
                    (ApplicationBar.Buttons[PinIndex] as ApplicationBarIconButton).IsEnabled = false;

                }

            }
            else if (e.PropertyName == "ShowError")
            {
                if (_vm.ShowError)
                    GeenDataPanel.Visibility = System.Windows.Visibility.Visible;
                else
                    GeenDataPanel.Visibility = System.Windows.Visibility.Collapsed;

            }
            else if (e.PropertyName == "SearchHistory")
            {
                if (_vm.SearchHistory == null || _vm.SearchHistory.Count == 0)
                {
                    GeenHistoryPanel.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    GeenHistoryPanel.Visibility = System.Windows.Visibility.Collapsed;

                }
            }
        }

        private void Step1InfoPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (_vm.SelectedReisMogelijkheid == null)
            {
                _vm.SelectedSearch = null;
                _vm.SelectedReisMogelijkheid = null;
            }
            else
            {
                _vm.SelectedReisMogelijkheid = null;
            }
        }

        private void Step2InfoPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            _vm.SelectedReisMogelijkheid = null;

        }

        private void PageTitle_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            _vm.SelectedSearch = null;
            _vm.SelectedReisMogelijkheid = null;
        }

       

        private void TerugReisAppBarButton_Click(object sender, EventArgs e)
        {
            _vm.TerugreisCommand.Execute(null);
        }

        private void DeleteHistoryButton_Click(object sender, EventArgs e)
        {
            _vm.DeleteHistoryCommand.Execute(null);
        }

        private void VertrektijdenAppBarButton_Click(object sender, EventArgs e)
        {
            _vm.VertrektijdenCommand.Execute(null);

        }

        private void OnFlick(object sender, FlickGestureEventArgs e)
        {
            double hAb = Math.Abs(e.HorizontalVelocity);
            double hVer = Math.Abs(e.VerticalVelocity);

            if (hAb > hVer && e.HorizontalVelocity < 500)
                _vm.LaterCommand.Execute(null);
            else if (hAb > hVer && e.HorizontalVelocity > 500)
                _vm.EerderCommand.Execute(null);
        }

        private void RepeatSearchAppBarButton_Click(object sender, EventArgs e)
        {
            _vm.RepeatSearchCommand.Execute(null);

        }

        private void SearchHistory_Click(object sender, EventArgs e)
        {
            _vm.SelectedSearch = null;
            _vm.SelectedReisMogelijkheid = null;
        }

        private void CloseGeenDataPanel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                GeenDataPanel.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void PinButton_Click(object sender, EventArgs e)
        {
            _vm.PinCommand.Execute(null);
        }

        private void PrijsAppBarButton_Click(object sender, EventArgs e)
        {
            _vm.PrijsCommand.Execute(null);

        }

        private void ReisPlanner_Click(object sender, RoutedEventArgs e)
        {
            _vm.DoSearchCommand.Execute(null);

        }

        private void ReminderMeAppBarButton_Click(object sender, EventArgs e)
        {
            _vm.AddReminderCommand.Execute(null);
        }

        private void MijnStationsButton_Click(object sender, EventArgs e)
        {
            _vm.MijnStationsCommand.Execute(null);
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ReisadviesListBox.SelectedItem != null)
            {
                ReisDeel deel = ReisadviesListBox.SelectedItem as ReisDeel;
                
                if(deel != null)
                    deel.IsExpanded = !deel.IsExpanded;

                ReisadviesListBox.SelectedItem = null;

            }


        }

        private void TapShareButton_Click_1(object sender, EventArgs e)
        {
            if (!SharingViewModel.Instance.IsConnected)
            {
                // Let's assume the intention was to connect and send
                SharingViewModel.Instance.StartSharingSession();
            }
            else
            {
                ShareSearch s = new ShareSearch();
                s.PlannerSearch = _vm.SelectedSearch;
                s.ReisMogelijkheden = _vm.ReisMogelijkheden;

                SharingViewModel.Instance.SendReisadviesToPeer(s.ObjectToByteArray());
                //ShowSendingArrow();
            }
        }
       
    }
}