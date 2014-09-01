using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Nieko.Infrastructure.ComponentModel
{
    internal class PivotFactory<TFrom, TTo> : IPivotFactory<TFrom, TTo>
    {
        internal Action<IMapperPivot<TFrom, TTo>> FinishAction;

        public IPivotFactoryWithSource<TFrom, TTo, TObject> WithSource<TObject>(Func<TTo, IEnumerable<TObject>> toSource)
        {
            return new PivotFactory<TFrom, TTo, TObject>()
            {
                FinishAction = FinishAction,
                ToSource = toSource
            };
        }
    }

    internal class PivotFactory<TFrom, TTo, TObject> : PivotFactory<TFrom, TTo>,
        IPivotFactoryWithSource<TFrom, TTo, TObject>,
        IPivotFactoryWithPivot<TFrom, TTo, TObject>
    {
        internal Func<TTo, IEnumerable<TObject>> ToSource;
        internal Func<string, Func<TObject, bool>> ToFilter;

        public IPivotFactoryWithPivot<TFrom, TTo, TObject> PivotBy(Func<string, Func<TObject, bool>> toFilter)
        {
            ToFilter = toFilter;

            return this;
        }

        public IPivotFactoryWithObjectAccessor<TFrom, TTo, TObject, TValue> ObjectAccessor<TValue>(Expression<Func<TObject, TValue>> objectProperty)
        {
            return new PivotFactory<TFrom, TTo, TObject, TValue>()
            {
                FinishAction = FinishAction,
                ToSource = ToSource,
                ToFilter = ToFilter,
                ObjectSetter = objectProperty.ToSetter().Compile(),
                ObjectGetter = objectProperty.Compile()
            };
        }
    }

    internal class PivotFactory<TFrom, TTo, TObject, TValue> : PivotFactory<TFrom, TTo, TObject>,
        IPivotFactoryWithObjectAccessor<TFrom, TTo, TObject, TValue>
    {
        private Dictionary<Type, object> _ConvertersFrom = new Dictionary<Type, object>();
        private Dictionary<Type, object> _ConvertersTo = new Dictionary<Type, object>();
        private List<Action<TFrom, TTo>> _PivotsFrom = new List<Action<TFrom, TTo>>();
        private List<Action<TFrom, TTo>> _PivotsTo = new List<Action<TFrom, TTo>>();

        internal Action<TObject, TValue> ObjectSetter;
        internal Func<TObject, TValue> ObjectGetter;

        public IPivotFactoryWithObjectAccessor<TFrom, TTo, TObject, TValue> Add(Expression<Func<TFrom, TValue>> column)
        {
            Add(column, v => v, v => v);

            return this;
        }

        public IPivotFactoryWithObjectAccessor<TFrom, TTo, TObject, TValue> Add<TColumn>(Expression<Func<TFrom, TColumn>> column, Func<TValue, TColumn> from, Func<TColumn, TValue> to)
        {
            var setter = column.ToSetter().Compile();
            var getter = column.Compile();
            var path = BindingHelper.Name(column);

            Action<TFrom, TTo> fromAction = (f, t) =>
                {
                    var items = ToSource(t);
                    var item = items
                        .First(o => ToFilter(path)(o));
                    var itemValue = ObjectGetter(item);
                    var columnValue = from(itemValue);

                    setter(f, columnValue);
                };

            Action<TFrom, TTo> toAction = (f, t) =>
                {
                    var columnValue = getter(f);
                    var itemValue = to(columnValue);
                    var items = ToSource(t);
                    var item = items
                        .First(o => ToFilter(path)(o));

                    ObjectSetter(item, itemValue);
                };

            _PivotsFrom.Add(fromAction);
            _PivotsTo.Add(toAction);

            return this;
        }

        public void From(TFrom from, TTo to)
        {
            foreach(var action in _PivotsFrom)
            {
                action(from, to);
            }
        }

        public void To(TFrom from, TTo to)
        {
            foreach (var action in _PivotsTo)
            {
                action(from, to);
            }
        }

        public IPivotFactoryWithObjectAccessor<TFrom, TTo, TObject, TValue> ConvertBy<TColumn>(Func<TValue, TColumn> from, Func<TColumn, TValue> to)
        {
            _ConvertersFrom.Add(typeof(TColumn), from);
            _ConvertersTo.Add(typeof(TColumn), to);

            return this;
        }

        public IPivotFactoryWithObjectAccessor<TFrom, TTo, TObject, TValue> AddConvert<TColumn>(Expression<Func<TFrom, TColumn>> column)
        {
            return Add<TColumn>(column, (Func<TValue, TColumn>)_ConvertersFrom[typeof(TColumn)], (Func<TColumn, TValue>)_ConvertersTo[typeof(TColumn)]);
        }
    }
}
