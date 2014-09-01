using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using Nieko.Infrastructure.Collections;
using System.Linq.Expressions;
using System.ComponentModel.DataAnnotations;

namespace Nieko.Infrastructure.Data
{
    /// <summary>
    /// Entity Unique PrimaryKey.
    /// </summary>
    /// <remarks>
    /// Set of property values that uniquely identifies an object against others
    /// of the same type. Allows changes to models derived from data classes
    /// to be duplicated to and from, provides standard value-based comparison for
    /// uniquely identifiable objects, and allows searching and identifying data
    /// objects other than by auto-generated IDs.
    /// </remarks>
    [Serializable]
    public class PrimaryKey : Dictionary<string, object>, IComparable<PrimaryKey>, IComparable, IEquatable<PrimaryKey>, ISerializable
    {
        private static string _IPrimaryKeyedKeyPropertyName;
        public static Dictionary<Type, Func<object>> ProtoTypeBuilders = new Dictionary<Type, Func<object>>();
        private static Comparer _Comparer = new Comparer(CultureInfo.CurrentCulture);
        private static Dictionary<Type, Dictionary<string, Func<object, object>>> _TypeKeyProperties = new Dictionary<Type, Dictionary<string, Func<object, object>>>();
        private static Dictionary<Type, IPrimaryKeyFilterExpressionProvider> _TypeFilterExpressionProviders = new Dictionary<Type, IPrimaryKeyFilterExpressionProvider>();
        private static Dictionary<Type, IList<string>> _TypeOrderedKeys = new Dictionary<Type, IList<string>>();

        private WeakReference _EntityReference;
        private Type _EntityType;
        private bool _HasToString = false;
        private string _ToString = "";
        private bool _HasHash = false;
        private int _Hash = 0;
        private Func<string> _ToStringFunction;
        private IList<string> _OrderedKeys;

        private IList<string> OrderedKeys
        {
            get
            {
                if (_OrderedKeys == null)
                {
                    _OrderedKeys = this
                        .OrderBy(kvp => kvp.Key)
                        .Select(kvp => kvp.Key)
                        .ToList();
                }

                return _OrderedKeys;
            }
            set
            {
                _OrderedKeys = value;
            }
        }

        private PrimaryKey() 
        {
            _ToStringFunction = DefaultToString;
        } 

        /// <summary>
        /// PrimaryKey Constructor for de-serialization
        /// </summary>
        /// <param name="info">SerializationInfo</param>
        /// <param name="context">StreamingContext</param>
        public PrimaryKey(SerializationInfo info, StreamingContext context) 
            : this()
        {
            int fieldCount = info.GetInt32("FieldCount");
            Type fieldType;
            string fieldTypeName;
            string fieldName;
            object keyValue;

            for (int i = 0; i < fieldCount; i++)
            {
                fieldName = info.GetString("FieldName" + i.ToString());
                fieldTypeName = info.GetString("FieldType" + i.ToString());
                fieldType = Type.GetType(fieldTypeName);
                keyValue = info.GetValue("KeyValue" + i.ToString(), fieldType);
                Add(fieldName, keyValue);
            }
        }

        /// <summary>
        /// Main PrimaryKey Constructor 
        /// </summary>
        /// <param name="entity">Keyed object</param>
        public PrimaryKey(IPrimaryKeyed entity)
            : this()
        {
            _EntityReference = new WeakReference(entity);
            if (entity is INotifyPropertyChanged)
            {
                (entity as INotifyPropertyChanged).PropertyChanged += entity_PropertyChanged;
            }
            
            _EntityType = entity.GetType();
            var keyProperties = GetKeyProperties(_EntityType);
            OrderedKeys = _TypeOrderedKeys[_EntityType]; 

            if (keyProperties.Count == 0)
            {
                throw new InvalidKeysException(_EntityType.FullName + " does not define any keys");
            }

            foreach (KeyValuePair<string, Func<object, object>> entry in keyProperties)
            {
                base.Add(entry.Key, entry.Value(entity));
            }

            if (entity is IPrimaryKeyedToStringProvider)
            {
                _ToStringFunction = (entity as IPrimaryKeyedToStringProvider).PrimaryKeyToString;
            }
        }

        static PrimaryKey()
        {
            _IPrimaryKeyedKeyPropertyName = BindingHelper.Name((IPrimaryKeyed pk) => pk.PrimaryKey);
        }

        /// <summary>
        /// Gets key value by name
        /// </summary>
        /// <param name="key">Key name</param>
        /// <returns>Key value</returns>
        public new object this[string key]
        {
            get
            {
                return base[key];
            }
            set
            {
                if (_EntityReference != null)
                {
                    throw new InvalidOperationException("Cannot change PrimaryKey; PrimaryKey is attached to an Entity");
                }
                base[key] = value;
                _HasToString = false;
                _HasHash = false;
            }
        }

        /// <summary>
        /// Creates empty Key for a keyed type
        /// </summary>
        /// <typeparam name="T">Keyed type</typeparam>
        /// <returns>Empty key</returns>
        public static PrimaryKey GetBlankKey<T>()
            where T : IPrimaryKeyed, new()
        {
            PrimaryKey key = new PrimaryKey();

            foreach (var property in GetKeyProperties(typeof(T)).Keys)
            {
                key.Add(property, null);
            }

            return key;
        }

        public static PrimaryKey CreateKey<T>(params object[] keys)
            where T : IPrimaryKeyed, new()
        {
            var key = GetBlankKey<T>();
            
            for(int i =0;i<keys.Length;i++ )
            {
                key[key.OrderedKeys[i]] = keys[i];
            }

            return key;
        }

        /// <summary>
        /// Builds a key from an object and a set of property expressions of that object
        /// </summary>
        /// <typeparam name="T">Type of object holding properties to create the key from</typeparam>
        /// <param name="keyed">Object that holds the key properties</param>
        /// <param name="properties">Property expressions to build the key from</param>
        /// <returns>Fabricated Primary Key</returns>
        public static PrimaryKey CreateKey<T>(T keyed, params Expression<Func<T, object>>[] properties)
        {
            var key = new PrimaryKey();

            foreach(var property in properties)
            {
                key[BindingHelper.Name(property)] = property.Compile()(keyed);
            }

            return key;
        }

        /// <summary>
        /// Get key property accessors for a keyed type
        /// </summary>
        /// <param name="type">Keyed type</param>
        /// <returns>Dictionary of Accessors by property name</returns>
        public static Dictionary<string, Func<object, object>> GetKeyProperties(Type type)
        {
            Dictionary<string, Func<object, object>> keyProperties;

            if (!_TypeKeyProperties.TryGetValue(type, out keyProperties))
            {
                Expression<Func<object, object>> accessor;
                ParameterExpression param = Expression.Parameter(typeof(object), "o");
                Type definingType;
                if(Attribute.IsDefined(type, typeof(MetadataTypeAttribute)))
                {
                    definingType = (Attribute.GetCustomAttribute(type, typeof(MetadataTypeAttribute)) as MetadataTypeAttribute).MetadataClassType;
                }
                else
                {
                    definingType = type;
                }

                keyProperties = new Dictionary<string, Func<object, object>>();
                foreach (PropertyInfo property in definingType.GetProperties
                    (BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance).Where(
                        p=> Attribute.GetCustomAttribute(p, typeof(PrimaryKeyAttribute)) != null))
                {
                    accessor = Expression.Lambda<Func<object, object>>(
                        Expression.Convert(
                            Expression.PropertyOrField(
                                Expression.Convert(param, type),
                                property.Name),
                            typeof(object)),
                        param);

                    keyProperties.Add(property.Name, accessor.Compile());
                }

                _TypeKeyProperties.Add(type, keyProperties);
                _TypeOrderedKeys.Add(type, keyProperties.Keys
                    .OrderBy(k => k)
                    .ToList());
            }

            return keyProperties;
        }

        public static int CompareKeyed(object left, object right)
        {
            var leftKey = ((left is IPrimaryKeyed) ?
                (left as IPrimaryKeyed).PrimaryKey :
                null);
            var rightKey = ((right is IPrimaryKeyed) ?
                (right as IPrimaryKeyed).PrimaryKey :
                null);

            if(leftKey == null && rightKey == null)
            {
                return 0;
            }

            if(leftKey == null && rightKey != null)
            {
                return -1;
            }

            if (leftKey != null && rightKey == null)
            {
                return 1;
            }

            return leftKey.CompareTo(rightKey);
        }

        /// <summary>
        /// Adds a key property and value. If PrimaryKey is associated with an actual object
        /// then an InvalidOperationException is raised
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        /// <param name="key">Key Property name</param>
        /// <param name="value">Key Property value</param>
        public new void Add(string key, object value)
        {
            if (_EntityReference != null)
            {
                throw new InvalidOperationException("Cannot change PrimaryKey; PrimaryKey is attached to an Entity");
            }
            base.Add(key, value);
            _HasToString = false;
            _HasHash = false;
        }

        /// <summary>
        /// Removes all property names and values. If PrimaryKey is associated with an actual object
        /// then an InvalidOperationException is raised
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        public new void Clear()
        {
            if (_EntityReference != null)
            {
                throw new InvalidOperationException("Cannot change PrimaryKey; PrimaryKey is attached to an Entity");
            }
            base.Clear();
            _HasToString = false;
            _HasHash = false;
        }

        /// <summary>
        /// Compares this instance to another key
        /// </summary>
        /// <param name="other">Key to compare to</param>
        /// <remarks>
        /// Compares only by value, ignoring key names. This allows keys that are from different types that
        /// are functionally equivalent to be compared.
        /// </remarks>
        /// <returns>Negative if instance is less than <paramref name="other"/>, zero if equal, positive if greater
        /// </returns>
        public int CompareTo(PrimaryKey other)
        {
            try
            {
                IEnumerator<string> oSubjectEnum;
                IEnumerator<string> oObjectEnum;
                PrimaryKey toKey = (PrimaryKey)other;
                int iResult;

                oSubjectEnum = this._OrderedKeys.GetEnumerator();
                oObjectEnum = toKey._OrderedKeys.GetEnumerator();

                while (oSubjectEnum.MoveNext())
                {
                    oObjectEnum.MoveNext();
                    if (this[oSubjectEnum.Current] is IPrimaryKeyed)
                    {
                        iResult = (this[oSubjectEnum.Current] as IPrimaryKeyed).CompareTo(other[oObjectEnum.Current]); 
                    }
                    else if (other[oObjectEnum.Current] is IPrimaryKeyed)
                    {
                        iResult = (other[oObjectEnum.Current] as IPrimaryKeyed).CompareTo(this[oSubjectEnum.Current]);
                    }
                    else
                    {
                        iResult = _Comparer.Compare(this[oSubjectEnum.Current], other[oObjectEnum.Current]);
                    }

                    if (iResult == 0)
                    {
                        continue;
                    }
                    return iResult;
                }
                return 0;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public override int GetHashCode()
        {
            if(_HasHash)
            {
                return _Hash;
            }

            _Hash = 13;

            foreach (var key in OrderedKeys)
            {
                _Hash = (_Hash * 7) + (this[key] ?? 0).GetHashCode();
            }

            _HasHash = true;
            return _Hash;
        }

        public bool Equals(PrimaryKey other)
        {
            return CompareTo(other) == 0;
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            object keyValue;
            int i = 0;

            info.AddValue("FieldCount", Count);
            foreach (string fieldName in Keys)
            {
                keyValue = this[fieldName];
                info.AddValue("FieldName" + i.ToString(), fieldName);
                info.AddValue("FieldType" + i.ToString(), keyValue.GetType().FullName);
                info.AddValue("KeyValue" + i.ToString(), keyValue);
                i++;
            }
        }

        public new void Remove(string key)
        {
            if (_EntityReference != null)
            {
                throw new InvalidOperationException("Cannot change PrimaryKey; PrimaryKey is attached to an Entity");
            }
            base.Remove(key);
            _HasToString = false;
            _HasHash = false;
        }

        public object[] ToArray()
        {
            object[] keysArray = new object[Count];
            IEnumerator enumerator = Values.GetEnumerator();

            for(int i = 0; i < Count; i++)
            {
                enumerator.MoveNext();
                keysArray[i] = enumerator.Current;
            }

            return keysArray;
        }

        public override string ToString()
        {
            if(!_HasToString)
            {
                _ToString = _ToStringFunction();
                _HasToString = true;
            }
            return _ToString;
        }

        /// <summary>
        /// Returns an expression to find instance(s) of type T that have key property values
        /// indicated by this key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Expression<Func<T, bool>> ToFilterExpression<T>()
        {
            IPrimaryKeyFilterExpressionProvider provider = null;

            if (!_TypeFilterExpressionProviders.TryGetValue(typeof(T), out provider))
            {
                Type providerType;
                var attribute = Attribute.GetCustomAttribute(typeof(T), typeof(FilterExpressionProviderAttribute));

                if (attribute == null)
                {
                    provider = DefaultPrimaryKeyExpressionProvider.Instance;
                }
                else
                {
                    providerType = (attribute as FilterExpressionProviderAttribute).Provider;

                    provider = _TypeFilterExpressionProviders.Values.FirstOrDefault(p => p.GetType() == providerType);

                    if (provider == null)
                    {
                        provider = Activator.CreateInstance(providerType) as IPrimaryKeyFilterExpressionProvider;
                    }
                }

                _TypeFilterExpressionProviders.Add(typeof(T), provider);
            }

            return provider.ToFilterExpression<T>(this); 
        }

        public void TouchKeys()
        {
            object dummy;
            IPrimaryKeyed keyedReference;

            foreach(var key in Values)
            {
                dummy = key;
                keyedReference = dummy as IPrimaryKeyed;
                if(keyedReference != null)
                {
                    keyedReference.PrimaryKey.TouchKeys();
                }
            }
        }

        private string DefaultToString()
        {
            string toString = "";
            foreach(object keyvalue in base.Values)
            {
                if(keyvalue == null)
                {
                    _ToString += "";
                    continue;
                }
                toString += keyvalue.ToString() + " ";
            }

            return toString;
        }

        private void DetachEntity(IPrimaryKeyed entity)
        {
            if (_EntityReference == null || !_EntityReference.IsAlive || _EntityReference.Target != entity)
            {
                throw new InvalidOperationException("PrimaryKey to attached to this Entity");
            }
            _EntityReference = null;
            if (entity is INotifyPropertyChanged)
            {
                (entity as INotifyPropertyChanged).PropertyChanged -= entity_PropertyChanged;
            }
        }

        private void entity_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Dictionary<string, Func<object, object>> keyProperties = GetKeyProperties(_EntityType);
            if (!keyProperties.ContainsKey(e.PropertyName))
            {
                return;
            }

            base[e.PropertyName] = keyProperties[e.PropertyName].Invoke(sender);
            _HasToString = false;
            _HasHash = false;
        }

        new bool Equals(object obj)
        {
            if(! obj.GetType().IsAssignableFrom(typeof(PrimaryKey)))
            {
                return false;
            }
            return CompareTo((PrimaryKey)obj) == 0;
        }

        public int CompareTo(object obj)
        {
            if (!(obj is PrimaryKey))
            {
                throw new ArgumentException(obj.GetType().BasicName() + " is not of type PrimaryKey");
            }

            return CompareTo((PrimaryKey)obj); 
        }
    }
}