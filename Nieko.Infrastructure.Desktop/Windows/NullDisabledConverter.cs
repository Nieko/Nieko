using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Nieko.Infrastructure.Windows
{
    /// <summary>
    /// Value Converter for turning references nullity (or non-nullity) into a boolean
    /// </summary>
    public class NullDisabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var result = value != null;

            if (parameter != null && parameter.ToString() == "Invert")
            {
                result = !result;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var boolValue = false;

            if (!bool.TryParse(value.ToString(), out boolValue))
            {
                return null;
            }

            if (boolValue)
            {
                return boolValue;
            }
            else
            {
                return null;
            }
        }
    }
}
