using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Data
{
    /// <summary>
    /// Base class for time period dependant ModelView Stores registrations.
    /// </summary>
    /// <typeparam name="TBase">Type of store</typeparam>
    /// <typeparam name="T">Rule-set</typeparam>
    public abstract class ModelViewStoreRegistration<TBase, T> : IModelViewStoreRegistration
        where TBase : IModelViewStore
        where T : TBase, IModelViewStore
    {
        public Type Type { get { return typeof(T); } }

        public Type BaseStoreType { get { return typeof(TBase); } }

        /// <summary>
        /// Date/Time from which to use this ruleset
        /// </summary>
        public abstract DateTime UsedFrom { get; }

        /// <summary>
        /// Method for setting on a ModelView Store the date at which its business rules operate
        /// </summary>
        public virtual Action<IModelViewStore, DateTime> SetDateAsAt { get { return (store, asAt) => { }; } }
    }
}
