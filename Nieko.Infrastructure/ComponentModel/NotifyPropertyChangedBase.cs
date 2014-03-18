using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace Nieko.Infrastructure.ComponentModel
{
    public class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        private NotifyingFields _Fields;

        protected void SetDefault<T>(Expression<Func<T>> property, T value)
        {
            if (ShouldInitialize)
            {
                _Fields = new NotifyingFields(this, () => PropertyChanged);
            }

            _Fields.SetDefault(property, value); 
        }

        protected T Get<T>(Expression<Func<T>> property)
        {
            if (ShouldInitialize)
            {
                _Fields = new NotifyingFields(this, () => PropertyChanged);
            }

            return _Fields.Get(property);
        }

        protected void Set<T>(Expression<Func<T>> property, T value)
        {
            if (ShouldInitialize)
            {
                _Fields = new NotifyingFields(this, () => PropertyChanged);
            }

            _Fields.Set(property, value);
        }

        protected void Set<T>(Expression<Func<T>> property, T value, Action valueUpdated)
        {
            if (ShouldInitialize)
            {
                _Fields = new NotifyingFields(this, () => PropertyChanged);
            }

            _Fields.Set(property, value, valueUpdated);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual bool ShouldInitialize
        {
            get
            {
                return _Fields == null;
            }
        }

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected virtual void RaisePropertyChanged<TResult>(Expression<Func<TResult>> propertyExpression)
        {
            RaisePropertyChanged(BindingHelper.Name(propertyExpression));
        }
    }
}
