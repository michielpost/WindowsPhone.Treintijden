﻿using Treintijden.Shared.Services.Models;

namespace Treintijden.Shared.Services.Interfaces
{
    public interface ISettingService
    {
        AppSetting GetSettings();

        void SaveSettings(AppSetting settings);
    }
}
