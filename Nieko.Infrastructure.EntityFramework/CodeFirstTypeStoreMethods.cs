using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Reflection;

namespace Nieko.Infrastructure.EntityFramework
{
    internal class CodeFirstTypeStoreMethods<T, TClass> : Nieko.Infrastructure.EntityFramework.ICodeFirstTypeStoreMethods
        where TClass : class, T
    {
        private Func<DbContext, DbSet<TClass>> _DbSetRetrieval;

        public void Delete<TEntity>(DbContext context, TEntity item)
        {
            GetSet(context).Remove((TClass)(object)item);
            context.SaveChanges();
        }

        public TEntity GetItem<TEntity>(DbContext context, Expression<Func<TEntity, bool>> filter)
        {
            return (TEntity)(object)GetSet(context).FirstOrDefault((Expression<Func<TClass, bool>>)(object)filter);
        }

        public IQueryable<TEntity> GetItems<TEntity>(DbContext context)
        {
            return (IQueryable<TEntity>)GetSet(context);
        }

        public void Save<TEntity>(DbContext context, TEntity item)
        {
            GetSet(context).Add((TClass)(object)item);
            context.SaveChanges();
        }

        private DbSet<TClass> GetSet(DbContext context)
        {
            if (_DbSetRetrieval == null)
            {
                BuildDbSetRetrieval();
            }

            return _DbSetRetrieval(context);
        }

        private void BuildDbSetRetrieval()
        {
            var methodName = BindingHelper.Name((DbContext context) => context.Set<TClass>());
            var method = typeof(DbContext).GetMethods()
                .First(m => m.Name == methodName && m.IsGenericMethod)
                .MakeGenericMethod(typeof(TClass)); 
            var contextParameter = Expression.Parameter(typeof(DbContext));

            _DbSetRetrieval = Expression.Lambda<Func<DbContext, DbSet<TClass>>>(
                Expression.Call(
                    contextParameter,
                    method),
                contextParameter)
            .Compile();
        }
    }
}
