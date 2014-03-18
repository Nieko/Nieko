using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Nieko.Infrastructure.Navigation;
using Nieko.Infrastructure.ComponentModel;
using Nieko.Infrastructure.EventAggregation;
using System.Xml.Serialization;

namespace Nieko.Infrastructure.Windows
{
    /// <summary>
    /// Base class for a Forms Suite that has a facade for implementing
    /// notifying properties
    /// </summary>
    /// <typeparam name="T">Self reference to inheriting type</typeparam>
    public abstract class NotifyingFormsSuiteBase<T> : FormsSuiteBase<T>, INotifyPropertyChanged
        where T : NotifyingFormsSuiteBase<T>
    {
        protected NotifyingFields Fields { get; private set; }

        [XmlIgnore]
        public override int SelectedTabIndex
        {
            get
            {
                return Fields.Get(() => SelectedTabIndex); 
            }
            set
            {
                Fields.Set(() => SelectedTabIndex, value, TabIndexChanged);
            }
        }

        public NotifyingFormsSuiteBase(IViewNavigator regionNavigator, IInfrastructureEventAggregator eventAggregator, IFormsManager formsManager)
            : base(regionNavigator, eventAggregator, formsManager)
        {
            Fields = new NotifyingFields(this, RaisePropertyChanged);
            Fields.SetDefault(() => SelectedTabIndex, -1);
        }
        
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
  
        protected virtual void TabIndexChanged()
        {
            if (SelectedTabIndex >= 0 && SelectedTabIndex < Forms.Count)
            {
                Forms[SelectedTabIndex].ShowAction();
            }
        }
    }
}
