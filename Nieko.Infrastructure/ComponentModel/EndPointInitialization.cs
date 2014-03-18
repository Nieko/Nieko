using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.ComponentModel;
using Nieko.Infrastructure.Reflection;

namespace Nieko.Infrastructure.ComponentModel
{
    /// <summary>
    /// Initializes the EndPoints hierarchy at Plug-in resolution and
    /// sets the validation for EndPoint creation
    /// </summary>
    public class EndPointInitialization
    {
        private IEndPointValidation _Validation;
        private IEnumerable<IEndPointProvider> _Providers;

        public EndPointInitialization(IPluginFinder pluginFinder)
        {
            pluginFinder.RegisterCreatePluginsCallBack<IEndPointValidation>(plugins =>
                {
                    var validators = plugins
                        .Where(p => (plugins.Count() == 1 || p.GetType() != typeof(NoEndPointValidation)));

                    if (validators.Count() != 1)
                    {
                        throw new InvalidOperationException("Exactly one IEndPointValidation must be defined per application"); 
                    }

                    _Validation = validators.First();
                    TryInitialize();

                });

            pluginFinder.RegisterCreatePluginsCallBack<IEndPointProvider>(plugins =>
                {
                    _Providers = plugins;
                    TryInitialize();
                });
        }

        private void TryInitialize()
        {
            if (_Validation == null || _Providers == null)
            {
                return;
            }

            EndPoint.CanAddCheck = _Validation.CanAddCheck;
            object dummy;
            
            foreach (var provider in _Providers)
            {
                foreach (var property in provider.GetType().GetProperties()
                    .Where(p => p.PropertyType == typeof(EndPoint)))
                {
                    dummy = property.GetValue(provider, null); 
                }
            }
        }
     }
}
