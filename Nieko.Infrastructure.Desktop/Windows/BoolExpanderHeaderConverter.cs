using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace Nieko.Infrastructure.Windows
{
    /// <summary>
    /// Converts to / from Expander expanded property into
    /// header display text
    /// </summary>
    public class BoolExpanderHeaderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolValue = false;

            if (!bool.TryParse(value.ToString(), out boolValue))
            {
                return string.Empty;
            }

            return boolValue ? "Hide" : "Show";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null || value.ToString() != "Show";
        }
    }
}
