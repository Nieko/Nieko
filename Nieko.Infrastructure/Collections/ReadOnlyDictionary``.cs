using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Collections
{
    /// <summary>
    /// Read only wrapper for an IDictionary instance
    /// </summary>
    /// <typeparam name="TKey">Unique Key type</typeparam>
    /// <typeparam name="TValue">Keyed value type</typeparam>
    public class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private IDictionary<TKey, TValue> _Source;

        public void Add(TKey key, TValue value)
        {
            throw new InvalidOperationException(); 
        }

        public bool ContainsKey(TKey key)
        {
            return _Source.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get 
            {
                return _Source.Keys;
            }
        }

        public bool Remove(TKey key)
        {
            throw new InvalidOperationException();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _Source.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values
        {
            get 
            {
                return _Source.Values;
            }
        }

        public TValue this[TKey key]
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

        public ReadOnlyDictionary(IDictionary<TKey, TValue> source)
        {
            _Source = source;
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            throw new InvalidOperationException();
        }

        public void Clear()
        {
            throw new InvalidOperationException();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _Source.Contains(item); 
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
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

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new InvalidOperationException();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _Source.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (_Source as System.Collections.IEnumerable).GetEnumerator();
        }
    }
}