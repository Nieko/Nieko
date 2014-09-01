using System;
using Nieko.Infrastructure.Collections;
using System.Reflection;
using Nieko.Infrastructure.Data;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Nieko.Infrastructure.ComponentModel
{
    public interface ITypeMapper<TFrom, TTo>
    {
        ITypeMapper<TFrom, TTo> Cache();

        ITypeMapper<TFrom, TTo> Cache(string name);

        /// <summary>
        /// Map between all properties on TFrom and TTo that are Public non-inherited
        /// </summary>
        /// <returns>The same instance</returns>
        ITypeMapper<TFrom, TTo> ImplyAll();
        /// <summary>
        /// Map between all properties on TFrom and TTo that are Public non-inherited
        /// except where name in <paramref name="exceptions"/>
        /// </summary>
        /// <param name="exceptions">Names of properties to exclude from mapping</param>
        /// <returns>The same instance</returns>
        ITypeMapper<TFrom, TTo> ImplyAll(ISet<string> exceptions);
        /// <summary>
        /// Map between all properties on TFrom and TTo matching <paramref name="propertyBindingFlags"/>
        /// Binding Flags except where name in <paramref name="exceptions"/>
        /// </summary>
        /// <param name="propertyBindingFlags">Binding Flags to filter properties with</param>
        /// <param name="exceptions">Names of properties to exclude from mapping</param>
        /// <returns>The same instance</returns>
        ITypeMapper<TFrom, TTo> ImplyAll(BindingFlags propertyBindingFlags, ISet<string> exceptions);
        /// <summary>
        /// Map between all properties on TFrom and TTo matching <paramref name="propertyBindingFlags"/>
        /// Binding Flags
        /// </summary>
        /// <param name="propertyBindingFlags">Binding Flags to filter properties with</param>
        /// <returns>The same instance</returns>
        ITypeMapper<TFrom, TTo> ImplyAll(BindingFlags propertyBindingFlags);
        /// <summary>
        /// Map properties of TFrom and TTo with name <paramref name="propertyName"/>
        /// </summary>
        /// <typeparam name="T">Type of property</typeparam>
        /// <param name="propertyName">Name of properties to map</param>
        /// <returns>The same instance</returns>
        ITypeMapper<TFrom, TTo> ImplyProperty<T>(string propertyName);
        /// <summary>
        /// Maps TFrom reference properties in <paramref name="accessors"/> to corresponding
        /// reference properties on TTo
        /// </summary>
        /// <remarks>
        /// Note that the corresponding properties on TTo must implement IPrimaryKey
        /// and have a key composed of only one property
        /// </remarks>
        /// <typeparam name="TDataStore">Store where TTo instances repose. Is inferred from <paramref name="dataStorePrototype"/></typeparam>
        /// <param name="dataStoresManagerSupplier">Method to retrieve the IDataStoreManager</param>
        /// <param name="dataStorePrototype"><typeparamref name="TDataStore"/> inference. i.e. <c>() => default(ProductStore)</c> </param>
        /// <param name="accessors">Reference Properties on TFrom to imply mappings for</param>
        /// <returns>The same instance</returns>
        ITypeMapper<TFrom, TTo> ImplyGraphs<TDataStore>(Func<IDataStoresManager> dataStoresManagerSupplier, Func<TDataStore> dataStorePrototype, params Expression<Func<TFrom, object>>[] accessors)
            where TDataStore : class, IDataStore;
        /// <summary>
        /// Maps a TTo reference Property to a set of TFrom Properties
        /// representing the TTo Properties keys
        /// </summary>
        /// <typeparam name="T">Type of TTo and TFrom Property</typeparam>
        /// <typeparam name="TDataStore">Store where TTo instances repose. Is inferred from <paramref name="dataStorePrototype"/></typeparam>
        /// <param name="dataStoresManagerSupplier">Method to retrieve the IDataStoreManager</param>
        /// <param name="dataStorePrototype"><typeparamref name="TDataStore"/> inference. i.e. <c>() => default(ProductStore)</c> </param>
        /// <param name="referenceExpression">Property on TTo to map</param>
        /// <param name="keyProperties">Properties to TFrom to map against TTo key</param>
        /// <returns>The same instance</returns>
        ITypeMapper<TFrom, TTo> RegisterGraph<T, TDataStore>(Func<IDataStoresManager> dataStoresManagerSupplier, Func<TDataStore> dataStorePrototype, Expression<Func<TTo, T>> referenceExpression, params Expression<Func<TFrom, object>>[] keyProperties)
            where T : IPrimaryKeyed, new()
            where TDataStore : class, IDataStore;
        /// <summary>
        /// Maps a TTo reference Property to a set of TFrom Properties
        /// representing the TTo Properties keys
        /// </summary>
        /// <typeparam name="T">Type of TTo and TFrom Property</typeparam>
        /// <param name="referenceSearch">Method for finding the reference by filter</param>
        /// <param name="referenceExpression">Property on TTo to map</param>
        /// <param name="keyProperties">Properties to TFrom to map against TTo key</param>
        /// <returns>The same instance</returns>
        ITypeMapper<TFrom, TTo> RegisterGraph<T>(Func<Expression<Func<T, bool>>, T> referenceSearch, Expression<Func<TTo, T>> referenceExpression, params Expression<Func<TFrom, object>>[] keyProperties) where T : IPrimaryKeyed, new();
        /// <summary>
        /// Maps a property on TFrom and TTo via Expressions
        /// </summary>
        /// <typeparam name="T">Property Type</typeparam>
        /// <param name="fromProperty">Property on TFrom</param>
        /// <param name="toProperty">Property on TTo</param>
        /// <returns></returns>
        ITypeMapper<TFrom, TTo> RegisterMap<T>(Expression<Func<TFrom, T>> fromProperty, Expression<Func<TTo, T>> toProperty);
        
        ITypeMapper<TFrom, TTo> RegisterFromGraph<TValue, TObject>(Expression<Func<TFrom, TValue>> fromProperty, Expression<Func<TTo, TObject>> toProperty, Func<IEnumerable<TObject>> toSource, Func<TObject, TValue> objectAccessor)
            where TValue : IEquatable<TValue>;

        ITypeMapper<TFrom, TTo> RegisterToGraph<TValue, TObject>(Expression<Func<TFrom, TObject>> fromProperty, Expression<Func<TTo, TValue>> toProperty, Func<IEnumerable<TObject>> fromSource, Func<TObject, TValue> objectAccessor)
            where TValue : IEquatable<TValue>;

        ITypeMapper<TFrom, TTo> RegisterFromPivot(Action<IPivotFactory<TFrom, TTo>> config);

        ITypeMapper<TFrom, TTo> RegisterToPivot(Action<IPivotFactory<TTo, TFrom>> config);

        /// <summary>
        /// Maps to and from target types using custom actions
        /// </summary>
        /// <param name="fromAction">Action when mapping foward: from &lt;= to</param>
        /// <param name="toAction">Action when mapping back: from =&gt; to</param>
        /// <returns></returns>
        ITypeMapper<TFrom, TTo> RegisterAction(Action<TFrom, TTo> fromAction, Action<TFrom, TTo> toAction);
        /// <summary>
        /// Copies back from i.e. : from &lt;= to
        /// </summary>
        /// <param name="from">object to set property values on</param>
        /// <param name="to">object to take property values from</param>
        void From(TFrom from, TTo to);
        /// <summary>
        /// Copies foward to i.e. : from =&gt; to
        /// </summary>
        /// <param name="from">object to take property values from</param>
        /// <param name="to">object to set property values on</param>
        void To(TFrom from, TTo to);
        /// <summary>
        /// Mirrors items back to one collection from another i.e. : from &lt;= to
        /// </summary>
        /// <param name="from">enumeration of objects to set property values on</param>
        /// <param name="to">enumeration of objects to take property values from</param>
        void From(IEnumerable<TFrom> from, IEnumerable<TTo> to);
        /// <summary>
        /// Mirrors foward items in one collection to another i.e. : from =&gt; to
        /// </summary>
        /// <param name="from">enumeration of objects to take property values from</param>
        /// <param name="to">enumeration of objects to set property values on</param>
        void To(IEnumerable<TFrom> from, IEnumerable<TTo> to);
        /// <summary>
        /// Duplicates items back from one collection to another, creating a new objects in the
        /// destination collection i.e. : from &lt;= to
        /// </summary>
        /// <param name="from">enumeration of objects to set property values on</param>
        /// <param name="to">enumeration of objects to take property values from</param>
        void NewFrom<TNewFrom>(ICollection<TNewFrom> from, IEnumerable<TTo> to)
            where TNewFrom : TFrom, new();
        /// <summary>
        /// Duplicates foward items from one collection to another, creating a new objects in the
        /// destination collection i.e. : from =&gt; to
        /// </summary>
        /// <param name="from">enumeration of objects to take property values from</param>
        /// <param name="to">enumeration of objects to set property values on</param>
        void NewTo<TNewTo>(IEnumerable<TFrom> from, ICollection<TNewTo> to)
            where TNewTo : TTo, new();
    }
}
