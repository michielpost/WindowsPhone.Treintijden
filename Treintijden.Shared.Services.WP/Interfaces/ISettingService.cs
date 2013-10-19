using System.Threading.Tasks;
using Treintijden.Shared.Services.Models;

namespace Treintijden.Shared.Services.Interfaces
{
    public interface ISettingService
    {
      Task<AppSetting> GetSettingsAsync();

      Task SaveSettingsAsync(AppSetting settings);
    }
}
