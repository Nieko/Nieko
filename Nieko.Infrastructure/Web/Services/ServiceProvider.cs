using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Reflection;

namespace Nieko.Infrastructure.Web.Services
{
    public class ServiceProvider
    {
        private static IPluginFinder _PluginFinder;
        private static Dictionary<Type, IService> _Services;

        internal static IPluginFinder PluginFinder 
        {
            get
            {
                return _PluginFinder;
            }
            set
            {
                if (_PluginFinder == value)
                {
                    return;
                }

                _PluginFinder = value;

                _PluginFinder.RegisterCreatePluginsCallBack<IService>((services) =>
                    _Services = services
                    .ToDictionary(service => service.GetType()));
            }
        }

        public static IDictionary<Type, IService> Services { get; private set; }

        public static T Get<T>()
            where T : class, IService
        {
            return _Services[typeof(T)] as T;
        }
    }
}
}
