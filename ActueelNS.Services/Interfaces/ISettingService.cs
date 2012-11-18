using ActueelNS.Services.Models;

namespace ActueelNS.Services.Interfaces
{
    public interface ISettingService
    {
        AppSetting GetSettings();

        void SaveSettings(AppSetting settings);
    }
}
