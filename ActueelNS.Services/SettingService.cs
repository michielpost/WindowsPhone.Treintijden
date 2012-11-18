using ActueelNS.Services.Interfaces;
using ActueelNS.Services.Models;

namespace ActueelNS.Services
{
    public class SettingService : ISettingService
    {
        private AppSetting _settings;

        public AppSetting GetSettings()
        {
            //Return from memory
            if (_settings != null)
                return _settings;

            var settings = IsolatedStorageCacheManager<AppSetting>.Retrieve("settings.xml");

            if (settings == null)
            {
                settings = new AppSetting()
                {
                    AllowGps = true,
                    AutoFill = true,
                    ShowList = true,
                    GpsListCount = 2,
                    AllowBackgroundTask = false
                };
            }
            
            if (!settings.AllowBackgroundTask.HasValue)
            {
                settings.AllowBackgroundTask = false;
            }

            if (!settings.GpsListCount.HasValue)
            {
                settings.GpsListCount = 2;
            }

            //Save in memory
            _settings = settings;

            return settings;
        }

        public void SaveSettings(Models.AppSetting settings)
        {
            IsolatedStorageCacheManager<AppSetting>.Store("settings.xml", settings);

            //Save in memory
            _settings = settings;
        }
    }
}
