using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Data;
using Nieko.Infrastructure.ComponentModel.ViewModelling;
using Nieko.Infrastructure.ComponentModel;
using System.Windows.Data;
using Nieko.Infrastructure.Navigation.RecordNavigation;

namespace Nieko.Infrastructure.Windows.Data
{
    /// <summary>
    /// Step in graph factory to determine Entity details
    /// </summary>
    public interface IOwnedGraphFactory
    {
        /// <summary>
        /// Specifies the persistence classes
        /// </summary>
        /// <typeparam name="TEntity">Entity to map ModelView to</typeparam>
        /// <typeparam name="TDataStore">Store containing TEntity instances</typeparam>
        /// <returns>Next step in factory</returns>
        IEntityGraphFactory<TEntity, TDataStore> ForEntity<TEntity, TDataStore>()
            where TDataStore : class, IDataStore
            where TEntity : class, new();
    }
}