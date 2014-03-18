using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Collections;

namespace Nieko.Infrastructure.Reflection
{
    /// <summary>
    /// Flags a plug-in with which plug-in(s) must be initialized
    /// before this plug-in is instantiated
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)] 
    public class PluginDependanciesAttribute : Attribute 
    {
        /// <summary>
        /// All base plug-in types that must be initialized before this
        /// plug-in
        /// </summary>
        public ISet<Type> PluginTypes { get; private set; }

        public PluginDependanciesAttribute(params Type[] pluginTypes)
        {
            PluginTypes = new HashSet<Type>(pluginTypes);
        }
    }
}
