using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Reflection;
using Nieko.Infrastructure.EventAggregation;
using Nieko.Infrastructure.Collections;
using Nieko.Infrastructure.Logging;

namespace Nieko.Infrastructure.Reflection
{
    /// <summary>
    /// Default implementation of <see cref="IPluginFinder "/>
    /// Plug-in type discovery and instantiation is via AssemblyHelper.FindTypes
    /// (i.e. reflection)
    /// </summary>
    public class PluginFinder : IPluginFinder
    {
        private bool _Resolving;
        protected bool AppIsInitialised { get; private set; }
        protected Queue<Action> PlugInCreationCallBacks { get; private set; }
        protected Func<Type, IPlugInFactory> PlugInFactory { get; private set; }
        protected IInfrastructureEventAggregator EventAggregator { get; private set; }
        private HashSet<Type> _InitializedPluginTypes;
        private Dictionary<Type, Action> _PendingCallbacks;
        private Dictionary<Type, ISet<Type>> _Dependancies;
        private Dictionary<Type, object> _ResolvedPlugins = new Dictionary<Type, object>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="plugInFactory">Builds plug-in factory from a type</param>
        /// <param name="eventAggregator">Aggregator for publishing plug-in start-up events and subscribing to <see cref="IInitializePluginFrameworkEvent"/></param>
        public PluginFinder(Func<Type, IPlugInFactory> plugInFactory, IInfrastructureEventAggregator eventAggregator)
        {
            AppIsInitialised = false;
            _Resolving = false;
            PlugInCreationCallBacks = new Queue<Action>();
            _InitializedPluginTypes = new HashSet<Type>();
            _PendingCallbacks = new Dictionary<Type, Action>();
            _Dependancies = new Dictionary<Type, ISet<Type>>();

            PlugInFactory = plugInFactory;
            EventAggregator = eventAggregator;
            
            EventAggregator.Subscribe<IInitializePluginFrameworkEvent>(InitializePlugins); 
        }

        public void RegisterCreatePluginsCallBack<T>(Action<IEnumerable<T>> callBack)
            where T : class
        {
            Logger.Instance.Log("Plug-ins call-back registered for " + typeof(T).FullName);
            if (AppIsInitialised)
            {
                Logger.Instance.Log("   Call-back resolved immediately");
                callBack(CreatePlugins<T>());
            }
            else
            {
                Logger.Instance.Log("   Call-back added to queue");
                PlugInCreationCallBacks.Enqueue(() => CallbackIfResolved(callBack));
            }
        }

        private IEnumerable<T> CreatePlugins<T>()
            where T : class
        {
            if (_ResolvedPlugins.ContainsKey(typeof(T)))
            {
                return _ResolvedPlugins[typeof(T)] as IEnumerable<T>; 
            }

            var plugins = new List<T>();
            var pluginTypes = AssemblyHelper.FindTypes(t => typeof(T).IsAssignableFrom(t) &&
                !t.IsGenericTypeDefinition && 
                !t.IsAbstract && !t.IsInterface && !Attribute.IsDefined(t, typeof(PluginIgnoreAttribute)));

            foreach (var pluginType in pluginTypes)
            {
                plugins.Add(PlugInFactory(pluginType).Instance as T);
            }
            _ResolvedPlugins[typeof(T)] = plugins;
            
            return plugins;
        }

        private void CallbackIfResolved<T>(Action<IEnumerable<T>> callBack)
            where T : class
        {
            Logger.Instance.Log("Attempting to resolve plug-in " + typeof(T).FullName + ":");
            ISet<Type> dependacies;

            if (!_Dependancies.TryGetValue(typeof(T), out dependacies))
            {
                if (Attribute.IsDefined(typeof(T), typeof(PluginDependanciesAttribute)))
                {
                    dependacies = (Attribute.GetCustomAttribute(typeof(T), typeof(PluginDependanciesAttribute)) as PluginDependanciesAttribute).PluginTypes;
                }
                else
                {
                    dependacies = new HashSet<Type>();
                }
                _Dependancies.Add(typeof(T), dependacies);
            }

            if (dependacies.Any(t => !_InitializedPluginTypes.Contains(t)))
            {
                if (!_PendingCallbacks.ContainsKey(typeof(T)))
                {
                    _PendingCallbacks.Add(typeof(T), () => CallbackIfResolved<T>(callBack));
                }

                Logger.Instance.Log("Resolution of plug-in " + typeof(T).FullName + " deferred" + Environment.NewLine);
                return;
            }

            Logger.Instance.Log(typeof(T).FullName + " plug-ins created" + Environment.NewLine);
            callBack(CreatePlugins<T>());

            if (!_InitializedPluginTypes.Contains(typeof(T)))
            {
                _InitializedPluginTypes.Add(typeof(T));
            }

            if (_PendingCallbacks.ContainsKey(typeof(T)))
            {
                _PendingCallbacks.Remove(typeof(T));
            }

            if (_Resolving)
            {
                return;
            }

            _Resolving = true;
            AttemptResolveAll();
            _Resolving = false;
        }

        private void AttemptResolveAll()
        {
            int lastCount = _PendingCallbacks.Count;
            
            if (lastCount == 0)
            {
                return;
            }

            while (true)
            {
                foreach (var pending in _PendingCallbacks.Values.ToList())
                {
                    pending();
                }

                if (lastCount == _PendingCallbacks.Count)
                {
                    return;
                }

                lastCount = _PendingCallbacks.Count;
            }
        }

        /// <summary>
        /// Called when <see cref="IInitializePluginFrameworkEvent"/> raised at
        /// application start-up, indicating that the IPluginFinder can begin resolving
        /// registered call-backs
        /// </summary>
        /// <param name="args"></param>
        private void InitializePlugins(IInitializePluginFrameworkEvent args)
        {
            if (AppIsInitialised)
            {
                return;
            }

            while (PlugInCreationCallBacks.Count != 0)
            {
                PlugInCreationCallBacks.Dequeue()();
            }

            // If there are still plug-in type(s) that have not been resolved
            // raise an exception. Should only occur if either:
            // - Plug-ins have circular dependencies
            // - A plug-in declares a dependency on a type that has not been requested via
            //      RegisterCreatePluginsCallBack[T].
            // The last case highlights the need to restrict PluginDependencies to either
            // core plug-in types or those requested via the same assembly (implementations
            // are obviously expected to be scattered throughout modules and infrastructure)
            if (_PendingCallbacks.Count > 0)
            {
                var firstFailedPlugin = _PendingCallbacks.First().Key;

                throw new PluginException("Cannot resolve dependencies of plug-in " + firstFailedPlugin.FullName + ". Dependencies : "
                    + _Dependancies[firstFailedPlugin].Aggregate(string.Empty, (list, d) => list + Environment.NewLine + d.FullName));
            }

            AppIsInitialised = true;
            EventAggregator.Publish<IPluginsLoadedEvent>();
        }
    }
}