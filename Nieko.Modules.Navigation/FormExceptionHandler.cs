using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Navigation;
using Nieko.Infrastructure.Logging;
using Nieko.Infrastructure.Data;
using Nieko.Infrastructure.ComponentModel;

namespace Nieko.Modules.Navigation
{
    internal class FormExceptionHandler
    {
        private Func<IViewNavigator> _RegionNavigatorSupplier;
        private ILogger _Logger;
        private IDataStoresManager _DataStoresManager;
        private IViewNavigator _RegionNavigator;

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

        public FormExceptionHandler(Func<IViewNavigator> regionNavigatorSupplier, ILogger logger, IDataStoresManager dataStoresManager)
        {
            _RegionNavigatorSupplier = regionNavigatorSupplier;
            _Logger = logger;
            _DataStoresManager = dataStoresManager;
        }

        public void HandleFormException(string prefix, Exception e)
        {
            _Logger.LogException(prefix, e);
            RegionNavigator.EnqueuePostLayoutWork(() =>
            {
                RegionNavigator.Dialogs.ShowModalMessage("ERROR : " + prefix + Environment.NewLine + "If you have any unsaved changes, they will be lost");
                _DataStoresManager.CloseAllDataStores(true);
                RegionNavigator.NavigateTo(EndPoint.Root); 
            });
        }
    }
}
