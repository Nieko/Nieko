using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Reflection;

namespace Nieko.Infrastructure.Data
{
    internal class GeneratedStoreMergeOptionUpdater
    {
        private DataStoreMethodsFinder _MethodsFinder;

        public GeneratedStoreMergeOptionUpdater(DataStoreMethodsFinder methodsFinder)
        {
            _MethodsFinder = methodsFinder;
        }

        public void SetMergeOption<TContext>(TContext context, MergeOption mergeOption)
            where TContext : ObjectContext
        {
            var collectionSetMethod = typeof(GeneratedStoreMergeOptionUpdater).GetMethod("SetCollectionMergeOption", BindingFlags.Instance | BindingFlags.NonPublic);

            foreach (var collectionType in _MethodsFinder.EntityCollectionsByType.Keys)
            {
                collectionSetMethod.MakeGenericMethod(typeof(TContext), collectionType).Invoke(this, new object[] { context, mergeOption });  
            }
        }

        private void SetCollectionMergeOption<TContext, T>(TContext context, MergeOption mergeOption)
            where TContext : ObjectContext
        {
            var accessor = (Func<TContext, ObjectQuery<T>>)_MethodsFinder.EntityCollectionsByType[typeof(T)];

            accessor(context).MergeOption = mergeOption;
        }
    }
}
