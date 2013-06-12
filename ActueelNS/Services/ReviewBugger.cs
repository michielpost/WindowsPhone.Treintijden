using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Tasks;
using System.IO.IsolatedStorage;
using Coding4Fun.Toolkit.Controls;
using ActueelNS.Resources;

namespace ActueelNS.Services
{
    public static class ReviewBugger
    {
        private const int numOfRunsBeforeFeedback = 11;
        private static int numOfRuns = -1;
        private static readonly IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
        private static readonly Button yesButton = new Button() { Content = AppResources.Ja, Width = 120 };
        private static readonly Button laterButton = new Button() { Content = "later", Width = 120 };
        private static readonly Button neverButton = new Button() { Content = AppResources.Nooit, Width = 120 };
        private static readonly MessagePrompt messagePrompt = new MessagePrompt();

        public static void CheckNumOfRuns()
        {
            if (!settings.Contains("numOfRuns"))
            {
                numOfRuns = 1;
                settings.Add("numOfRuns", 1);
            }
            else if (settings.Contains("numOfRuns") && (int)settings["numOfRuns"] != -1)
            {
                settings.TryGetValue("numOfRuns", out numOfRuns);
                numOfRuns++;
                settings["numOfRuns"] = numOfRuns;
            }
        }

        public static void DidReview()
        {
            if (settings.Contains("numOfRuns"))
            {
                numOfRuns = -1;
                settings["numOfRuns"] = -1;
            }
        }

        public static bool IsTimeForReview()
        {
            return numOfRuns % numOfRunsBeforeFeedback == 0 ? true : false;
        }

        public static void PromptUser()
        {
            yesButton.Click += new RoutedEventHandler(yesButton_Click);
            yesButton.Background = new SolidColorBrush(Colors.Black);
            yesButton.Foreground = new SolidColorBrush(Colors.White);
            yesButton.BorderBrush = new SolidColorBrush(Colors.White);

            laterButton.Click += new RoutedEventHandler(laterButton_Click);
            laterButton.Background = new SolidColorBrush(Colors.Black);
            laterButton.Foreground = new SolidColorBrush(Colors.White);
            laterButton.BorderBrush = new SolidColorBrush(Colors.White);

            neverButton.Click += new RoutedEventHandler(neverButton_Click);
            neverButton.Background = new SolidColorBrush(Colors.Black);
            neverButton.Foreground = new SolidColorBrush(Colors.White);
            neverButton.BorderBrush = new SolidColorBrush(Colors.White);

            messagePrompt.Message = AppResources.AskReview;

            messagePrompt.ActionPopUpButtons.RemoveAt(0);
            messagePrompt.ActionPopUpButtons.Add(yesButton);
            messagePrompt.ActionPopUpButtons.Add(laterButton);
            messagePrompt.ActionPopUpButtons.Add(neverButton);
            messagePrompt.Show();
        }

        static void yesButton_Click(object sender, RoutedEventArgs e)
        {
            var review = new MarketplaceReviewTask();
            review.Show();
            messagePrompt.Hide();
            DidReview();
        }

        static void laterButton_Click(object sender, RoutedEventArgs e)
        {
            numOfRuns = -1;
            messagePrompt.Hide();
        }

        static void neverButton_Click(object sender, RoutedEventArgs e)
        {
            DidReview();
            messagePrompt.Hide();
        }
    }
}
