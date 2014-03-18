using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Reflection
{
    /// <summary>
    /// Flags a concrete type implementing a plug-in type for exclusion by the
    /// IPluginFinder
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PluginIgnoreAttribute : Attribute
    {
    }
}
