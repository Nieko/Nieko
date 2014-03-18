using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Nieko.Infrastructure.Collections
{
    public class ReadOnlyDictionary<TKey> : IDictionary<TKey, object>
    {
        private IDictionary _Source;
        private Func<TKey, bool> _ContainsKey;
        private Func<TKey, KeyValuePair<object, bool>> _TryGetValue;
        private Func<IEnumerator<KeyValuePair<TKey, object>>> _GetEnumerator;

        public void Add(TKey key, object value)
        {
            throw new InvalidOperationException();
        }

        public bool ContainsKey(TKey key)
        {
            return _ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get
            {
                return (ICollection<TKey>)_Source.Keys;
            }
        }

        public bool Remove(TKey key)
        {
            throw new InvalidOperationException();
        }

        public bool TryGetValue(TKey key, out object value)
        {
            var result = _TryGetValue(key);

            if (result.Value)
            {
                value = result.Key;
                return true;
            }

            value = null;
            return false;
        }

        public ICollection<object> Values
        {
            get
            {
                return new List<object>(_Source.Values
                    .Cast<object>());
            }
        }

        public object this[TKey key]
        {
            get
            {
                return _Source[key];
            }
            set
            {
                throw new InvalidOperationException();
            }
        }

        private ReadOnlyDictionary() { }

        public static ReadOnlyDictionary<TKey> Create<TValue>(Dictionary<TKey, TValue> source)
        {
            var instance = new ReadOnlyDictionary<TKey>();

            instance._Source = source;
            instance._ContainsKey = k => source.ContainsKey(k);
            instance._TryGetValue = k =>
                {
                    TValue value;
                    var result = source.TryGetValue(k, out value);

                    return new KeyValuePair<object, bool>(result ? value : default(TValue), result);
                };
            instance._GetEnumerator = () =>
                {
                    return source
                        .Select(kvp => new KeyValuePair<TKey, object>(kvp.Key, kvp.Value))
                        .GetEnumerator();
                };

            return instance;
        }

        public void Add(KeyValuePair<TKey, object> item)
        {
            throw new InvalidOperationException();
        }

        public void Clear()
        {
            throw new InvalidOperationException();
        }

        public bool Contains(KeyValuePair<TKey, object> item)
        {
            return _Source.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, object>[] array, int arrayIndex)
        {
            _Source.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                return _Source.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return _Source.IsReadOnly;
            }
        }

        public bool Remove(KeyValuePair<TKey, object> item)
        {
            throw new InvalidOperationException();
        }

        public IEnumerator<KeyValuePair<TKey, object>> GetEnumerator()
        {
            return _GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (_Source as System.Collections.IEnumerable).GetEnumerator();
        }
    }
}
