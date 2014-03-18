using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Nieko.Infrastructure.ComponentModel;
using Nieko.Infrastructure.Windows.Data;
using Nieko.Infrastructure.Data;

namespace Nieko.Infrastructure.Windows.Data
{
    /// <summary>
    /// Step in membership graph factory to determine
    /// relationship details
    /// </summary>
    /// <typeparam name="TParentEntity">Parent Entity</typeparam>
    /// <typeparam name="TEntity">Membership entity</typeparam>
    /// <typeparam name="TDataStore">Store for parent and member entities</typeparam>
    public interface IMembershipProviderFactory<TParentEntity, TEntity, TDataStore>
        where TEntity : class, new()
        where TDataStore : class, IDataStore
    {
        /// <summary>
        /// Specifies accessor to membership relationship property
        /// </summary>
        /// <param name="relationship">Membership collection accessor</param>
        /// <returns>Next step in factory</returns>
        IMembershipProviderFactoryWithRelationship<TParentEntity, TEntity, TDataStore> WithRelationship(Expression<Func<TParentEntity, ICollection<TEntity>>> relationship);
    }
}