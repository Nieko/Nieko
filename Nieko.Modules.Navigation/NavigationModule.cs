using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Navigation;
using Nieko.Infrastructure.EventAggregation;
using Nieko.Infrastructure.Composition;
using Nieko.Infrastructure.Navigation.Menus;
using Background = Nieko.Infrastructure.Windows.Background;
using System.Linq.Expressions;
using Nieko.Modules.Navigation.Menus;
using Nieko.Infrastructure.Windows;
using Nieko.Infrastructure.Navigation.RecordNavigation;
using Nieko.Modules.Navigation.RecordNavigator;
using Nieko.Prism.Events;
using Nieko.Infrastructure.Windows.Data;
using Nieko.Modules.Navigation.Data;

namespace Nieko.Modules.Navigation
{
    public class NavigationModule : ContainerModule
    {
        public override void Initialize()
        {
            Container.RegisterSingleton<IViewNavigator, ViewNavigator>();
            Container.RegisterInstance<Func<IViewNavigator>>(() => Container.Resolve<IViewNavigator>());  
            Container.RegisterSingleton<IMenuBarManager, MenuBarManager>();
            Container.RegisterSingleton<IStartupNotifier, StartupNotifier>();
            Container.RegisterSingleton<IMenuNavigator, MenuNavigator>();
            
            Container.RegisterTypeAndFactory<IStartupNotificationsRequestEvent, StartupNotificationsRequestEvent>();

            Container.RegisterTypeAndFactory<IFormsManager, FormsManager>();
            Container.RegisterSingleton<IDataNavigatorFactory, DataNavigatorFactory>();
            Container.RegisterTypeAndFactory<IDataNavigatorOwnerBuilder, DataNavigatorOwnerBuilder>();

            Container.RegisterTypeAndFactory<IDataNavigator, DataNavigator>();
            Container.RegisterTypeAndFactory<IDataNavigatorViewModel, DataNavigatorViewModel>();
            Container.RegisterTypeAndFactory<IDataNavigatorView, DataNavigatorControl>();

            Container.RegisterType<IGraphFactory, GraphFactory>();
            Container.RegisterTypeAndFactory<IPersistedViewRoot, PersistedViewRoot>();
        }

        protected override DependantModule GetDependancies()
        {
            return CreateDependancies<NiekoModuleNames>(mn => mn.Navigation)
                .AddDependancy<NiekoModuleNames>(mn => mn.Architecture);  
        }
    }
}
