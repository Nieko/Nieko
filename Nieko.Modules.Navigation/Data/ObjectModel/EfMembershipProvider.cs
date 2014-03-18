using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Nieko.Infrastructure.Data;
using Nieko.Infrastructure.Navigation.RecordNavigation;
using Nieko.Infrastructure.Windows.Data;
using Nieko.Infrastructure;

namespace Nieko.Modules.Navigation.Data.ObjectModel
{
    /// <summary>
    /// Membership Provider using an Entity Framework collection 
    /// relationship to represent membership
    /// </summary>
    /// <typeparam name="T">ModelView of membership entity</typeparam>
    /// <typeparam name="TEntity">Membership entity</typeparam>
    /// <typeparam name="TParentEntity">Entity holding membership(s)</typeparam>
    /// <typeparam name="TDataStore">Data Store</typeparam>
    internal abstract class EfMembershipProvider<T, TEntity, TParentEntity, TDataStore> : MembershipProvider<T, TEntity, TParentEntity, TDataStore>, IMembershipProvider
        where T : class, IMembershipProviderLineItem, new()
        where TDataStore : class, IDataStore
        where TEntity : class, new()
    {
        private Data.MembershipPersistence<TParentEntity, TEntity> _MembershipPersistence;

        /// <summary>
        /// Expression indicating collection property on <typeparamref name="TParentEntity"/> that
        /// holds membership entities <typeparamref name="TEntity"/>
        /// </summary>
        protected abstract Expression<Func<TParentEntity, ICollection<TEntity>>> Relationship { get;}

        protected override Data.MembershipPersistence<TParentEntity, TEntity> MembershipPersistence
        {
            get 
            {
                // MembershipPersistance is built from information provided by Relationship property
                if (_MembershipPersistence == null)
                {
                    BuildMembershipPersistence();
                }
                return _MembershipPersistence;
            }
        }

        public EfMembershipProvider(Func<IDataNavigatorOwnerBuilder> builderFactory, IDataStoresManager dataStoresManager, IPersistedView owner)
            : base(builderFactory, dataStoresManager, owner) {}

        private void BuildMembershipPersistence()
        {
            // The fluent parameters for the Membership Persistence Factory are all
            // created from the Relationship Property...

            var factory = new Data.MembershipPersistenceFactory<TParentEntity, TEntity>();

            var parentEntityParam = Expression.Parameter(typeof(TParentEntity), "pe");
            var entityParam = Expression.Parameter(typeof(TEntity), "entity");

            // ...starting with the member accessor, built by compiling a Property Expression
            // using the property name given in Relationship, converting the ICollection<TEntity>
            // return value to an IEnumerable<TEntity>
            var memberAccessor = Expression.Lambda<Func<TParentEntity, IEnumerable<TEntity>>>(
                Expression.PropertyOrField(
                    parentEntityParam,
                    BindingHelper.Name(Relationship)),
                    parentEntityParam);

            var relationshipAccessor = Relationship.Compile();

            // Membership clear and addition actions just use members on the relationship Collection 
            // property
            Action<TParentEntity> clearAction = (parentEntity) => relationshipAccessor(parentEntity).Clear();
            Action<TParentEntity, TEntity> addAction = (parentEntity, member) => relationshipAccessor(parentEntity).Add(member);

            _MembershipPersistence = factory.RetrievingBy(memberAccessor)
                .ClearingBy(clearAction)
                .AddingBy(addAction)
                .Build();
        }
    }
}
