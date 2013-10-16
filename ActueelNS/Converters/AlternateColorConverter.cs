using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace ActueelNS.Converters
{
    public class AlternateColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value == true)
            {
                Color a = new Color() { R = 242, G = 242, B = 247, A = 255 };
                SolidColorBrush alternateColor = new SolidColorBrush(a);

                return alternateColor;
            }

            return new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
