using GalaSoft.MvvmLight;
using ActueelNS.Services.Models;
using System.Collections.Generic;
using System;
using ActueelNS.Services;
using ActueelNS.Services.Interfaces;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Scheduler;
using ActueelNS.Resources;

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
    public class ReminderViewModel : CustomViewModelBase
    {
        public IPlannerService PlannerService { get; set; }
        public INavigationService NavigationService { get; set; }
        public IReminderService ReminderService { get; set; }

        public string PageName
        {
            get { return AppResources.herinnermij; }

        }

        public Guid? Guid { get; set; }
        public int? Index { get; set; }
        public DateTime? StartDate { get; set; }
        public string Spoor { get; set; }
        public int Minutes { get; set; }


        private PlannerSearch _selectedSearch;

        public PlannerSearch SelectedSearch
        {
            get { return _selectedSearch; }
            set
            {
                _selectedSearch = value;
                RaisePropertyChanged(() => SelectedSearch);
            }
        }

        private IList<Reminder> _reminders;

        public IList<Reminder> Reminders
        {
            get { return _reminders; }
            set
            {
                _reminders = value;
                RaisePropertyChanged(() => Reminders);
            }
        }

      

        public RelayCommand AddCommand { get; private set; }
        public RelayCommand BackCommand { get; private set; }
        public RelayCommand<string> DeleteCommand { get; private set; }



        /// <summary>
        /// Initializes a new instance of the ReminderViewModel class.
        /// </summary>
        public ReminderViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real": Connect to service, etc...
            ////}

            PlannerService = SimpleIoc.Default.GetInstance<IPlannerService>();
            NavigationService = SimpleIoc.Default.GetInstance<INavigationService>();
            ReminderService = SimpleIoc.Default.GetInstance<IReminderService>();

            AddCommand = new RelayCommand(() => DoAdd());
            BackCommand = new RelayCommand(() => DoBack());
            DeleteCommand = new RelayCommand<string>(x => DeleteReminder(x));

        }

        private void DeleteReminder(string name)
        {
            ReminderService.DeleteReminder(name);

            Reminders = ReminderService.GetReminders();

        }

        private void DoBack()
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
        }

        private void DoAdd()
        {
            if (SelectedSearch != null
                && StartDate.HasValue)
            {
                DateTime reminderTime = StartDate.Value.AddMinutes(-1 * Minutes);

                ReminderService.CreateReminder(SelectedSearch, Index, StartDate.Value, Spoor, reminderTime);

            }
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean own resources if needed

        ////    base.Cleanup();
        ////}



        internal void Initialize(Guid? id, int? index, DateTime? startTime, string spoor)
        {
            SelectedSearch = null;
            StartDate = startTime;
            Spoor = spoor;

            Index = index;

            if (id.HasValue)
            {
                var search = PlannerService.GetSearch(id.Value);

                if (search != null)
                {
                    SelectedSearch = search;
                }

            }

            Minutes = 10;
            Reminders = ReminderService.GetReminders();
        }
    }

}