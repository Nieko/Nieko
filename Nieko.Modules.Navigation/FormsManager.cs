using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Windows;
using Nieko.Infrastructure.ComponentModel;
using Nieko.Infrastructure.Reflection;
using Nieko.Infrastructure.Navigation;
using Nieko.Infrastructure.Logging;
using Nieko.Infrastructure.Export;

namespace Nieko.Modules.Navigation
{
    internal class FormsManager : IFormsManager
    {
        private IViewNavigator _RegionNavigator;
        private Func<IViewNavigator> _RegionNavigatorSupplier;
        private FormExceptionHandler _FormExceptionHandler;

        private IViewNavigator RegionNavigator
        {
            get
            {
                if (_RegionNavigator == null)
                {
                    _RegionNavigator = _RegionNavigatorSupplier();
                }
                return _RegionNavigator;
            }
        }

        public IDictionary<EndPoint, ViewModelForm> FormsByEndPoint { get; private set; }

        public FormsManager(IPluginFinder pluginFinder, Func<IViewNavigator> regionNavigatorSupplier, FormExceptionHandler formExceptionHandler)
        {
            _RegionNavigatorSupplier = regionNavigatorSupplier;
            _FormExceptionHandler = formExceptionHandler;

            pluginFinder.RegisterCreatePluginsCallBack<IFormsProvider>(RegisterForms);
        }

        public bool Show(EndPoint formEndPoint)
        {
            return Show(formEndPoint, false);  
        }

        public bool Show(EndPoint formEndPoint, bool throwOnFormMissing)
        {
            if (!FormsByEndPoint.ContainsKey(formEndPoint))
            {
                if (throwOnFormMissing)
                {
                    throw new ArgumentException(formEndPoint.GetFullPath() + " does not have an associated form"); 
                }

                return true;
            }

            return RegionNavigator.NavigateTo(formEndPoint); 
        }

        private void RegisterForms(IEnumerable<IFormsProvider> providers)
        {
            var forms = providers.SelectMany(p => p.GetAllForms());

            var duplicates = forms.Where(f => forms.Any(f1 => f1.EndPoint == f.EndPoint && f1 != f));

            if (duplicates.Any())
            {
                throw new ArgumentException("An end point may only have one form defined" + duplicates
                    .Select(d => d.EndPoint)
                    .Distinct()
                    .Aggregate(string.Empty, (current, ep) => current + Environment.NewLine + "Duplicate Form found for : " + ep.GetFullPath()));
            }

            FormsByEndPoint = forms.ToDictionary(f => f.EndPoint); 
        }

        public void HandleFormException(string prefix, Exception e)
        {
            _FormExceptionHandler.HandleFormException(prefix, e); 
        }
    }
}
