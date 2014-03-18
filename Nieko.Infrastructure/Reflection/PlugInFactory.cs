using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Reflection
{
    /// <summary>
    /// Builds a plug-in instance of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PlugInFactory<T> : IPlugInFactory
    {
        public PlugInFactory(T instance)
        {
            Instance = instance;
        }

        public object Instance { get; private set; }
    }
}
