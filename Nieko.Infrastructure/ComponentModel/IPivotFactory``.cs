using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Nieko.Infrastructure.ComponentModel
{
    public interface IPivotFactory<TFrom, TTo>
    {
        IPivotFactoryWithSource<TFrom, TTo, TObject> WithSource<TObject>(Func<TTo, IEnumerable<TObject>> toSource);
    }

    public interface IPivotFactoryWithSource<TFrom, TTo, TObject>
    {
        IPivotFactoryWithPivot<TFrom, TTo, TObject> PivotBy(Func<string, Func<TObject, bool>> toFilter);
    }

    public interface IPivotFactoryWithPivot<TFrom, TTo, TObject>
    {
        IPivotFactoryWithObjectAccessor<TFrom, TTo, TObject, TValue> ObjectAccessor<TValue>(Expression<Func<TObject, TValue>> objectProperty);
    }

    public interface IPivotFactoryWithObjectAccessor<TFrom, TTo, TObject, TValue> : IMapperPivot<TFrom, TTo>
    {
        IPivotFactoryWithObjectAccessor<TFrom, TTo, TObject, TValue> ConvertBy<TColumn>(Func<TValue, TColumn> from, Func<TColumn, TValue> to);

        IPivotFactoryWithObjectAccessor<TFrom, TTo, TObject, TValue> Add(Expression<Func<TFrom, TValue>> column);

        IPivotFactoryWithObjectAccessor<TFrom, TTo, TObject, TValue> AddConvert<TColumn>(Expression<Func<TFrom, TColumn>> column);

        IPivotFactoryWithObjectAccessor<TFrom, TTo, TObject, TValue> Add<TColumn>(Expression<Func<TFrom, TColumn>> column, Func<TValue, TColumn> from, Func<TColumn, TValue> to);
    }
}
