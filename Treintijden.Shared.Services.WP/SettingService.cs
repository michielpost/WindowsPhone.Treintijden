using Q42.WinRT.Storage;
using System.Threading.Tasks;
using Treintijden.Shared.Services.Interfaces;
using Treintijden.Shared.Services.Models;

namespace Treintijden.Shared.Services
{
    public class SettingService : ISettingService
    {
        private AppSetting _settings;

        public async Task<AppSetting> GetSettingsAsync()
        {
            //Return from memory
            if (_settings != null)
                return _settings;

            var sh = new StorageHelper<AppSetting>(StorageType.Local, serializerType: StorageSerializer.XML);
            var settings = await sh.LoadAsync("settings");

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

        public Task SaveSettingsAsync(AppSetting settings)
        {
          //Save in memory
          _settings = settings;

          var sh = new StorageHelper<AppSetting>(StorageType.Local, serializerType: StorageSerializer.XML);
          return sh.SaveAsync(settings, "settings");

        }
    }
}
