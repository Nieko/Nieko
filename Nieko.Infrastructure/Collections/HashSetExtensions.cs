using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Collections.Generic
{
    public static class HashSetExtensions
    {
        /// <summary>
        /// Fluent extension for Sets facilitating easy addition of multiple items
        /// </summary>
        /// <typeparam name="T">Set item type</typeparam>
        /// <param name="hashSet">Instance to add items to</param>
        /// <param name="items">Items to add</param>
        public static void AddRange<T>(this ISet<T> hashSet, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                hashSet.Add(item);
            }
        }
    }
}
