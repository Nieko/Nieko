using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Reflection;
using Nieko.Infrastructure.EventAggregation;
using Nieko.Infrastructure.Windows.Data;
using Nieko.Infrastructure.Composition;
using Nieko.Infrastructure.Export;
using Nieko.Modules.Architecture.Export;
using Nieko.Infrastructure.ComponentModel;
using System.Linq.Expressions;
using Nieko.Modules.Architecture.Composition;
using Nieko.Infrastructure.Data;
using Nieko.Infrastructure.Navigation;
using Nieko.Modules.Architecture.Navigation;

namespace Nieko.Modules.Architecture
{
    public class ArchitectureModule : ContainerModule
    {
        public override void Initialize()
        {
            Container.RegisterInstance<IGenericSupplierBuilder>(new GenericSupplierBuilder(type => Container.Resolve(typeof(Func<>).MakeGenericType(type))));   

            Container.RegisterInstance<Func<Type, IPlugInFactory>>((type) =>
                {
                    var factoryType = typeof(PlugInFactory<>).MakeGenericType(type);
                    return (IPlugInFactory)Container.Resolve(factoryType);
                });

            Container.RegisterSingleton<IStoreConnectionStringProvider, StoreConnectionStringProvider>(); 
            Container.RegisterSingleton<IEventAggregatorFacade, PrismEventAggregator>();
            Container.RegisterSingleton<IInfrastructureEventAggregator, InfrastuctureEventAggregator>();

            Container.RegisterSingleton<IPluginFinder, PluginFinder>(true);
            Container.RegisterSingleton<ISettingsProvider, SettingsProvider>();
            Container.Resolve<EndPointInitialization>();
            Container.RegisterSingleton<IPrintManager, PrintManager>();

            Container.RegisterTypeAndFactory<IWaitDialog, WaitDialog>();
            Container.RegisterSingleton<IDialogs, Dialogs>();
        }

        protected override DependantModule GetDependancies()
        {
            return CreateDependancies<NiekoModuleNames>(mn => mn.Architecture);
        }
    }
}
