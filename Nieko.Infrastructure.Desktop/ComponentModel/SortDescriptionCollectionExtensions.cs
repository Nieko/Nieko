using Nieko.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace System
{
    public static class SortDescriptionCollectionExtensions
    {
        public static SortDescriptionCollection Add<T>(this SortDescriptionCollection collection, Expression<Func<T, object>> property, ListSortDirection direction)
        {
            collection.Add(new SortDescription(BindingHelper.Name(property), direction));

            return collection;
        }

        public static SortDescriptionCollection Add<T>(this SortDescriptionCollection collection, Expression<Func<T, object>> property)
        {
            Add(collection, property, ListSortDirection.Ascending);

            return collection;
        }
    }
}
