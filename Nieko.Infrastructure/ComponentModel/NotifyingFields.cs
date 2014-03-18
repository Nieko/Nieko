using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.ComponentModel;
using System.Reflection;

namespace Nieko.Infrastructure.ComponentModel
{
    public class NotifyingFields
    {
        private bool _IsDisposed = false;
        private Action<string> _RaisePropertyChanged;
        private readonly INotifyPropertyChanged _Owner;

        protected Dictionary<string, object> FieldValues { get; private set;}
        protected Dictionary<Type, object> FieldComparers { get; private set;}

        public virtual T Get<T>(Expression<Func<T>> propertyExpression)
        {
            if (_IsDisposed)
            {
                return default(T);
            }

            string propertyName = BindingHelper.Name(propertyExpression);
            object value;
            T result;

            if (!FieldValues.TryGetValue(propertyName, out value))
            {
                result = default(T);
                FieldValues.Add(propertyName, result);

                return result;
            }

            return (T)value;
        }

        private void Set<T>(Expression<Func<T>> propertyExpression, T value, Action valueUpdated, bool raiseEvents)
        {
            if (_IsDisposed)
            {
                return;
            }

            string propertyName = BindingHelper.Name(propertyExpression);
            T currentValue = Get(propertyExpression);
            var comparer = GetFieldComparer<T>();

            if (comparer(currentValue, value))
            {
                return;
            }

            FieldValues[propertyName] = value;

            if (!raiseEvents)
            {
                return;
            }

            _RaisePropertyChanged(propertyName);

            if (valueUpdated != null)
            {
                valueUpdated();
            }
        }

        public virtual void Set<T>(Expression<Func<T>> propertyExpression, T value, Action valueUpdated)
        {
            Set(propertyExpression, value, valueUpdated, true);  
        }

        public virtual void Set<T>(Expression<Func<T>> propertyExpression, T value)
        {
            Set<T>(propertyExpression, value, null);
        }

        public virtual void SetDefault<T>(Expression<Func<T>> propertyExpression, T value)
        {
            Set(propertyExpression, value, null, false);
        }

        public NotifyingFields CreateCopy()
        {
            var copy = new NotifyingFields(_Owner, _RaisePropertyChanged);

            copy.FieldValues = new Dictionary<string, object>(FieldValues);
            copy.FieldComparers = new Dictionary<Type, object>(FieldComparers);

            return copy;
        }

        public NotifyingFields(INotifyPropertyChanged owner, Func<PropertyChangedEventHandler> eventHandler)
            : this(owner, (propertyName) =>
                {
                    var handler = eventHandler();
                    if (handler != null)
                    {
                        handler(owner, new PropertyChangedEventArgs(propertyName));
                    }
                })
        { }

        public NotifyingFields(INotifyPropertyChanged owner, Action<string> raisePropertyChanged)
        {
            FieldValues = new Dictionary<string, object>();
            FieldComparers = new Dictionary<Type, object>();

            _Owner = owner;

            _RaisePropertyChanged = raisePropertyChanged;

            if (_Owner is INotifyDisposing)
            {
                EventHandler ownerDispose = null;

                ownerDispose = (sender, args) =>
                {
                    _IsDisposed = true;
                    (_Owner as INotifyDisposing).Disposing -= ownerDispose;

                    FieldValues.Clear();
                    FieldComparers.Clear();
                    _RaisePropertyChanged = null;
                };

                (_Owner as INotifyDisposing).Disposing += ownerDispose;
            }
        }

        public IEnumerable<string> GetDifferences(NotifyingFields other)
        {
            MethodInfo compareMethod = typeof(NotifyingFields).GetMethod("Compare",BindingFlags.Instance | BindingFlags.NonPublic);

            Func<NotifyingFields, NotifyingFields, IEnumerable<string>> comparer = (left, right) =>
                {
                    return left.FieldValues
                        .Where(entry =>
                            {
                                if (!right.FieldValues.ContainsKey(entry.Key))
                                {
                                    return false;
                                }

                                object rightValue = right.FieldValues[entry.Key];

                                if (entry.Value == null && rightValue == null)
                                {
                                    return false;
                                }

                                Type fieldType = entry.Value == null ? rightValue.GetType() : entry.Value.GetType();

                                return !(bool)compareMethod.MakeGenericMethod(fieldType).Invoke(this, new object[] { entry.Value, rightValue });
                            })
                        .Select(entry => entry.Key);
                };

            return comparer(this, other)
                .Union(comparer(other, this))
                .Distinct();
        }

        private Func<T, T, bool> GetFieldComparer<T>()
        {
            object compiledComparer;
            Func<T, T, bool> comparer;

            if (!FieldComparers.TryGetValue(typeof(T), out compiledComparer))
            {
                var left = System.Linq.Expressions.Expression.Parameter(typeof(T), "x");
                var right = System.Linq.Expressions.Expression.Parameter(typeof(T), "y");

                comparer = System.Linq.Expressions.Expression.Lambda<Func<T, T, bool>>(
                    System.Linq.Expressions.Expression.Equal(left, right),
                    left,
                    right).Compile();

                FieldComparers.Add(typeof(T), comparer);
            }
            else
            {
                comparer = (Func<T, T, bool>)compiledComparer;
            }

            return comparer;
        }

        private bool Compare<T>(T left, T right)
        {
            return GetFieldComparer<T>()(left, right);
        }
    }
}
