﻿using Microsoft.Phone.Shell;
using ActueelNS.ViewModel;
using ActueelNS.Resources;
using System.Threading.Tasks;
using System.Threading;
using GalaSoft.MvvmLight.Ioc;
using System.Windows;
using ActueelNS.Services;
using ActueelNS.Views.Base;
using Treintijden.Shared.Services.Interfaces;
using Treintijden.PCL.Api.Models;

namespace ActueelNS
{
    public partial class MainPage : ViewBase
    {
        private MainViewModel _vm;
        private bool _animationPlayed = false;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            this.Loaded += new System.Windows.RoutedEventHandler(MainPage_Loaded);

            (ApplicationBar.Buttons[0] as ApplicationBarIconButton).Text = AppResources.toevoegen;
            (ApplicationBar.Buttons[1] as ApplicationBarIconButton).Text = AppResources.planner;

            (ApplicationBar.MenuItems[0] as ApplicationBarMenuItem).Text = AppResources.MainStoringenAppButton;
            (ApplicationBar.MenuItems[1] as ApplicationBarMenuItem).Text = AppResources.MainReviewButton;
            (ApplicationBar.MenuItems[2] as ApplicationBarMenuItem).Text = AppResources.MainSettingsButton;
            (ApplicationBar.MenuItems[3] as ApplicationBarMenuItem).Text = AppResources.MainDonateButton;
            (ApplicationBar.MenuItems[4] as ApplicationBarMenuItem).Text = AppResources.MainInformatieButton;


            SetCulture();

        }

        private async Task SetCulture()
        {
          await Task.Delay(2500);
          ISettingService settingService = SimpleIoc.Default.GetInstance<ISettingService>();
          var settingsCulture = settingService.GetCulture();

          if (string.IsNullOrEmpty(settingsCulture))
          {
            bool shouldRestart = false;

            if (Thread.CurrentThread.CurrentUICulture.CompareInfo.Name.IndexOf("nl-") >= 0
                || Thread.CurrentThread.CurrentCulture.CompareInfo.Name.IndexOf("nl-") >= 0)
              settingsCulture = "nl-NL";
            else if (Thread.CurrentThread.CurrentUICulture.CompareInfo.Name.IndexOf("en-") >= 0
                || Thread.CurrentThread.CurrentCulture.CompareInfo.Name.IndexOf("en-") >= 0)
              settingsCulture = "en-US";
            else
            {
              var result = MessageBox.Show("Set the application language to English?", string.Empty, MessageBoxButton.OKCancel);

              if (result == MessageBoxResult.OK)
              {
                settingsCulture = "en-US";
                shouldRestart = true;
              }
              else
                settingsCulture = "nl-NL";
            }

            settingService.SetCulture(settingsCulture);

            if (shouldRestart)
              MessageBox.Show("Please restart the application to apply the language changes.");
          }

        }

        void MainPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _vm.EnableGpsAsync();
            
            try
            {
                if (ReviewBugger.IsTimeForReview())
                {
                    ReviewBugger.PromptUser();
                }
            }
            catch { }

        }


        protected override async void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            _vm = (MainViewModel)DataContext;
            _vm.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_vm_PropertyChanged);

            _vm.Update();

            CheckStoringenVisible();

            base.OnNavigatedTo(e);

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
                CheckStoringenVisible();
            }
        }

        private void CheckStoringenVisible()
        {
            if (_vm.StoringenVisible && !_animationPlayed)
            {
                _animationPlayed = true;
                StoringenVisible.Begin();
            }
        }

        private void AddButton_Click(object sender, System.EventArgs e)
        {
            _vm.AddStationCommand.Execute(null);
        }

        private void Review_Click(object sender, System.EventArgs e)
        {
            _vm.ReviewCommand.Execute(null);

        }

        private void Donate_Click(object sender, System.EventArgs e)
        {
            _vm.DonateCommand.Execute(null);

        }

        private void StoringenAppButton_Click(object sender, System.EventArgs e)
        {
            _vm.StoringenCommand.Execute(null);
        }

        private void About_Click(object sender, System.EventArgs e)
        {
            _vm.AboutCommand.Execute(null);
        }

        private void Settings_Click(object sender, System.EventArgs e)
        {
            _vm.SettingsCommand.Execute(null);

        }

        private void SearchButton_Click(object sender, System.EventArgs e)
        {
            _vm.PlanCommand.Execute(null);
        }

        private void SearchHistoryListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (SearchHistoryListBox.SelectedItem != null)
            {
                ((ReisadviesViewModel)SearchHistoryListBox.DataContext).AdviceTapCommand.Execute(((PlannerSearch)SearchHistoryListBox.SelectedItem).Id);

                SearchHistoryListBox.SelectedItem = null;
            }
        }

        
    }
}
