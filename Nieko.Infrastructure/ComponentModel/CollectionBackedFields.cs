using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Nieko.Infrastructure.ComponentModel
{
    public class CollectionBackedFields
    {
        public interface ISetup<TItem, T>
        {
            ISetupWithOwner<TItem, T> WithOwner(INotifyPropertyChanged owner, Func<PropertyChangedEventHandler> eventHandler);
            ISetupWithOwner<TItem, T> WithOwner(INotifyPropertyChanged owner, Action<string> raisePropertyChanged);
        }

        public interface ISetupWithOwner<TItem, T> 
        {
            ISetupWithFilter<TItem, T> WithFilter(Func<TItem, string, bool> filter);
        }

        public interface ISetupWithFilter<TItem, T>
        {
            ISetupWithFilter<TItem, T> IsReadOnly(bool readOnly);
            CollectionBackedFields Build();
            CollectionBackedFields Build(Action<T, string> setActions);
        }

        internal class Setup<TItem, T> : 
            ISetup<TItem, T>, 
            ISetupWithOwner<TItem, T>, 
            ISetupWithFilter<TItem, T>
        {
            private CollectionBackedFields _Instance;
            private  bool _IsReadOnly;

            public IEnumerable<TItem> Collection { get; set; }

            public Expression<Func<TItem, T>> ValuePath { get; set; }

            public Func<TItem, string, bool> Filter { get; set; }

            public ISetupWithOwner<TItem, T> WithOwner(INotifyPropertyChanged owner, Func<PropertyChangedEventHandler> eventHandler)
            {
                _Instance = new CollectionBackedFields(owner, eventHandler);

                return this;
            }

            public ISetupWithOwner<TItem, T> WithOwner(INotifyPropertyChanged owner, Action<string> raisePropertyChanged)
            {
                _Instance = new CollectionBackedFields(owner, raisePropertyChanged);

                return this;
            }

            public ISetupWithFilter<TItem, T> WithFilter(Func<TItem, string, bool> filter)
            {
                Filter = filter;

                return this;
            }

            public ISetupWithFilter<TItem, T> IsReadOnly(bool readOnly)
            {
                _IsReadOnly = readOnly;

                return this;
            }

            public CollectionBackedFields Build()
            {
                return Build(null);
            }

            public CollectionBackedFields Build(Action<T, string> setActions)
            {
                var accessor = ValuePath.Compile();
                Action<TItem, T> setter;

                if(_IsReadOnly)
                { 
                    setter = ValuePath.ToSetter().Compile();
                }
                else
                {
                    setter = (i, v) => { };
                }

                _Instance._Getter = key => accessor(Collection.First(i => Filter(i, key)));
                
                if(setActions == null)
                {
                    _Instance._Setter = (key, value) =>
                        {
                            setter(Collection.First(i => Filter(i, key)), (T)value);
                        };
                }
                else
                {
                    _Instance._Setter = (key, value) =>
                    {
                        setter(Collection.First(i => Filter(i, key)), (T)value);
                        setActions(accessor(Collection.First(i => Filter(i, key))), key);
                    };
                }

                return _Instance;
            }
        }

        private INotifyPropertyChanged _Owner;
        private Action<string> _RaisePropertyChanged;

        private Func<string, object> _Getter;
        private Action<string, object> _Setter;

        internal CollectionBackedFields(INotifyPropertyChanged owner, Func<PropertyChangedEventHandler> eventHandler)
            : this(owner, (propertyName) =>
                {
                    var handler = eventHandler();
                    if (handler != null)
                    {
                        handler(owner, new PropertyChangedEventArgs(propertyName));
                    }
                })
        { }

        internal CollectionBackedFields(INotifyPropertyChanged owner, Action<string> raisePropertyChanged)
        {
            _Owner = owner;

            _RaisePropertyChanged = raisePropertyChanged;

            if (_Owner is INotifyDisposing)
            {
                EventHandler ownerDispose = null;

                ownerDispose = (sender, args) =>
                {
                    (_Owner as INotifyDisposing).Disposing -= ownerDispose;

                    _Getter = null;
                    _Setter = null;
                    _RaisePropertyChanged = null;
                };

                (_Owner as INotifyDisposing).Disposing += ownerDispose;
            }
        }

        public T Get<T>(Expression<Func<T>> path)
        {
            var name = BindingHelper.Name(path);

            return (T)Convert.ChangeType(_Getter(name), typeof(T));
        }

        public void Set<T>(Expression<Func<T>> path, T value)
        {
            var name = BindingHelper.Name(path);

            _Setter(name, value);
        }

        public static ISetup<TItem, T> Create<TItem, T>(IEnumerable<TItem> collection, Expression<Func<TItem, T>> accessor)
        {
            return new Setup<TItem, T>()
            {
                Collection = collection,
                ValuePath = accessor
            };
        }
    }
}
