using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Navigation.RecordNavigation;
using Nieko.Infrastructure.Data;
using Nieko.Infrastructure;
using Nieko.Infrastructure.Collections;
using Nieko.Infrastructure.Navigation;
using Nieko.Infrastructure.ComponentModel;
using Nieko.Infrastructure.Windows.Data;
using System.Windows.Threading;
using Microsoft.Practices.Prism.Regions;

namespace Nieko.Modules.Navigation.RecordNavigator
{
    public class DataNavigatorOwnerBuilder : IDataNavigatorOwnerBuilder, IDataNavigatorOwnerBuilderOwned, IDataNavigatorOwnerBuilderWithView
    {
        private static Dictionary<string, IDataNavigatorView> _NavigatorRegions;

        private IDataStoresManager _DataStoresManager;
        private IViewNavigator _NotifyingRegionNavigator;
        private IDataNavigatorFactory _DataNavigatorFactory;
        private IRegionManager _RegionManager;
        private Func<IDataNavigatorView> _ViewResolver;

        private Func<INotifyDisposing, IDataStoresManager, IDataNavigator, IDataNavigatorOwner> _OwnerBuilder;
        private INotifyDisposing _Parent;
        private IUIConfig _Config;
        private Action<IDataNavigatorViewModel> _ViewCreationAction = null;
        private Func<bool, IDataNavigator> _DataNavigatorCreation;
        
        static DataNavigatorOwnerBuilder()
        {
            _NavigatorRegions = new Dictionary<string, IDataNavigatorView>();
        }

        public DataNavigatorOwnerBuilder(IDataNavigatorFactory dataNavigatorFactory, IRegionManager regionManager, Func<IDataNavigatorView> viewResolver, IViewNavigator notifyingRegionNavigator, IDataStoresManager dataStoresManager)
        {
            _DataNavigatorFactory = dataNavigatorFactory;
            _RegionManager = regionManager;
            _ViewResolver = viewResolver;
            _NotifyingRegionNavigator = notifyingRegionNavigator;
            _DataStoresManager = dataStoresManager;
        }

        public IDataNavigatorOwnerBuilderOwned CreateDataNavigator(INotifyDisposing viewModel)
        {
            _Parent = viewModel;
            _DataNavigatorCreation = (hasView) => _DataNavigatorFactory.CreateInstance(viewModel, hasView);

            return this;
        }

        public IDataNavigatorOwnerBuilderWithView UsingPersistedView<T>(IPersistedView<T> persistedView)
            where T : IEditableMirrorObject
        {
            _OwnerBuilder = (parent, dataStoresManager, dataNavigator) =>
                {
                    DataNavigatorOwner<T> owner;

                    if (parent is IDataNavigatorOwner)
                    {
                        owner = new DataNavigatorOwner<T>((IDataNavigatorOwner)parent, dataStoresManager, dataNavigator, persistedView, _NotifyingRegionNavigator);
                    }
                    else
                    {
                        owner = new DataNavigatorOwner<T>(parent, dataStoresManager, dataNavigator, persistedView, _NotifyingRegionNavigator);
                    }

                    return owner;
                };

            return this;
        }

        public IDataNavigatorOwnerBuilderWithView WithParent(IDataNavigatorOwner parent)
        {
            _Parent = parent;

            return this;
        }

        public IDataNavigatorOwnerBuilderWithView ProvidingUIControlAt(IUIConfig config)
        {
            _Config = config;

            _ViewCreationAction = (dataNavigator) =>
                {
                    IDataNavigatorView view = _ViewResolver();
                    EventHandler viewDisposingHandler = null;

                    view.ViewModel = dataNavigator;
                    view.ViewModel.VisibilityProvider = _Config.VisibilityProvider;

                    Action addControlAction = () =>
                    {

                        if (_NavigatorRegions.ContainsKey(_Config.Region))
                        {
                            foreach (var existingView in _RegionManager.Regions[_Config.Region].Views.ToList())
                            {
                                if (existingView is IDataNavigatorView)
                                {
                                    _RegionManager.Regions[_Config.Region].Remove(existingView);
                                }
                            }

                            _NavigatorRegions.Remove(_Config.Region);
                        }

                        _NavigatorRegions.Add(_Config.Region, view);
                        _RegionManager.Regions[_Config.Region].Add(view);
                        _RegionManager.Regions[_Config.Region].Activate(view);
                    };

                    _NotifyingRegionNavigator.EnqueuePostLayoutWork(addControlAction);

                    viewDisposingHandler = (sender, args) =>
                        {
                            if (_NavigatorRegions.ContainsKey(_Config.Region))
                            {
                                _NavigatorRegions.Remove(_Config.Region);
                            }
                            if (_RegionManager.Regions[_Config.Region].Views.Contains(view))
                            {
                                _RegionManager.Regions[_Config.Region].Remove(view);
                            }
                            view.ViewModel.Disposing -= viewDisposingHandler;
                        };

                    view.ViewModel.Disposing += viewDisposingHandler;
                };

            return this;
        }

        public IDataNavigatorOwner Build()
        {
            IDataNavigatorOwner owner;
            IDataNavigator dataNavigator = _DataNavigatorCreation(_ViewCreationAction != null); 

            if (_ViewCreationAction != null)
            {
                _NotifyingRegionNavigator.EnqueueUIWork(() => _ViewCreationAction((IDataNavigatorViewModel)dataNavigator));   
            }

            owner = _OwnerBuilder(_Parent, _DataStoresManager, dataNavigator);
            dataNavigator.Owner = owner;

            return owner;
        }
    }
}
