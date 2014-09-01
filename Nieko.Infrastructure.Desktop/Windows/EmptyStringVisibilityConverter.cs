using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Windows
{
    public class EmptyStringVisibilityConverter : NullVisibilityConverter
    {
        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string stringValue = (value ?? string.Empty).ToString().Trim();

            return base.Convert(string.IsNullOrEmpty(stringValue) ? null : stringValue, targetType, parameter, culture);
        }
    }
}
