using Microsoft.Phone.Controls;
using ActueelNS.ViewModel;
using System;
using Microsoft.Phone.Scheduler;
using ActueelNS.Views.Base;

namespace ActueelNS.Views
{
    /// <summary>
    /// Description for Reminder.
    /// </summary>
    public partial class Reminder : ViewBase
    {
        private ReminderViewModel _vm;


        /// <summary>
        /// Initializes a new instance of the Reminder class.
        /// </summary>
        public Reminder()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            _vm.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(_vm_PropertyChanged);

            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            _vm = (ReminderViewModel)DataContext;
            _vm.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_vm_PropertyChanged);

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


                DateTime? time = null;
                if (this.NavigationContext.QueryString.ContainsKey("time"))
                {
                    string timeString = this.NavigationContext.QueryString["time"];

                    time = DateTime.Parse(timeString);
                }

                string spoor = this.NavigationContext.QueryString["spoor"];

                _vm.Initialize(id, index, time, spoor);

            }
            else
            {
                _vm.Initialize(null, null, null, null);

            }

            base.OnNavigatedTo(e);
        }

        void _vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Reminders")
            {
                if (_vm.Reminders != null && _vm.Reminders.Count > 0)
                {
                    OldReminderPanel.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    OldReminderPanel.Visibility = System.Windows.Visibility.Collapsed;

                }
            }
        }


        private void AddButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_vm.StartDate.Value.AddMinutes(-1 * _vm.Minutes) > DateTime.Now)
            {

                _vm.AddCommand.Execute(null);

                ConfirmPanel.Visibility = System.Windows.Visibility.Visible;
                OldReminderPanel.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                NotPossiblePanel.Visibility = System.Windows.Visibility.Visible;
                OldReminderPanel.Visibility = System.Windows.Visibility.Collapsed;
            }

        }

        private void ListPicker_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (_vm != null && e.AddedItems.Count > 0)
            {
                string pick = (string)e.AddedItems[0];

                int minutes = 5;

                if (pick.Contains("10"))
                    minutes = 10;
                else if (pick.Contains("20"))
                    minutes = 20;
                else if (pick.Contains("30"))
                    minutes = 30;
                else if (pick.Contains("45"))
                    minutes = 45;
                else if (pick.Contains("1"))
                    minutes = 60;

                _vm.Minutes = minutes;
            }
        }

        private void NotPossibleClosekButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NotPossiblePanel.Visibility = System.Windows.Visibility.Collapsed;
            OldReminderPanel.Visibility = System.Windows.Visibility.Visible;
        }

       
    }
}