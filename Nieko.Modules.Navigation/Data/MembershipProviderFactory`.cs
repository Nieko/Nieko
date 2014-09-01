using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Windows.Data;
using Nieko.Infrastructure.Data;
using Nieko.Infrastructure.ComponentModel;
using Nieko.Infrastructure.Navigation.RecordNavigation;
using System.Linq.Expressions;
using Nieko.Modules.Navigation.Data.ObjectModel;

namespace Nieko.Modules.Navigation.Data
{
    internal class MembershipProviderFactory<TDataStore> : IMembershipProviderFactory<TDataStore>
        where TDataStore : class, IDataStore
    {
        private IPersistedView _Owner;
        private Func<ITierCoordinatorBuilder> _Builder;
        private IDataStoresManager _DataStoresManager;

        public MembershipProviderFactory(IPersistedView owner, Func<ITierCoordinatorBuilder> builder, IDataStoresManager dataStoresManager)
        {
            _Owner = owner;
            _Builder = builder;
            _DataStoresManager = dataStoresManager;
        }

        public IMembershipProviderFactory<TParentEntity, TEntity, TDataStore> ForEntities<TParentEntity, TEntity>() where TEntity : class, new()
        {
            return new MembershipProviderFactory<TParentEntity, TEntity, TDataStore>(_Owner, _Builder, _DataStoresManager);
        }
    }

    internal class MembershipProviderFactory<TParentEntity, TEntity, TDataStore> :
        IMembershipProviderFactory<TParentEntity, TEntity, TDataStore>,
        IMembershipProviderFactoryWithRelationship<TParentEntity, TEntity, TDataStore>
        where TEntity : class, new()
        where TDataStore : class, IDataStore
    {
        private IPersistedView _Owner;
        private Expression<Func<TParentEntity, ICollection<TEntity>>> _Relationship;
        private Func<ITierCoordinatorBuilder> _Builder;
        private IDataStoresManager _DataStoresManager;

        public MembershipProviderFactory(IPersistedView owner, Func<ITierCoordinatorBuilder> builder, IDataStoresManager dataStoresManager)
        {
            _Owner = owner;
            _Builder = builder;
            _DataStoresManager = dataStoresManager;
        }

        public IMembershipProviderFactoryWithRelationship<TParentEntity, TEntity, TDataStore> WithRelationship(Expression<Func<TParentEntity, ICollection<TEntity>>> relationship)
        {
            _Relationship = relationship;

            return this;
        }

        public IMembershipProviderFactory<T, TParentEntity, TEntity, TDataStore> ForLinesOf<T>()
            where T : class, IMembershipProviderLineItem, new()
        {
            var result = InitializingTypeMapperBy<T>(o => o.ImplyAll());

            return result;
        }

        public IMembershipProviderFactory<T, TParentEntity, TEntity, TDataStore> InitializingTypeMapperBy<T>(Action<ITypeMapper<T, TEntity>> initializer)
            where T : class, IMembershipProviderLineItem, new()
        {
            var result = new MembershipProviderFactory<T, TParentEntity, TEntity, TDataStore>(_Owner, _Relationship, _Builder, _DataStoresManager, initializer);

            return result;
        }
    }

    internal class MembershipProviderFactory<T, TParentEntity, TEntity, TDataStore> : IMembershipProviderFactory<T, TParentEntity, TEntity, TDataStore>
        where T : class, IMembershipProviderLineItem, new()
        where TEntity : class, new()
        where TDataStore : class, IDataStore
    {
        private IPersistedView _Owner;
        private Expression<Func<TParentEntity, ICollection<TEntity>>> _Relationship;
        private Func<ITierCoordinatorBuilder> _Builder;
        private IDataStoresManager _DataStoresManager;
        private Action<ITypeMapper<T, TEntity>> _TypeMapperInitializer;

        public class ConstructedMembershipProvider : EfMembershipProvider<T, TEntity, TParentEntity, TDataStore>
        {
            private ITypeMapper<T, TEntity> _TypeMapper;

            internal Expression<Func<TParentEntity, ICollection<TEntity>>> RelationshipInstance { get; set; }

            internal ConstructedMembershipProvider(Func<ITierCoordinatorBuilder> builderFactory, IDataStoresManager dataStoresManager, IPersistedView owner, ITypeMapper<T, TEntity> typeMapper)
                : base(builderFactory, dataStoresManager, owner) 
            {
                _TypeMapper = typeMapper;
            }

            protected override Expression<Func<TParentEntity, ICollection<TEntity>>> Relationship
            {
                get
                {
                    return RelationshipInstance;
                }
            }

            protected override ITypeMapper<T, TEntity> TypeMapper
            {
                get
                {
                    return _TypeMapper;
                }
            }
        }

        public MembershipProviderFactory(IPersistedView owner, Expression<Func<TParentEntity, ICollection<TEntity>>> relationship, Func<ITierCoordinatorBuilder> builder, IDataStoresManager dataStoresManager, Action<ITypeMapper<T, TEntity>> initializer)
        {
            _Owner = owner;
            _Relationship = relationship;
            _Builder = builder;
            _DataStoresManager = dataStoresManager;
            _TypeMapperInitializer = initializer;
        }

        public IMembershipProvider<T> Build(Func<IMembershipProvider<T>, bool> removeAllowed)
        {
            var typeMapper = Nieko.Infrastructure.ComponentModel.TypeMapper.FromOnly()
                    .New<T, TEntity>();
            ConstructedMembershipProvider result;

            _TypeMapperInitializer(typeMapper);
            result = new ConstructedMembershipProvider(_Builder, _DataStoresManager, _Owner, typeMapper);
            result.RelationshipInstance = _Relationship;
            if (removeAllowed != null)
            {
                result.RemoveAllowed = removeAllowed;
            }

            return result;
        }

        public IMembershipProvider<T> Build()
        {
            return Build(null);
        }
    }
}
