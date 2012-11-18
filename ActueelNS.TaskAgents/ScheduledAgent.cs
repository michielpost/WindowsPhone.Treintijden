using System.Windows;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using System.Linq;
using System;
using ActueelNS.TaskAgents.AsyncWorkloadHelper;
using Microsoft.Phone.Info;

namespace ActueelNS.TaskAgents
{
    public class ExtendedShellInfo
    {
        public ShellTile ShellTile { get; set; }
        public string Name { get; set; }
    }

    public class ScheduledAgent : ScheduledTaskAgent
    {
        private static volatile bool _classInitialized;

        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        public ScheduledAgent()
        {
            if (!_classInitialized)
            {
                _classInitialized = true;
                // Subscribe to the managed exception handler
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    Application.Current.UnhandledException += ScheduledAgent_UnhandledException;
                });
            }
        }

        /// Code to execute on Unhandled Exceptions
        private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
            //var a = DeviceStatus.ApplicationCurrentMemoryUsage;
            //if (a != null) { }

            ResetTiles(0);

            //var b = DeviceStatus.ApplicationCurrentMemoryUsage;
            //if (b != null) { }

            UpdateTiles();

            //var c = DeviceStatus.ApplicationCurrentMemoryUsage;
            //if (c != null) { }

            NotifyComplete();
        }

        private static void UpdateTiles()
        {
            var workManager = new AsyncWorkManager<ExtendedShellInfo, int?>();

            ShellTile appTile = ShellTile.ActiveTiles.First();

            WorkloadInfo<ExtendedShellInfo, int?> result = null;

            if (appTile != null)
            {
                try
                {
                    ExtendedShellInfo info = new ExtendedShellInfo()
                    {
                        Name = string.Empty,
                        ShellTile = appTile
                    };

                    result = workManager.AddWorkItem(StoringCountService.GetStoringen, info);

                }
                catch
                {
                }
            }

            //Wait for all tasks to complete
            workManager.WaitAll();


            if (result.IsComplete && result.Result.HasValue)
            {
                ResetTiles(result.Result.Value);
            }

            ////Update tiles
            //foreach (var foundTile in ShellTile.ActiveTiles.Where(x => x.NavigationUri.ToString().Contains("StationTijden")))
            //{
            //    int begin = foundTile.NavigationUri.ToString().IndexOf("id=");

            //    if (begin > 0)
            //    {
            //        string name = foundTile.NavigationUri.ToString().Substring(begin + 3);

            //        ExtendedShellInfo info = new ExtendedShellInfo()
            //        {
            //            Name = name,
            //            ShellTile = foundTile
            //        };

            //        workManager.AddWorkItem(StoringCountService.GetStoringen, info);
            //    }

            //}
        }

            


        private static void ResetTiles(int number)
        {
            //Update tiles
            foreach (var foundTile in ShellTile.ActiveTiles)
            {
                var liveTile = new StandardTileData
                {
                    Count = number
                };

                foundTile.Update(liveTile);

            }
        }
    }
}