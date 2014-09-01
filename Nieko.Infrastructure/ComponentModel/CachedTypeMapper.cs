using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.ComponentModel
{
    internal class CachedTypeMapper<TFrom, TTo> : ITypeMapper<TFrom, TTo>
    {
        private ITypeMapper<TFrom, TTo> _Implementation;

        public CachedTypeMapper(ITypeMapper<TFrom, TTo> implementation)
        {
            _Implementation = implementation;
        }

        public ITypeMapper<TFrom, TTo> Cache()
        {
            return this;
        }

        public ITypeMapper<TFrom, TTo> Cache(string name)
        {
            return this;
        }

        public ITypeMapper<TFrom, TTo> ImplyAll()
        {
            return this;
        }

        public ITypeMapper<TFrom, TTo> ImplyAll(ISet<string> exceptions)
        {
            return this;
        }

        public ITypeMapper<TFrom, TTo> ImplyAll(System.Reflection.BindingFlags propertyBindingFlags, ISet<string> exceptions)
        {
            return this;
        }

        public ITypeMapper<TFrom, TTo> ImplyAll(System.Reflection.BindingFlags propertyBindingFlags)
        {
            return this;
        }

        public ITypeMapper<TFrom, TTo> ImplyProperty<T>(string propertyName)
        {
            return this;
        }

        public ITypeMapper<TFrom, TTo> ImplyGraphs<TDataStore>(Func<Data.IDataStoresManager> dataStoresManagerSupplier, Func<TDataStore> dataStorePrototype, params System.Linq.Expressions.Expression<Func<TFrom, object>>[] accessors) where TDataStore : class, Data.IDataStore
        {
            return this;
        }

        public ITypeMapper<TFrom, TTo> RegisterGraph<T, TDataStore>(Func<Data.IDataStoresManager> dataStoresManagerSupplier, Func<TDataStore> dataStorePrototype, System.Linq.Expressions.Expression<Func<TTo, T>> referenceExpression, params System.Linq.Expressions.Expression<Func<TFrom, object>>[] keyProperties)
            where T : Data.IPrimaryKeyed, new()
            where TDataStore : class, Data.IDataStore
        {
            return this;
        }

        public ITypeMapper<TFrom, TTo> RegisterGraph<T>(Func<System.Linq.Expressions.Expression<Func<T, bool>>, T> referenceSearch, System.Linq.Expressions.Expression<Func<TTo, T>> referenceExpression, params System.Linq.Expressions.Expression<Func<TFrom, object>>[] keyProperties) where T : Data.IPrimaryKeyed, new()
        {
            return this;
        }

        public ITypeMapper<TFrom, TTo> RegisterMap<T>(System.Linq.Expressions.Expression<Func<TFrom, T>> fromProperty, System.Linq.Expressions.Expression<Func<TTo, T>> toProperty)
        {
            return this;
        }

        public ITypeMapper<TFrom, TTo> RegisterFromGraph<TValue, TObject>(System.Linq.Expressions.Expression<Func<TFrom, TValue>> fromProperty, System.Linq.Expressions.Expression<Func<TTo, TObject>> toProperty, Func<IEnumerable<TObject>> toSource, Func<TObject, TValue> objectAccessor) where TValue : IEquatable<TValue>
        {
            return this;
        }

        public ITypeMapper<TFrom, TTo> RegisterToGraph<TValue, TObject>(System.Linq.Expressions.Expression<Func<TFrom, TObject>> fromProperty, System.Linq.Expressions.Expression<Func<TTo, TValue>> toProperty, Func<IEnumerable<TObject>> fromSource, Func<TObject, TValue> objectAccessor) where TValue : IEquatable<TValue>
        {
            return this;
        }

        public ITypeMapper<TFrom, TTo> RegisterFromPivot(Action<IPivotFactory<TFrom, TTo>> config)
        {
            return this;
        }

        public ITypeMapper<TFrom, TTo> RegisterToPivot(Action<IPivotFactory<TTo, TFrom>> config)
        {
            return this;
        }

        public ITypeMapper<TFrom, TTo> RegisterAction(Action<TFrom, TTo> fromAction, Action<TFrom, TTo> toAction)
        {
            return this;
        }

        public void From(TFrom from, TTo to)
        {
            _Implementation.From(from, to);
        }

        public void To(TFrom from, TTo to)
        {
            _Implementation.To(from, to);
        }

        public void From(IEnumerable<TFrom> from, IEnumerable<TTo> to)
        {
            _Implementation.To(from, to);
        }

        public void To(IEnumerable<TFrom> from, IEnumerable<TTo> to)
        {
            _Implementation.To(from, to);
        }

        public void NewFrom<TNewFrom>(ICollection<TNewFrom> from, IEnumerable<TTo> to) where TNewFrom : TFrom, new()
        {
            _Implementation.NewFrom(from, to);
        }

        public void NewTo<TNewTo>(IEnumerable<TFrom> from, ICollection<TNewTo> to) where TNewTo : TTo, new()
        {
            _Implementation.NewTo(from, to);
        }
    }
}
