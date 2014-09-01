using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using Nieko.Infrastructure.Data;
using Nieko.Infrastructure.ComponentModel;
using System.Reflection;
using System.Xml.Serialization;

namespace Nieko.Infrastructure.ViewModel
{
    /// <summary>
    /// Base class for a Mirror of a Entity data implementing functionality required
    /// for use as a ModelView with change commit and rollback.
    /// </summary>
    public abstract class EditableViewModel : IEditableDataErrorInfoMirror, INotifyPropertyChanged, IEquatable<EditableViewModel>
    {
        private NotifyingFields _Fields;
        private NotifyingFields _OldFields;
        private IWeakEventRouter _PropertyChangedRouter;
        private PrimaryKey _SourceKey;

        [XmlIgnore]
        public IDictionary<string, IList<Func<string>>> ValidationByColumnName { get; private set; }

        [XmlIgnore]
        public IDictionary<string, string> CurrentErrors { get; private set; }

        protected T Get<T>(Expression<Func<T>> propertyExpression)
        {
            return _Fields.Get(propertyExpression);
        }

        private void SetImpl<T>(Expression<Func<T>> propertyExpression, T value, Action valueUpdated)
        {
            if(SuppressNotifications)
            {
                _Fields.SetDefault(propertyExpression, value);
                return;
            }

            if (IsEditing)
            {
                _Fields.Set(propertyExpression, value, valueUpdated);

                return;
            }

            bool wasChanged = false;
            T oldValue = Get(propertyExpression); 

            _Fields.Set(propertyExpression, value, () =>
                {
                    wasChanged = true;
                    if (valueUpdated != null)
                    {
                        valueUpdated();
                    }
                });

            if (wasChanged && IsEditing)
            {
                _OldFields.SetDefault(propertyExpression, oldValue);  
            }
        }

        protected void Set<T>(Expression<Func<T>> propertyExpression, T value, Action valueUpdated)
        {
            SetImpl(propertyExpression, value, valueUpdated); 
        }

        protected void Set<T>(Expression<Func<T>> propertyExpression, T value)
        {
            SetImpl(propertyExpression, value, null);
        }

        protected void SetDefault<T>(Expression<Func<T>> propertyExpression, T value)
        {
            _Fields.SetDefault(propertyExpression, value); 
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [XmlIgnore]
        public PrimaryKey SourceKey
        {
            get
            {
                return _SourceKey;
            }
            set
            {
                if (_SourceKey == value)
                {
                    return;
                }
                _SourceKey = value;
                if(_SourceKey != null)
                {
                    _SourceKey.TouchKeys(); 
                }
            }
        }

        [XmlIgnore]
        public virtual bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        [XmlIgnore]
        public bool IsEditing { get; private set; }

        [XmlIgnore]
        public bool SuppressNotifications { get; set; }

        [XmlIgnore]
        public bool HasChanged { get; set; }

        public EditableViewModel()
        {
            _Fields = new NotifyingFields(this, () => PropertyChanged);
            ValidationByColumnName = new Dictionary<string, IList<Func<string>>>();
            InitializeValidation();
            CurrentErrors = new Dictionary<string, string>();

            _PropertyChangedRouter = WeakEventRouter.CreateInstance(this, this,
                () => default(PropertyChangedEventArgs),
                (p, d) => p.PropertyChanged += d.Handler,
                (p, d) => p.PropertyChanged -= d.Handler,
                (s, p, a) => s.UpdateErrors(a.PropertyName)); 

            IsEditing = false;
        }

        public virtual void BeginEdit()
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException("Cannot edit; read only");
            }

            if (IsEditing)
            {
                throw new InvalidOperationException("Already editing");
            }

            _OldFields = _Fields.CreateCopy();
            IsEditing = true;
        }

        public virtual void CancelEdit()
        {
            if (!IsEditing)
            {
                throw new InvalidOperationException("Not editing");
            }

            var changes = _Fields.GetDifferences(_OldFields);  

            _Fields = _OldFields;

            foreach (var change in changes)
            {
                RaisePropertyChanged(change); 
            }

            IsEditing = false;
        }

        public virtual void EndEdit()
        {
            _OldFields = null;

            IsEditing = false;
            HasChanged = true;
        }

        public bool Equals(EditableViewModel other)
        {
            if (other.SourceKey == null && SourceKey == null)
            {
                return true;
            }

            if (other.SourceKey == null ^ SourceKey == null)
            {
                return false;
            }

            return other.SourceKey.CompareTo(SourceKey) == 0;   
        }

        public override bool Equals(object obj)
        {
            if(obj is EditableViewModel)
            {
                return Equals((obj as EditableViewModel)); 
            }

            return false;
        }

        public override int GetHashCode()
        {
            if (SourceKey == null)
            {
                return base.GetHashCode();
            }
            return SourceKey.GetHashCode(); 
        }

        public void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));  
            }
        }

        public void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            RaisePropertyChanged(BindingHelper.Name(propertyExpression));  
        }

        public void AddValidation(string propertyName, Func<string> validation)
        {
            IList<Func<string>> validators = null;

            if (!ValidationByColumnName.TryGetValue(propertyName, out validators))
            {
                validators = new List<Func<string>>();
                ValidationByColumnName.Add(propertyName, validators); 
            }

            validators.Add(validation);
        }

        public void AddValidation<T>(Expression<Func<T>> property, Func<string> validation)
        {
            AddValidation(BindingHelper.Name(property), validation);
        }

        public void RecheckForErrors()
        {
            foreach(var propertyName in ValidationByColumnName.Keys)
            {
                UpdateErrors(propertyName);
            }
        }

        protected virtual void InitializeValidation() { }

        protected virtual void UpdateErrors(string propertyName)
        {
            if (!ValidationByColumnName.ContainsKey(propertyName))
            {
                if (CurrentErrors.ContainsKey(propertyName))
                {
                    CurrentErrors.Remove(propertyName);  
                }

                return;
            }

            var error = ValidationByColumnName[propertyName]
                .Select(v => v())
                .FirstOrDefault(e => ! string.IsNullOrEmpty(e));

            if (error == null && CurrentErrors.ContainsKey(propertyName))
            {
                CurrentErrors.Remove(propertyName);
                RaisePropertyChanged(() => Error);

                return;
            }

            CurrentErrors[propertyName] = error;
            RaisePropertyChanged(() => Error);
        }

        public virtual string Error
        {
            get 
            {
                if (CurrentErrors.Count == 0)
                {
                    return string.Empty;
                }

                return CurrentErrors.Values.First();
            } 
        }

        public virtual string this[string columnName]
        {
            get 
            {
                if (CurrentErrors.ContainsKey(columnName))
                {
                    return CurrentErrors[columnName];
                }

                return string.Empty;
            }
        }
    }
}
