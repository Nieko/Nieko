using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Globalization;

namespace Nieko.Infrastructure.Windows
{
    /// <summary>
    /// Value Converter for turning references nullity (or non-nullity) into a visiblity value
    /// </summary>
    public class NullVisibilityConverter : IValueConverter
    {
        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = value != null;

            result = parameter != null && parameter.ToString().ToLower() == "invert" ?
                !result :
                result;

            return result ? Visibility.Visible : Visibility.Collapsed;
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
