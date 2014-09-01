using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.EventAggregation;
using Nieko.Infrastructure.Data;
using Nieko.Infrastructure.Logging;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Unity;
using Nieko.Infrastructure.Composition;
using Microsoft.Practices.Prism.Modularity;
using Nieko.Prism.Modularity;
using Nieko.Infrastructure;
using System.Configuration;
using System.Data.SqlClient;

namespace Nieko.Prism.Unity
{
    public abstract class NiekoBootstrapper : UnityBootstrapper
    {
        protected UnityContainerAdapter ContainerAdapter { get; private set; }

        /// <summary>
        /// Initialization action for the Shell created in CreateShell
        /// </summary>
        protected Action ShellLoad { get; set; }

        protected virtual Func<Type, IDataStore> DataStoreSupplier
        {
            get
            {
                return type =>
                {
                    var dataStore = (IDataStore)ContainerAdapter.Resolve(type);

                    return dataStore;
                };
            }
        }

        protected abstract IPrismLogger CreatePrismLogger();

        protected virtual void SetDataStoresManagerType(SingletonImplementation<IDataStoresManager> implementation)
        {
            implementation.Set<PluginDataStoresManager>();
        }

        protected virtual void SetModelViewStoreManagerType(SingletonImplementation<IModelViewStoresManager> implementation)
        {
            implementation.Set<PluginModelViewStoreManager>();
        }

        protected abstract void SetAppDetailsType(SingletonImplementation<IApplicationDetails> implementation);

        protected virtual void RegisterDataInterfaces()
        {
            Logger.Log("Registering Data Interfaces", Category.Info, Priority.Medium);
            SetImplementation<IApplicationDetails>(SetAppDetailsType);
            SetImplementation<IDataStoresManager>(SetDataStoresManagerType);
            ContainerAdapter.RegisterInstance<Func<Type, IDataStore>>(DataStoreSupplier);

            SetImplementation<IModelViewStoresManager>(SetModelViewStoreManagerType);
            ContainerAdapter.RegisterInstance<Func<Nieko.Infrastructure.Data.IModelViewStoresManager>>(() =>
                {
                    return Container.Resolve<Nieko.Infrastructure.Data.IModelViewStoresManager>();
                });
            Container.RegisterInstance<Func<Type, IModelViewStore>>(type =>
            {
                return Container.Resolve(type) as IModelViewStore;
            });
        }

        protected override void InitializeModules()
        {
            Logger.Log("Initializing Modules", Category.Info, Priority.Medium);
#if !DEBUG
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
#endif
            ContainerAdapter.RegisterInstance<IContainerAdapter>(ContainerAdapter);
            ContainerAdapter.RegisterInstance<ILogger>(Logger as ILogger, new ContainerControlledLifetimeManager());

            RegisterDataInterfaces();

            base.InitializeModules();

            //Check for IDataStoresManager errors before publishing events.
            ContainerAdapter.Resolve<Nieko.Infrastructure.Data.IDataStoresManager>();
            PublishStartupEvents(Container.Resolve<IInfrastructureEventAggregator>());
        }

        protected virtual void PublishStartupEvents(IInfrastructureEventAggregator eventAggregator)
        {
            Logger.Log("Loading Shell", Category.Info, Priority.Medium);
            ShellLoad();
            Logger.Log("Publishing Startup Events", Category.Info, Priority.Medium);
            eventAggregator.Publish<IModulesInitializedEvent>();
            eventAggregator.Publish<IInitializePluginFrameworkEvent>();
            eventAggregator.Publish<IInitializeSecurityEvent>();
            eventAggregator.Publish<IApplicationInitializedEvent>();
        }

        protected override IUnityContainer CreateContainer()
        {
            ContainerAdapter = new UnityContainerAdapter();

            ContainerAdapter.RegisterType<IModuleManager, NiekoModuleManager>();

            return ContainerAdapter;
        }

        public void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.Log(e.ToString(), Category.Exception, Priority.High);
            MessageBox.Show("An unhandled exception has occurred. " + Environment.NewLine +
                "If you have any unsaved changes, they will be lost" + Environment.NewLine +
                "The application will now close");
        }

        protected override ILoggerFacade CreateLogger()
        {
            var logger = CreatePrismLogger();

            Nieko.Infrastructure.Logging.Logger.SetLogger(logger);
            return logger;
        }

        private void SetImplementation<T>(Action<SingletonImplementation<T>> method)
        {
            var implementation = new SingletonImplementation<T>();

            method(implementation);
            ContainerAdapter.RegisterSingleton(typeof(T),  implementation.Implementation);
        }
    }
}
