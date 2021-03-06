﻿using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using ActueelNS.Services.Interfaces;
using GalaSoft.MvvmLight.Ioc;
using ActueelNS.Views.Base;
using ActueelNS.Resources;
using Treintijden.Shared.Services.Interfaces;
using System.Threading.Tasks;
using System;

namespace ActueelNS.Views
{
    /// <summary>
    /// Description for About.
    /// </summary>
    public partial class About : ViewBase
    {
        /// <summary>
        /// Initializes a new instance of the About class.
        /// </summary>
        public About()
        {
            InitializeComponent();

            LoadSettingsForReleaseNotes();

            YearRun.Text = DateTime.Now.Year.ToString();


        }

        private async Task LoadSettingsForReleaseNotes()
        {
          ISettingService settingService = SimpleIoc.Default.GetInstance<ISettingService>();
          if (settingService.GetCulture() != "nl-NL")
            ReleaseNotesPivot.Visibility = System.Windows.Visibility.Collapsed;
        }

      
        private void EmailButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();

            emailComposeTask.Subject = "Treintijden WP v" + AppResources.AboutVersion.ToLower().Replace("treintijden", string.Empty).Trim();
            emailComposeTask.To = "michiel@michielpost.nl";
            emailComposeTask.Show();

        }

        private void ReviewButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }
    }
}