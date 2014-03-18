using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Data;
using Nieko.Infrastructure.ComponentModel;
using Nieko.Infrastructure.Navigation.RecordNavigation;
using Nieko.Infrastructure.Windows.Data;

namespace Nieko.Modules.Navigation.Data
{
    internal class EntityGraphFactory<TEntity, TDataStore> : IEntityGraphFactory<TEntity, TDataStore>
        where TDataStore : class, IDataStore
        where TEntity : class, new()
    {
        internal IPersistedViewRoot Root { get; set; }

        internal Func<IDataNavigatorOwnerBuilder> BuilderFactory { get; set; }

        internal IDataStoresManager DataStoresManager { get; set; }

        public IFinishedGraphFactory<T, TEntity, TDataStore> WithMirror<T>()
            where T : class, IEditableMirrorObject, new()
        {
            var nextStage = new FinishedGraphFactory<T, TEntity, TDataStore>();

            nextStage.Root = Root;
            nextStage.BuilderFactory = BuilderFactory;
            nextStage.DataStoresManager = DataStoresManager;

            return nextStage;
        }
    }
}
