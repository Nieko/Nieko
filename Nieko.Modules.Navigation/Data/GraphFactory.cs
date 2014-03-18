using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.ComponentModel;
using Nieko.Infrastructure.Data;
using Nieko.Infrastructure.Navigation.RecordNavigation;
using System.Linq.Expressions;
using System.Windows.Data;
using System.ComponentModel;
using Nieko.Infrastructure.Windows.Data;
using Nieko.Infrastructure;

namespace Nieko.Modules.Navigation.Data
{
    /// <summary>
    /// Initial steps of graph factory specifying which object determines
    /// the life-time for the ModelView graph
    /// </summary>
    public class GraphFactory : IGraphFactory, IOwnedGraphFactory
    {
        private INotifyDisposing _Owner;
        private Func<IPersistedViewRoot> _RootSupplier;
        private Func<IDataNavigatorOwnerBuilder> _OwnerBuilder;
        private IDataStoresManager _DataStoresManager;

        public GraphFactory(Func<IPersistedViewRoot> rootSupplier, Func<IDataNavigatorOwnerBuilder> ownerBuilder, IDataStoresManager dataStoresManager)
        {
            _RootSupplier = rootSupplier;
            _OwnerBuilder = ownerBuilder;
            _DataStoresManager = dataStoresManager;
        }

        public IOwnedGraphFactory OwnedBy(INotifyDisposing owner)
        {
            _Owner = owner;

            return this;
        }

        public IEntityGraphFactory<TEntity, TDataStore> ForEntity<TEntity, TDataStore>()
            where TDataStore : class, IDataStore
            where TEntity : class, new()
        {
            var nextStage = new EntityGraphFactory<TEntity, TDataStore>();
            var root = _RootSupplier();

            root.Root = _Owner;
            nextStage.Root = root;
            nextStage.BuilderFactory = _OwnerBuilder;
            nextStage.DataStoresManager = _DataStoresManager;

            return nextStage;
        }
    }
}
