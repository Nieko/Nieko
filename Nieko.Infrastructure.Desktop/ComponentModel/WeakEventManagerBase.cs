using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Nieko.Infrastructure.ComponentModel
{
    /// <summary>
    /// Weak event subscription lifetime manager
    /// </summary>
    /// <typeparam name="T">Type of current class</typeparam>
    /// <typeparam name="TSource">Publishing class</typeparam>
    public abstract class WeakEventManagerBase<T, TSource> : WeakEventManager
        where T : WeakEventManagerBase<T, TSource>, new()
        where TSource : class
    {
        /// <summary>
        /// Event manager singleton for publishers of type <typeparamref name="TSource"/>
        /// </summary>
        public static T Current
        {
            get
            {
                Type managerType = typeof(T);
                T manager = WeakEventManager.GetCurrentManager(managerType) as T;
                if (manager == null)
                {
                    manager = new T();
                    WeakEventManager.SetCurrentManager(managerType, manager);
                }
                return manager;
            }
        }

        public static void AddListener(TSource source, IWeakEventListener listener)
        {
            Current.ProtectedAddListener(source, listener);
        }

        public static void RemoveListener(TSource source, IWeakEventListener listener)
        {
            Current.ProtectedRemoveListener(source, listener);
        }

        protected override sealed void StartListening(object source)
        {
            StartListeningTo(source as TSource);
        }

        protected abstract void StartListeningTo(TSource source);

        protected override sealed void StopListening(object source)
        {
            StopListeningTo(source as TSource);
        }

        protected abstract void StopListeningTo(TSource source);
    }
}