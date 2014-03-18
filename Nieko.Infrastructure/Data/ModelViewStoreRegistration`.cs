using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Data
{
    /// <summary>
    /// Base class for time-universal ModelView Stores registrations.
    /// </summary>
    /// <typeparam name="T">Type of ModelView Store</typeparam>
    public class ModelViewStoreRegistration<T> : IModelViewStoreRegistration
        where T : IModelViewStore
    {
        public Type Type { get { return typeof(T); } }

        public Type BaseStoreType { get { return typeof(T); } }

        public DateTime UsedFrom { get { return DateTime.MinValue; } }

        public virtual Action<IModelViewStore, DateTime> SetDateAsAt { get { return (store, asAt) => { }; } }
    }
}
