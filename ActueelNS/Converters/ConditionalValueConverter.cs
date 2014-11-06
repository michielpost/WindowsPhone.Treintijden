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
using System.Globalization;

namespace ActueelNS.Converters
{
  public class ConditionalValueConverter : IValueConverter
  {

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string parameterString = parameter as string;
      bool condition = (bool)value;

      if (!string.IsNullOrEmpty(parameterString))
      {
        string[] parameters = parameterString.Split(new char[] { '|' });
        // Now do something with the parameters

        if (parameters.Length < 2)
          return null;

        if (condition)
          return parameters[0];
        else
          return parameters[1];

      }

      return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
