using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Nieko.Infrastructure.Windows
{
    public class BoolNotConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(value != null &&
                value is bool &&
                (bool)value == true);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value == null ||
                !(value is bool) ||
                (bool)value == false);
        }
    }
}
