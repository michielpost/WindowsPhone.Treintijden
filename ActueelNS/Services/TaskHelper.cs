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
using Microsoft.Phone.Scheduler;
using GalaSoft.MvvmLight.Ioc;
using ActueelNS.Services.Interfaces;
using ActueelNS.Resources;
using Treintijden.Shared.Services.Interfaces;
using System.Threading.Tasks;

namespace ActueelNS.Services
{
    public static class TaskHelper
    {

        public static void ResetTask(bool force)
        {
            try
            {
                // A unique name for your task. It is used to // locate it in from the service.
                var taskName = "BackgroundTask";
                // If the task exists
                var oldTask = ScheduledActionService.Find(taskName) as PeriodicTask;
                if (oldTask != null)
                {
                    ScheduledActionService.Remove(taskName);

                    if (!force)
                        CreateTask(taskName);
                }

                if (force)
                {
                  var settingsService = SimpleIoc.Default.GetInstance<ISettingService>();
                  var settings = settingsService.GetSettings();
                    // Create the Task
                    if (settings.AllowBackgroundTask.HasValue && settings.AllowBackgroundTask.Value)
                    {
                        bool result = CreateTask(taskName);

                        if (!result)
                        {
                            try
                            {
                                settings.AllowBackgroundTask = false;
                                settingsService.SaveSettings(settings);
                            }
                            catch
                            {
                            }
                        }
                    }
                }

                LiveTileService.ResetTiles(0);
            }
            catch { }

            //Test
            //ScheduledActionService.LaunchForTest(taskName, TimeSpan.FromMilliseconds(1500));
        }

        private static bool CreateTask(string taskName)
        {
            bool result = true;

            try
            {
                PeriodicTask task = new PeriodicTask(taskName);
                // Description is required
                task.Description = AppResources.TaskHelperOmschrijving;
                // Add it to the service to execute
                ScheduledActionService.Add(task);
            }
            catch (InvalidOperationException ex)
            {
                result = false;
            }
            catch (Exception e)
            {
                result = false;
            }

            return result;
        }
    }
}
