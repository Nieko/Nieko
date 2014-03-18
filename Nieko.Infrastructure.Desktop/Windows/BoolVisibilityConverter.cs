using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Nieko.Infrastructure.Windows
{
    /// <summary>
    /// Complex value converter that allows specifying a comma
    /// delimited list for mapping boolean values to
    /// visibility values
    /// </summary>
    [ValueConversion(typeof(Boolean), typeof(BoolVisibilityConverter))]
    public class BoolVisibilityConverter : IValueConverter
    {
        private static Dictionary<Visibility, bool> _DefaultBackConversions;
        private static Dictionary<bool, Visibility> _DefaultConversions;
        private static Dictionary<string, Visibility> _VisibilitiesByName;

        public static Dictionary<Visibility, bool> DefaultBackConversions
        {
            get
            {
                if (_DefaultBackConversions == null)
                {
                    _DefaultBackConversions = new Dictionary<Visibility, bool>();
                    _DefaultBackConversions[Visibility.Collapsed] = false;
                    _DefaultBackConversions[Visibility.Visible] = true;
                }
                return _DefaultBackConversions;
            }
        }

        public static Dictionary<bool, Visibility> DefaultConversions
        {
            get
            {
                if (_DefaultConversions == null)
                {
                    _DefaultConversions = new Dictionary<bool, Visibility>();
                    _DefaultConversions[false] = Visibility.Collapsed;
                    _DefaultConversions[true] = Visibility.Visible;
                }
                return _DefaultConversions;
            }
        }

        protected static Dictionary<string, Visibility> VisibilitiesByName
        {
            get
            {
                if (_VisibilitiesByName == null)
                {
                    _VisibilitiesByName = new Dictionary<string, Visibility>();
                    foreach (Visibility visibility in Enum.GetValues(typeof(Visibility)))
                    {
                        _VisibilitiesByName.Add(Enum.GetName(typeof(Visibility), visibility), visibility);
                    }
                }
                return _VisibilitiesByName;
            }
        }

        /// <summary>
        /// Converts bool to visibility
        /// </summary>
        /// <param name="value">Boolean value</param>
        /// <param name="targetType">Ignored</param>
        /// <param name="parameter">If string with two values separated by a comma, indicates
        /// what visibility to use for, respectively, false and true values
        /// i.e. "Hide, Visible" : false = Hide, true = Visible
        /// </param>
        /// <param name="culture">ignored</param>
        /// <returns>Visibility</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter == null)
            {
                return DefaultConversions[(Boolean)value];
            }

            Dictionary<bool, Visibility> conversions = null;
            List<string> parameters;

            if (parameter is String || parameter is string)
            {
                parameters = new List<string>(((String)parameter).Split(','));
                if (parameters.Count == 2)
                {
                    if (VisibilitiesByName.ContainsKey(parameters[0]) &&
                        VisibilitiesByName.ContainsKey(parameters[1]))
                    {
                        conversions = new Dictionary<bool, Visibility>();

                        conversions[false] = VisibilitiesByName[parameters[0]];
                        conversions[true] = VisibilitiesByName[parameters[1]];
                    }
                }
            }

            if (conversions == null)
            {
                conversions = DefaultConversions;
            }

            return conversions[(Boolean)value];
        }

        /// <summary>
        /// Converts visibility to bool
        /// </summary>
        /// <param name="value">Visibility</param>
        /// <param name="targetType">Ignored</param>
        /// <param name="parameter">If string with two values separated by a comma, indicates
        /// what visibility to use for, respectively, false and true values
        /// i.e. "Hide, Visible" : false = Hide, true = Visible
        /// </param>
        /// <param name="culture">Ignored</param>
        /// <returns>bool</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter == null)
            {
                return DefaultBackConversions[(Visibility)value];
            }

            Dictionary<Visibility, bool> conversions = null;
            List<string> parameters;

            if (parameter is String || parameter is string)
            {
                parameters = new List<string>(((String)parameter).Split(','));
                if (parameters.Count == 2)
                {
                    if (VisibilitiesByName.ContainsKey(parameters[0]) &&
                        VisibilitiesByName.ContainsKey(parameters[1]))
                    {
                        conversions = new Dictionary<Visibility, bool>();

                        conversions[VisibilitiesByName[parameters[0]]] = false;
                        conversions[VisibilitiesByName[parameters[1]]] = true;
                    }
                }
            }

            if (conversions == null)
            {
                conversions = DefaultBackConversions;
            }

            return conversions[(Visibility)value];
        }

    }
}