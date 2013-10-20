using Q42.WinRT.Storage;
using System.Threading.Tasks;
using Treintijden.Shared.Services.Interfaces;
using Treintijden.Shared.Services.Models;
using Treintijden.Shared.Services.WP.TEMP;

namespace Treintijden.Shared.Services
{
    public class SettingService : ISettingService
    {
        private AppSetting _settings;

        public AppSetting GetSettings()
        {
            //Return from memory
            if (_settings != null)
                return _settings;

            var settings = SettingsHelper.Get<AppSetting>("settings");

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

        public void SaveSettings(AppSetting settings)
        {
          //Save in memory
          _settings = settings;

          SettingsHelper.Set("settings", settings);

        }


        public string GetCulture()
        {
          return SettingsHelper.Get<string>("culture");
        }

        public void SetCulture(string culture)
        {
          SettingsHelper.Set("culture", culture);
        }
    }
}
