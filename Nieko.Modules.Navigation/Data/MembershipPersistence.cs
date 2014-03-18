using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Nieko.Infrastructure.Windows.Data;

namespace Nieko.Modules.Navigation.Data
{
    internal class MembershipPersistence<TParentEntity, TEntity>
        where TEntity : class, new()
    {
        public Expression<Func<TParentEntity, IEnumerable<TEntity>>> Memberships { get; internal set; }
        public Action<TParentEntity> Clear { get; internal set; }
        public Action<TParentEntity, TEntity> Add { get; internal set; }

        internal MembershipPersistence() { }
    }
}
