using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Nieko.Infrastructure.ComponentModel
{
    public class Repositioner<T>
        where T : class, INotifyPropertyChanged
    {
        private IEnumerable<T> _StronglyTypedItems;
        private INotifyCollectionChanged _Items;
        private IWeakEventRouter _CollectionRouter;
        private Func<T, object> _Getter;
        private Action<T, object> _Setter;

        public Action<T, bool> CanMoveUpSetter { get; set; }

        public Action<T, bool> CanMoveDownSetter { get; set; }

        private Repositioner(INotifyDisposing owner, INotifyCollectionChanged items, Func<T, object> getter, Action<T, object> setter)
        {
            if(items == null)
            {
                throw new ArgumentNullException("items");
            }

            if (getter == null)
            {
                throw new ArgumentNullException("getter");
            }

            if (setter == null)
            {
                throw new ArgumentNullException("setter");
            }

            if(!(items is System.Collections.IEnumerable))
            {
                throw new ArgumentException("Parameter items does not implement IEnumerable");
            }

            if (items is IEnumerable<T>)
            {
                _StronglyTypedItems = (IEnumerable<T>)items;
            }

            owner.Disposing +=OwnerDisposing;
            _Items = items;
            _CollectionRouter = WeakEventRouter.CreateInstance(this,
                items,
                () => default(NotifyCollectionChangedEventArgs),
                (a, d) => a.CollectionChanged += d.Handler,
                (a, d) => a.CollectionChanged -= d.Handler,
                CollectionChanged);
            _Getter = getter;
            _Setter = setter;
        }

        public static Repositioner<T> Create<TOrdinal>(INotifyDisposing owner, INotifyCollectionChanged items, Expression<Func<T, TOrdinal>> ordinalProperty)
        {
            var getterMethod = ordinalProperty.Compile();
            var setterMethod = ordinalProperty.ToSetter().Compile();
            Func<T, object> getter = o => (object)getterMethod(o);
            Action<T, object> setter = (obj, ord) => setterMethod(obj, (TOrdinal)ord);

            return new Repositioner<T>(owner, items, getter, setter);
        }

        public void MoveUp(T item)
        {
            var ordinal = GetOrdinal(item);

            foreach(var affected in GetItems()
                .Select(i => new { Ordinal = GetOrdinal(i), Item = i })
                .Where(d => d.Ordinal > ordinal))
            {
                if(affected.Ordinal == (ordinal + 1))
                {
                    _Setter(affected.Item, affected.Ordinal - 1);
                }
                else
                {
                    _Setter(affected.Item, affected.Ordinal + 1);
                }
            }

            _Setter(item, ordinal + 1);
        }

        public void MoveDown(T item)
        {
            var ordinal = GetOrdinal(item);

            foreach (var affected in GetItems()
                .Select(i => new { Ordinal = GetOrdinal(i), Item = i })
                .Where(d => d.Ordinal < ordinal))
            {
                if (affected.Ordinal == (ordinal - 1))
                {
                    _Setter(affected.Item, affected.Ordinal + 1);
                }
                else
                {
                    _Setter(affected.Item, affected.Ordinal - 1);
                }
            }

            _Setter(item, ordinal - 1);
        }

        private static void CollectionChanged(Repositioner<T> subscriber, INotifyCollectionChanged publisher, NotifyCollectionChangedEventArgs args)
        {
            if(args.Action ==  NotifyCollectionChangedAction.Add)
            {
                var maxOrdinal = subscriber.GetItems()
                    .Max(i => subscriber.GetOrdinal(i));

                foreach(var newItem in args.NewItems
                    .Cast<T>())
                {
                    maxOrdinal++;
                    subscriber._Setter(newItem, maxOrdinal);
                }
            }
            else
            {
                subscriber.Reorder();
            }
        }

        public void Reorder()
        {
            int previous = 0;
            int current = 0;
            T lastItem = null;
            T firstItem = null;

            var orderedItems = GetItems()
                .OrderBy(_Getter);

            foreach (var item in orderedItems)
            {
                firstItem = firstItem ?? item;

                current = GetOrdinal(item);
                if (current != previous + 1)
                {
                    _Setter(item, previous + 1);
                }
                previous++;
                lastItem = item;
            }

            if (firstItem != null && CanMoveDownSetter != null)
            {
                CanMoveDownSetter(firstItem, false);
            }

            if (lastItem != null && CanMoveUpSetter != null)
            {
                CanMoveUpSetter(firstItem, false);
            }
        }

        public int GetOrdinal(T item)
        {
            var objectValue = _Getter(item);
            
            if (objectValue is int)
            {
                return (int)objectValue;
            }
            else
            {
                var stringValue = (objectValue ?? "0").ToString();
                return int.Parse(stringValue.Trim() == string.Empty ?
                    "0" :
                    stringValue);
            }
        }

        public IEnumerable<T> GetItems()
        {
            return (_StronglyTypedItems ?? (_Items as System.Collections.IEnumerable).Cast<T>());
        }

        private void OwnerDisposing(object sender, EventArgs e)
        {
            _StronglyTypedItems = null;
            _Items = null;
            _CollectionRouter.CancelSubscription(); 
        }
    }
}
