using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Command;
using System.Device.Location;
using System.Windows;
using ActueelNS.Services;
using ActueelNS.Resources;
using Treintijden.Shared.Services.Interfaces;
using Treintijden.Shared.Services.Models;

namespace ActueelNS.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// </summary>
    public class SettingsViewModel : CustomViewModelBase
    {
        public ISettingService SettingService { get; set; }



        public string PageName
        {
            get { return AppResources.instellingen; }

        }


        private AppSetting _settings;

        public AppSetting Settings
        {
            get { return _settings; }
            set { _settings = value;
            RaisePropertyChanged(() => Settings);
            }
        }

        private string _status;

        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                RaisePropertyChanged(() => Status);
            }
        }


        public RelayCommand SaveCommand { get; private set; }


        /// <summary>
        /// Initializes a new instance of the SettingsViewModel class.
        /// </summary>
        public SettingsViewModel()
        {
            SettingService = SimpleIoc.Default.GetInstance<ISettingService>();

            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real": Connect to service, etc...
            ////}

            SaveCommand = new RelayCommand(() => DoSave());

            ViewModelLocator.GpsWatcherStatic.Instance.StatusChanged += new System.EventHandler<System.Device.Location.GeoPositionStatusChangedEventArgs>(Instance_StatusChanged);
        }

        void Instance_StatusChanged(object sender, System.Device.Location.GeoPositionStatusChangedEventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    SetStatus();
                });
        }

        private void SetStatus()
        {
            //Status = ViewModelLocator.GpsWatcherStatic.Instance.Status.ToString();

            switch (ViewModelLocator.GpsWatcherStatic.Instance.Status)
            {
                case GeoPositionStatus.Disabled:
                    // The Location Service is disabled or unsupported.
                    // Check to see whether the user has disabled the Location Service.
                    if (ViewModelLocator.GpsWatcherStatic.Instance.Permission == GeoPositionPermission.Denied)
                    {
                        // The user has disabled the Location Service on their device.
                        //statusTextBlock.Text = "you have this application access to location.";
                        Status = AppResources.LocatieServiceUitgeschakeld;

                    }
                    else
                    {
                        Status = AppResources.LocatieServiceUitgeschakeld;
                    }
                    break;

                case GeoPositionStatus.Initializing:
                case GeoPositionStatus.NoData:
                case GeoPositionStatus.Ready:
                    Status = string.Empty;
                    break;
            }
        }

        private void DoSave()
        {
            SettingService.SaveSettings(this.Settings);

            if (this.Settings.AllowGps)
            {
              ViewModelLocator.GpsWatcherStatic.StartWatcher();
            }
            else
              ViewModelLocator.GpsWatcherStatic.StopWatcher();

            TaskHelper.ResetTask(true);


        }

        public void Initialize()
        {
            Settings = SettingService.GetSettings();
            SetStatus();

        }
    }
}