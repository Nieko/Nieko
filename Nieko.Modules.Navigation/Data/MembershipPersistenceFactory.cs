using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Nieko.Infrastructure.Windows.Data;

namespace Nieko.Modules.Navigation.Data
{
    internal class MembershipPersistenceFactory<TParentEntity, TEntity>
        where TEntity : class, new()
    {
        private Expression<Func<TParentEntity, IEnumerable<TEntity>>> _EntityMemberships;
        private Action<TParentEntity> _ClearEntityMemberships;
        private Action<TParentEntity, TEntity> _AddEntityMembership;

        public MembershipPersistenceFactory() {}

        public MembershipPersistenceFactory<TParentEntity, TEntity> AddingBy(Action<TParentEntity, TEntity> addingAction)
        {
            _AddEntityMembership = addingAction;

            return this;
        }

        public MembershipPersistenceFactory<TParentEntity, TEntity> ClearingBy(Action<TParentEntity> clearingAction)
        {
            _ClearEntityMemberships = clearingAction;

            return this;
        }

        public MembershipPersistenceFactory<TParentEntity, TEntity> RetrievingBy(Expression<Func<TParentEntity, IEnumerable<TEntity>>> membershipExpression)
        {
            _EntityMemberships = membershipExpression;

            return this;
        }

        public MembershipPersistence<TParentEntity, TEntity> Build()
        {
            if (_EntityMemberships == null ||
                _ClearEntityMemberships == null ||
                _AddEntityMembership == null)
            {
                throw new InvalidOperationException("Missing persistence details"); 
            }

            var result = new MembershipPersistence<TParentEntity, TEntity>();

            result.Memberships = _EntityMemberships;
            result.Clear = _ClearEntityMemberships;
            result.Add = _AddEntityMembership;

            return result;
        }
    }
}
