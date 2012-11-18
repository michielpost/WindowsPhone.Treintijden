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
using System.Windows.Data;
using ActueelNS.Resources;

namespace ActueelNS.Converters
{
    public class BoolToSwitchConverter : IValueConverter
    {
        private string FalseValue = AppResources.Uit;
        private string TrueValue = AppResources.Aan;

        public object Convert(object value, Type targetType, object parameter,
              System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return FalseValue;
            else
                return ("On".Equals(value)) ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType,
               object parameter, System.Globalization.CultureInfo culture)
        {
            return value != null ? value.Equals(TrueValue) : false;
        }
    }

}
