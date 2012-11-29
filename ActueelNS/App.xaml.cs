using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ActueelNS.ViewModel;
using System.Threading.Tasks;
using Microsoft.Phone.Scheduler;
using ActueelNS.Services;
using GalaSoft.MvvmLight.Ioc;
using ActueelNS.Services.Interfaces;
using System.Threading;
using System.Globalization;
using System;
using MockIAPLib;
using ActueelNS.Shared.Controls;
using System.Windows.Controls;

namespace ActueelNS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static NotificationControl NotificationPopup;


        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public static PhoneApplicationFrame RootFrame { get; private set; }


        // Constructor
        public App()
        {
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Change default styles
            InitializeStyleChanges();


            // Phone-specific initialization
            InitializePhoneApplication();

            // Set the template for the RootFrame to the new template we created in the Application.Resources in App.xaml
            RootFrame.Template = Resources["NewFrameTemplate"] as ControlTemplate;
            RootFrame.ApplyTemplate();

            NotificationPopup = (VisualTreeHelper.GetChild(RootFrame, 0) as FrameworkElement).FindName("notifyControl") as NotificationControl;
            NotificationPopup.CloseRequested += NotificationPopup_CloseRequested;

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                //Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disable user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;

                //Application.Current.Host.Settings.EnableCacheVisualization = true;
                //Application.Current.Host.Settings.EnableRedrawRegions = true;
            }


            SetupMockIAP();

            //SharingViewModel.Instance.UIDispatcher = (a) => RootFrame.Dispatcher.BeginInvoke(a);

        }

        private void SetupMockIAP()
        {
#if DEBUG

            MockIAP.Init();
            MockIAP.RunInMockMode(true);
            MockIAP.SetListingInformation(1, "nl-nl", "desc", "1", "Treintijden");

            ProductListing p = new ProductListing()
            {
                Description = "1",
                FormattedPrice = "9.0",
                ImageUri = new Uri("/Images/Purchase/upgrade1.png", UriKind.Relative),
                Name = "Passagier",
                ProductId = "upgrade1",
                ProductType = Windows.ApplicationModel.Store.ProductType.Durable,
                Keywords = new string[] { "" },
                Tag = string.Empty
            };
            ProductListing p2 = new ProductListing()
            {
                Description = "1",
                FormattedPrice = "2.0",
                ImageUri = new Uri("/Images/Purchase/upgrade2.png", UriKind.Relative),
                Name = "Conducteur",
                ProductId = "upgrade2",
                ProductType = Windows.ApplicationModel.Store.ProductType.Durable,
                Keywords = new string[] { "" },
                Tag = string.Empty
            };

            ProductListing p3 = new ProductListing()
            {
                Description = "1",
                FormattedPrice = "5.0",
                ImageUri = new Uri("/Images/Purchase/upgrade3.png", UriKind.Relative),
                Name = "Machinist",
                ProductId = "upgrade3",
                ProductType = Windows.ApplicationModel.Store.ProductType.Durable,
                Keywords = new string[] { "" },
                Tag = string.Empty
            };




            MockIAP.AddProductListing("upgrade1", p);
            MockIAP.AddProductListing("upgrade2", p2);
            MockIAP.AddProductListing("upgrade3", p3);

#endif
        }

        private void InitializeStyleChanges()
        {
            (App.Current.Resources["PhoneForegroundBrush"] as SolidColorBrush).Color = ((SolidColorBrush)App.Current.Resources["ForegroundColor"]).Color;
            (App.Current.Resources["PhoneBackgroundBrush"] as SolidColorBrush).Color = ((SolidColorBrush)App.Current.Resources["BackgroundColor"]).Color;
            (App.Current.Resources["PhoneBorderBrush"] as SolidColorBrush).Color = ((SolidColorBrush)App.Current.Resources["ForegroundColor"]).Color;
            (App.Current.Resources["PhoneTextBoxEditBackgroundBrush"] as SolidColorBrush).Color = ((SolidColorBrush)App.Current.Resources["AlternateColor"]).Color;
            (App.Current.Resources["PhoneTextBoxEditBorderBrush"] as SolidColorBrush).Color = ((SolidColorBrush)App.Current.Resources["AlternateColor"]).Color;

        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            TiltEffect.SetIsTiltEnabled(RootFrame, true);

            Task.Run(() =>
            {
                TaskHelper.ResetTask(false);
            });

            //Set culture
            ISettingService settingService = SimpleIoc.Default.GetInstance<ISettingService>();
            var settings = settingService.GetSettings();

            if (string.IsNullOrEmpty(settings.Culture))
            {

                if (Thread.CurrentThread.CurrentUICulture.CompareInfo.Name.IndexOf("nl-") >= 0
                    || Thread.CurrentThread.CurrentCulture.CompareInfo.Name.IndexOf("nl-") >= 0)
                    settings.Culture = "nl-NL";
                else if (Thread.CurrentThread.CurrentUICulture.CompareInfo.Name.IndexOf("en-") >= 0
                    || Thread.CurrentThread.CurrentCulture.CompareInfo.Name.IndexOf("en-") >= 0)
                    settings.Culture = "en-US";

                settingService.SaveSettings(settings);
            }

            if (!string.IsNullOrEmpty(settings.Culture))
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(settings.Culture);
            }

            try
            {
                ReviewBugger.CheckNumOfRuns();
            }
            catch { }

        }

        void NotificationPopup_CloseRequested(object sender, CloseMeEventArgs e)
        {
            VisualStateManager.GoToState(RootFrame, "TapCanceled", false);
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            TiltEffect.SetIsTiltEnabled(RootFrame, true);

            Task.Run(() =>
            {
                ViewModelLocator.GpsWatcherStatic.StartWatcher();
            });

        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            ViewModelLocator.GpsWatcherStatic.StopWatcher();
            SharingViewModel.Instance.StopSharingSession();

        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            ViewModelLocator.GpsWatcherStatic.StopWatcher();


            ViewModelLocator.Cleanup();

            SharingViewModel.Instance.StopSharingSession();

        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new TransitionFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Handle reset requests for clearing the backstack
            RootFrame.Navigated += CheckForResetNavigation;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;

        }

        private void CheckForResetNavigation(object sender, NavigationEventArgs e)
        {
            // If the app has received a 'reset' navigation, then we need to check
            // on the next navigation to see if the page stack should be reset
            if (e.NavigationMode == NavigationMode.Reset)
                RootFrame.Navigated += ClearBackStackAfterReset;
        }

        private void ClearBackStackAfterReset(object sender, NavigationEventArgs e)
        {
            // Unregister the event so it doesn't get called again
            RootFrame.Navigated -= ClearBackStackAfterReset;

            // Only clear the stack for 'new' (forward) and 'refresh' navigations
            if (e.NavigationMode != NavigationMode.New && e.NavigationMode != NavigationMode.Refresh)
                return;

            // For UI consistency, clear the entire page stack
            while (RootFrame.RemoveBackEntry() != null)
            {
                ; // do nothing
            }
        }

        #endregion
    }
}
