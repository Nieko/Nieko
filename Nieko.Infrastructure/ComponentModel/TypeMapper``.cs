using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Nieko.Infrastructure.Data;
using System.Reflection;
using Nieko.Infrastructure.Collections;
using Nieko.Infrastructure.ComponentModel;
using Nieko.Infrastructure;

namespace Nieko.Infrastructure.ComponentModel
{
    internal class TypeMapper<TFrom, TTo> : TypeMapper, ITypeMapper<TFrom, TTo>
    {
        private Dictionary<string, Action<TFrom, TTo>> _CopyFromByProperty = new Dictionary<string, Action<TFrom, TTo>>();
        private Dictionary<string, Action<TFrom, TTo>> _CopyToByProperty = new Dictionary<string, Action<TFrom, TTo>>();
        private List<Action<TFrom, TTo>> _CopyFromGraphs = new List<Action<TFrom, TTo>>();
        private List<Action<TFrom, TTo>> _CopyToGraphs = new List<Action<TFrom, TTo>>();

        private static Dictionary<string, ITypeMapper<TFrom, TTo>> _Cache = new Dictionary<string,ITypeMapper<TFrom,TTo>>();
        private static object _Lock = new object();

        internal bool ToReadOnly { get; set; }
        internal bool FromReadOnly { get; set; }

        internal TypeMapper() { }

        public ITypeMapper<TFrom, TTo> Cache()
        {
            return Cache(string.Empty);
        }

        public ITypeMapper<TFrom, TTo> Cache(string name)
        {
            lock(_Lock)
            {
                ITypeMapper<TFrom, TTo> implementation;

                if(!_Cache.TryGetValue(name, out implementation))
                {
                    _Cache.Add(name, this);

                    return this;
                }
                else
                {
                    return new CachedTypeMapper<TFrom, TTo>(implementation);
                }
            }
        }

        public ITypeMapper<TFrom, TTo> RegisterMap<T>(Expression<Func<TFrom, T>> fromProperty, Expression<Func<TTo, T>> toProperty)
        {
            var fromPropertyName = BindingHelper.Name(fromProperty);
            var toPropertyName = BindingHelper.Name(toProperty);

            Expression<Action<TFrom, T>> fromSet = null;
            Expression<Action<TTo, T>> toSet = null;
            Expression<Func<TFrom, T>> fromGet = null;
            Expression<Func<TTo, T>> toGet = null;

            Action<TFrom, T> compiledFromSet = null;
            Func<TFrom, T> compiledFromGet = null;
            Action<TTo, T> compiledtoSet = null;
            Func<TTo, T> compiledtoGet = null;

            Action<TFrom, TTo> fromAction = null;
            Action<TFrom, TTo> toAction = null;

            ParameterExpression fromExpression = Expression.Parameter(typeof(TFrom), "o");
            ParameterExpression toExpression = Expression.Parameter(typeof(TTo), "s");
            ParameterExpression valueExpression = Expression.Parameter(typeof(T), "p");

            if (!FromReadOnly)
            {
                fromSet = Expression.Lambda<Action<TFrom, T>>(
                    Expression.Call(
                        fromExpression,
                        typeof(TFrom).GetProperty(fromPropertyName).GetSetMethod(true), valueExpression),
                    fromExpression,
                    valueExpression);

                toGet = Expression.Lambda<Func<TTo, T>>(
                Expression.PropertyOrField(toExpression, toPropertyName),
                toExpression);
            }

            if (!ToReadOnly)
            {
                toSet = Expression.Lambda<Action<TTo, T>>(
                    Expression.Call(
                        toExpression,
                        typeof(TTo).GetProperty(toPropertyName).GetSetMethod(true), valueExpression),
                    toExpression,
                    valueExpression);

                fromGet = Expression.Lambda<Func<TFrom, T>>(
                    Expression.PropertyOrField(fromExpression, fromPropertyName),
                    fromExpression);
            }

            if (!FromReadOnly)
            {
                compiledFromSet = fromSet.Compile();
                compiledtoGet = toGet.Compile();
                fromAction = (target, to) =>
                {
                    if (to == null)
                    {
                        compiledFromSet(target, default(T));
                        return;
                    }
                    compiledFromSet(target, compiledtoGet(to));
                };
            }
            else
            {
                fromAction = (target, to) => { };
            }

            if (!ToReadOnly)
            {
                compiledFromGet = fromGet.Compile();
                compiledtoSet = toSet.Compile();
                toAction = (target, to) =>
                {
                    if (to != null)
                    {
                        compiledtoSet(to, compiledFromGet(target));
                    }
                };
            }
            else
            {
                toAction = (target, to) => { };
            }

            _CopyFromByProperty[fromPropertyName] = fromAction;
            _CopyToByProperty[fromPropertyName] = toAction;

            return (ITypeMapper<TFrom, TTo>)((object)this);
        }

        public ITypeMapper<TFrom, TTo> RegisterGraph<T>(Func<Expression<Func<T, bool>>, T> referenceSearch, Expression<Func<TTo, T>> referenceExpression, params Expression<Func<TFrom, object>>[] keyProperties)
            where T : IPrimaryKeyed, new()
        {
            var fromParameter = Expression.Parameter(typeof(TFrom), "f");
            var toParameter = Expression.Parameter(typeof(TTo), "t");
            var primaryKeyParameter = Expression.Parameter(typeof(PrimaryKey), "k");
            var referenceParameter = Expression.Parameter(typeof(T), "r");
            var keyValuePairMethod = typeof(PrimaryKey).GetProperties().First(p => p.Name == "Item" && p.DeclaringType == typeof(PrimaryKey)).GetSetMethod();
            PrimaryKey protoTypeKey = PrimaryKey.GetBlankKey<T>();
            IEnumerator<string> keyFieldEnumerator = protoTypeKey.Keys
                .OrderBy(k => k)
                .GetEnumerator();

            Action<TFrom, TTo> graphSetter;
            Action<TFrom, PrimaryKey> primaryKeyBuilder;
            Action<TFrom, PrimaryKey> keyFieldSetter;
            List<Action<TFrom, PrimaryKey>> keySetter = new List<Action<TFrom, PrimaryKey>>();
            Action<TTo, T> referenceSetter;

            Action<TFrom, TTo> graphGetter;
            Func<TTo, T> referenceGetter;
            PropertyInfo keyPropertyInfo;
            Action<TFrom, T> keyFieldGetter;
            List<Action<TFrom, T>> keyGetter = new List<Action<TFrom, T>>();

            referenceSetter = Expression.Lambda<Action<TTo, T>>(
                Expression.Call(
                    toParameter,
                    typeof(TTo).GetProperty(BindingHelper.Name(referenceExpression)).GetSetMethod(),
                    referenceParameter),
                toParameter,
                referenceParameter).Compile();
            referenceGetter = referenceExpression.Compile();

            foreach (var keyProperty in keyProperties)
            {
                keyFieldEnumerator.MoveNext();

                keyFieldSetter = Expression.Lambda<Action<TFrom, PrimaryKey>>(
                    Expression.Call(
                        primaryKeyParameter,
                        keyValuePairMethod,
                        Expression.Constant(keyFieldEnumerator.Current, typeof(string)),
                        Expression.Property(
                            fromParameter,
                            BindingHelper.Name(keyProperty))),
                    fromParameter,
                    primaryKeyParameter).Compile();

                keyPropertyInfo = typeof(TFrom).GetProperty(BindingHelper.Name(keyProperty));

                keyFieldGetter = Expression.Lambda<Action<TFrom, T>>(
                    Expression.Call(
                        fromParameter,
                        keyPropertyInfo.GetSetMethod(),
                        Expression.Condition(
                            Expression.Equal(
                                referenceParameter,
                                Expression.Constant(null)),
                            keyPropertyInfo.PropertyType.AsDefaultExpression(),
                            Expression.PropertyOrField(
                                referenceParameter,
                                keyFieldEnumerator.Current))),
                    fromParameter,
                    referenceParameter)
                    .Compile();

                keySetter.Add(keyFieldSetter);
                keyGetter.Add(keyFieldGetter);
                keyFieldEnumerator.MoveNext();
            }

            primaryKeyBuilder = (from, primaryKey) =>
            {
                foreach (var setter in keySetter)
                {
                    setter(from, primaryKey);
                }
            };

            graphSetter = (from, to) =>
            {
                var primaryKey = PrimaryKey.GetBlankKey<T>();
                primaryKeyBuilder(from, primaryKey);

                T reference = referenceSearch(primaryKey.ToFilterExpression<T>());

                referenceSetter(to, reference);
            };

            graphGetter = (from, to) =>
            {
                var reference = referenceGetter(to);

                foreach (var getter in keyGetter)
                {
                    getter(from, reference);
                }
            };

            _CopyFromGraphs.Add(graphGetter);
            _CopyToGraphs.Add(graphSetter);

            return (ITypeMapper<TFrom, TTo>)((object)this);
        }

        public ITypeMapper<TFrom, TTo> RegisterGraph<T, TDataStore>(Func<IDataStoresManager> dataStoresManagerFactory, Func<TDataStore> dataStorePrototype, Expression<Func<TTo, T>> referenceExpression, params Expression<Func<TFrom, object>>[] keyProperties)
            where T : IPrimaryKeyed, new()
            where TDataStore : class, IDataStore
        {
            Func<Expression<Func<T, bool>>, T> referenceSearch = filter =>
            {
                T instance = default(T);

                dataStoresManagerFactory().DoUnitOfWork<TDataStore>(dataStore =>
                {
                    instance = dataStore.GetItem(filter);
                });

                return instance;
            };

            return RegisterGraph(referenceSearch, referenceExpression, keyProperties);
        }

        public ITypeMapper<TFrom, TTo> RegisterFromGraph<TValue, TObject>(Expression<Func<TFrom, TValue>> fromProperty, Expression<Func<TTo, TObject>> toProperty, Func<IEnumerable<TObject>> toSource, Func<TObject, TValue> objectAccessor)
            where TValue : IEquatable<TValue>
        {
            var fromAccessor = fromProperty.Compile();
            var fromSetter = fromProperty.ToSetter().Compile();
            var toAccessor = toProperty.Compile();
            var toSetter = toProperty.ToSetter().Compile();

            Action<TFrom, TTo> copyFrom = (from, to) =>
                {
                    fromSetter(from, objectAccessor(toAccessor(to)));
                };
            Action<TFrom, TTo> copyTo = (from, to) =>
                {
                    var item = toSource()
                        .First(ts => objectAccessor(ts).Equals(fromAccessor(from)));
                    toSetter(to, item);
                };

            _CopyFromGraphs.Add(copyFrom);
            _CopyToGraphs.Add(copyTo);

            return this;
        }

        public ITypeMapper<TFrom, TTo> RegisterToGraph<TValue, TObject>(Expression<Func<TFrom, TObject>> fromProperty, Expression<Func<TTo, TValue>> toProperty, Func<IEnumerable<TObject>> fromSource, Func<TObject, TValue> objectAccessor)
            where TValue : IEquatable<TValue>
        {
            var fromAccessor = fromProperty.Compile();
            var fromSetter = fromProperty.ToSetter().Compile();
            var toAccessor = toProperty.Compile();
            var toSetter = toProperty.ToSetter().Compile();

            Action<TFrom, TTo> copyFrom = (from, to) =>
            {
                var item = fromSource()
                    .First(fs => objectAccessor(fs).Equals(toAccessor(to)));
                fromSetter(from, item);
            };
            Action<TFrom, TTo> copyTo = (from, to) =>
            {
                toSetter(to, objectAccessor(fromAccessor(from)));
            };

            _CopyFromGraphs.Add(copyFrom);
            _CopyToGraphs.Add(copyTo);

            return this;
        }

        public ITypeMapper<TFrom, TTo> RegisterAction(Action<TFrom, TTo> fromAction, Action<TFrom, TTo> toAction)
        {
            _CopyFromGraphs.Add(fromAction);
            _CopyToGraphs.Add(toAction);

            return this;
        }

        public ITypeMapper<TFrom, TTo> RegisterFromPivot(Action<IPivotFactory<TFrom, TTo>> config) 
        {
            config(new PivotFactory<TFrom, TTo>()
                {
                    FinishAction = mp =>
                        {
                            _CopyFromGraphs.Add(mp.From);
                            _CopyToGraphs.Add(mp.To);
                        }
                });

            return this;
        }

        public ITypeMapper<TFrom, TTo> RegisterToPivot(Action<IPivotFactory<TTo, TFrom>> config)
        {
            config(new PivotFactory<TTo, TFrom>()
            {
                FinishAction = mp =>
                {
                    _CopyFromGraphs.Add((f, t) => mp.To(t, f));
                    _CopyToGraphs.Add((f, t) => mp.From(t, f));
                }
            });

            return this;
        }

        public ITypeMapper<TFrom, TTo> ImplyAll()
        {
            return ImplyAll(new HashSet<string>());
        }

        public ITypeMapper<TFrom, TTo> ImplyAll(ISet<string> exceptions)
        {
            return ImplyAll(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly, exceptions);
        }

        public ITypeMapper<TFrom, TTo> ImplyAll(BindingFlags propertyBindingFlags, ISet<string> exceptions)
        {
            var toProperties = typeof(TTo).GetProperties()
                    .Where(p => !exceptions.Contains(p.Name) && (FromReadOnly || p.CanRead) && (ToReadOnly || p.CanWrite))
                    .Select(p => new { p.Name, p.PropertyType });

            var commonProperties = typeof(TFrom).GetProperties(propertyBindingFlags)
                .Where(p => (ToReadOnly || p.CanRead) && (FromReadOnly || p.CanWrite)) 
                .Select(p => new { p.Name, p.PropertyType })
                .Where(p => toProperties.Any(top => top.Name == p.Name && top.PropertyType == p.PropertyType));

            var implyPropertyMethod = typeof(TypeMapper<TFrom, TTo>).GetMethod("ImplyProperty");

            foreach (var property in commonProperties)
            {
                implyPropertyMethod.MakeGenericMethod(property.PropertyType).Invoke(this, new object[] { property.Name });
            }

            return (ITypeMapper<TFrom, TTo>)((object)this);
        }

        public ITypeMapper<TFrom, TTo> ImplyAll(BindingFlags propertyBindingFlags)
        {
            return ImplyAll(propertyBindingFlags, new HashSet<string>());   
        }

        public ITypeMapper<TFrom, TTo> ImplyProperty<T>(string propertyName)
        {
            ParameterExpression fromExpression = Expression.Parameter(typeof(TFrom), "o");
            ParameterExpression toExpression = Expression.Parameter(typeof(TTo), "s");
            ParameterExpression valueExpression = Expression.Parameter(typeof(T), "p");
            Expression<Func<TFrom, T>> fromProperty;
            Expression<Func<TTo, T>> toProperty;

            fromProperty = Expression.Lambda<Func<TFrom, T>>(
                Expression.PropertyOrField(
                    fromExpression,
                    propertyName),
                fromExpression);

            toProperty = Expression.Lambda<Func<TTo, T>>(
                Expression.PropertyOrField(
                    toExpression,
                    propertyName),
                toExpression);

            RegisterMap(fromProperty, toProperty);

            return (ITypeMapper<TFrom, TTo>)((object)this);
        }

        public ITypeMapper<TFrom, TTo> ImplyGraphs<TDataStore>(Func<IDataStoresManager> dataStoresManagerFactory, Func<TDataStore> dataStorePrototype, params Expression<Func<TFrom, object>>[] accessors)
            where TDataStore : class, IDataStore
        {
            MethodInfo registerMethod = GetType().GetMethod("ImplyGraph");
            Type propertyType;
            string propertyName;

            foreach (var accessor in accessors)
            {
                propertyName = BindingHelper.Name(accessor);
                propertyType = typeof(TTo).GetProperty(propertyName).PropertyType;
                registerMethod.MakeGenericMethod(propertyType, typeof(TDataStore)).Invoke(this, new object[]
                    {
                        dataStoresManagerFactory,
                        dataStorePrototype,
                        propertyName
                    });
            }

            return this;
        }

        public ITypeMapper<TFrom, TTo> ImplyGraph<T, TDataStore>(Func<IDataStoresManager> dataStoresManagerFactory, Func<TDataStore> dataStorePrototype, string propertyName)
            where T : IPrimaryKeyed, new()
            where TDataStore : class, IDataStore
        {
            Expression<Func<TTo, T>> referenceExpression;
            Expression<Func<TFrom, object>> keyExpression;
            var toParameter = Expression.Parameter(typeof(TTo), "to");
            var fromParameter = Expression.Parameter(typeof(TFrom), "from");

            referenceExpression = Expression.Lambda<Func<TTo, T>>(
                Expression.PropertyOrField(
                    toParameter,
                    propertyName),
                toParameter);

            keyExpression = Expression.Lambda<Func<TFrom, object>>(
                Expression.PropertyOrField(
                    fromParameter,
                    propertyName),
                fromParameter);

            return RegisterGraph<T, TDataStore>(dataStoresManagerFactory, dataStorePrototype, referenceExpression, keyExpression);

        }

        /// <summary>
        /// Copies back from i.e. : from &lt;= to
        /// </summary>
        /// <param name="from">object to set property values on</param>
        /// <param name="to">object to take property values from</param>
        public void From(TFrom from, TTo to)
        {
            if (FromReadOnly)
            {
                throw new InvalidOperationException(); 
            }

            TFrom fromObj = (TFrom)from;
            TTo toObj = (TTo)to;

            foreach (var copyAction in _CopyFromByProperty.Values)
            {
                copyAction(fromObj, toObj);
            }

            foreach (var graphAction in _CopyFromGraphs)
            {
                graphAction(from, to);
            }
        }

        /// <summary>
        /// Copies foward to i.e. : from =&gt; to
        /// </summary>
        /// <param name="from">object to take property values from</param>
        /// <param name="to">object to set property values on</param>
        public void To(TFrom from, TTo to)
        {
            if (ToReadOnly)
            {
                throw new InvalidOperationException();
            }

            TFrom fromObj = (TFrom)from;
            TTo toObj = (TTo)to;

            foreach (var copyAction in _CopyToByProperty.Values)
            {
                copyAction(fromObj, toObj);
            }

            foreach (var graphAction in _CopyToGraphs)
            {
                graphAction(from, to);
            }
        }

        public void From(IEnumerable<TFrom> from, IEnumerable<TTo> to)
        {
            var fromEnumerator = from.GetEnumerator();
            var toEnumerator = to.GetEnumerator();

            while (true)
            {
                if(!(fromEnumerator.MoveNext() && toEnumerator.MoveNext()))
                {
                    return;
                }

                From(fromEnumerator.Current, toEnumerator.Current);
            }
        }

        public void To(IEnumerable<TFrom> from, IEnumerable<TTo> to)
        {
            var fromEnumerator = from.GetEnumerator();
            var toEnumerator = to.GetEnumerator();

            while (true)
            {
                if (!(fromEnumerator.MoveNext() && toEnumerator.MoveNext()))
                {
                    return;
                }

                To(fromEnumerator.Current, toEnumerator.Current);
            }
        }

        public void NewFrom<TNewFrom>(ICollection<TNewFrom> from, IEnumerable<TTo> to)
            where TNewFrom : TFrom, new()
        {
            TNewFrom destination;

            foreach(var source in to)
            {
                destination = new TNewFrom();
                From(destination, source);
                from.Add(destination);
            }
        }

        public void NewTo<TNewTo>(IEnumerable<TFrom> from, ICollection<TNewTo> to)
            where TNewTo : TTo, new()
        {
            TNewTo destination;

            foreach (var source in from)
            {
                destination = new TNewTo();
                To(source, destination);
                to.Add(destination);
            }
        }
    }
}