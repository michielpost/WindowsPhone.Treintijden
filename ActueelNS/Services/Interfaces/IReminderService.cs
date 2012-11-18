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
using System.Collections.Generic;
using Microsoft.Phone.Scheduler;

namespace ActueelNS.Services.Interfaces
{
    public interface IReminderService
    {
        void CreateReminder(Models.PlannerSearch SelectedSearch, int? Index, DateTime dateTime, string spoor, DateTime reminderTime);

        IList<Reminder> GetReminders();

        void DeleteReminder(string name);
    }
}
