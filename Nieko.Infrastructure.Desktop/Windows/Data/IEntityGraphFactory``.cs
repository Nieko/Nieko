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
    /// <typeparam name="TEntity">Entity to map ModelView to</typeparam>
    /// <typeparam name="TDataStore">Store containing TEntity instances</typeparam>
    public interface IEntityGraphFactory<TEntity, TDataStore>
        where TDataStore : class, IDataStore
        where TEntity : class, new()
    {
        /// <summary>
        /// Specifies the ModelView to map <typeparamref name="TEntity"/> from
        /// </summary>
        /// <typeparam name="T">ModelView</typeparam>
        /// <returns>Next step in factory</returns>
        IFinishedGraphFactory<T, TEntity, TDataStore> WithMirror<T>()
            where T : class, IEditableMirrorObject, new();
    }
}