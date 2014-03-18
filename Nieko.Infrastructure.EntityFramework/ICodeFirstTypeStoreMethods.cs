using System;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Linq;

namespace Nieko.Infrastructure.EntityFramework
{
    internal interface ICodeFirstTypeStoreMethods
    {
        void Delete<T>(DbContext context, T item);
        T GetItem<T>(DbContext context, Expression<Func<T, bool>> filter);
        IQueryable<T> GetItems<T>(DbContext context);
        void Save<T>(DbContext context, T item);
    }
}
