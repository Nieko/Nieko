using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Navigation.RecordNavigation
{
    /// <summary>
    /// Indicates that a property of an item navigated through by an
    /// IDataNavigator is not associated with edit operations
    /// </summary>
    /// <remarks>
    /// Properties marked with this attribute are considered non-data
    /// properties and are ignored for purposes involving EditState
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public class NonEditAttribute : Attribute
    {
    }
}
