using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Nieko.Infrastructure.Collections;
using System.Collections;
using Nieko.Infrastructure.EventAggregation;

namespace Nieko.Infrastructure.Data
{
    /// <summary>
    /// Repository for persisted data objects. Implementation agnostic but allowing for
    /// Event Aggregation
    /// </summary>
    public interface IDataStore
    {
        ISet<StoredTypeInfo> StoredTypes { get; }
        IQueryable<T> GetItems<T>();
        void Delete<T>(T item);
        T GetItem<T>(Expression<Func<T, bool>> filter);
        void Save<T>(T item);
        void Refresh(IEnumerable collection);
        void Refresh(object entity);
    }
}