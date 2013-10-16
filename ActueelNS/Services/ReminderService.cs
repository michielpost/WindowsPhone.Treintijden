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
using ActueelNS.Services.Interfaces;
using Microsoft.Phone.Scheduler;
using System.Collections.Generic;
using System.Linq;
using ActueelNS.Resources;
using System.Globalization;
using Treintijden.PCL.Api.Models;

namespace ActueelNS.Services
{
    public class ReminderService : IReminderService
    {

        public void CreateReminder(PlannerSearch search, int? index, DateTime dateTime, string spoor, DateTime reminderTime)
        {

            try
            {
                string url = string.Format("/Views/Reisadvies.xaml?id={0}&index={1}", search.Id, index);

                ScheduledAction oldReminder = ScheduledActionService.Find(url);
                if (oldReminder != null)
                    ScheduledActionService.Remove(url);

                Reminder newReminder = new Reminder(url)
                {
                    BeginTime = reminderTime,
                    RecurrenceType = RecurrenceInterval.None,
                    NavigationUri = new Uri(url, UriKind.Relative),
                    Title = AppResources.ReminderAppTitle,
                    Content = string.Format(AppResources.ReminderServiceFormat, search.NaarStation.Name, dateTime.ToString("HH:mm", CultureInfo.InvariantCulture), search.VanStation.Name, spoor),
                }; ;

                ScheduledActionService.Add(newReminder);
            }
            catch { }

        }

        public IList<Reminder> GetReminders()
        {
            return ScheduledActionService.GetActions<ScheduledAction>().OfType<Reminder>().Where(x => x.IsEnabled).Where(x => x.IsScheduled).OrderBy(x => x.BeginTime).ToList();
        }

        public void DeleteReminder(string name)
        {
            ScheduledAction oldReminder = ScheduledActionService.Find(name);
            if (oldReminder != null)
                ScheduledActionService.Remove(name);
        }
    }
}
