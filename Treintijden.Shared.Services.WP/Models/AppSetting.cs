
namespace Treintijden.Shared.Services.Models
{
    public class AppSetting
    {
        public bool AllowGps { get; set; }
        public bool ShowList { get; set; }
        public bool AutoFill { get; set; }
        public bool? AllowBackgroundTask { get; set; }

        public bool UseHsl { get; set; }
        public bool HasYearCard { get; set; }

        //public string Culture { get; set; }

        public int? GpsListCount { get; set; }
    }
}
