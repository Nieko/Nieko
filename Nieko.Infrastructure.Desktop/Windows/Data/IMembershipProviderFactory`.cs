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
    /// Specific graph factory (<seealso cref="IGraphFactory"/>) to manufacture
    /// objects to manage a membership relationship
    /// </summary>
    /// <remarks>
    /// Cannot be called directly (No implementation is registered by the 
    /// container): is provided by (<seealso cref="IGraphFactory"/>) after
    /// constructing the initial tier
    /// </remarks>
    /// <typeparam name="TDataStore">Store to persist memberships</typeparam>
    public interface IMembershipProviderFactory<TDataStore>
        where TDataStore : class, IDataStore
    {
        /// <summary>
        /// Specifies Parent and Child Entities in membership relationship
        /// </summary>
        /// <typeparam name="TParentEntity">Parent Entity</typeparam>
        /// <typeparam name="TEntity">Membership Entity</typeparam>
        /// <returns>Next step in factory</returns>
        IMembershipProviderFactory<TParentEntity, TEntity, TDataStore> ForEntities<TParentEntity, TEntity>()
            where TEntity : class, new();
    }
}
