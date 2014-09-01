#if DEBUG
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Nieko.Infrastructure.Windows.Data.SampleData
{
    public class MembershipProviderSample<T> : PersistedViewSample<T>, IMembershipProvider<T>
        where T : class, IMembershipProviderLineItem, new() 
    {
        public event EventHandler Reset = delegate { };

        public object NewItem
        {
            get
            {
                return (this as IMembershipProvider<T>).NewItem;
            }
            set
            {
                (this as IMembershipProvider<T>).NewItem = (T)value;
            }
        }

        public object Options
        {
            get { return AvailableMemberships; }
        }

        public ICommand AddCommand { get; private set; }

        public bool AddEnabled { get; protected set; }

        public ICommand RemoveCommand { get; private set; }

        public ObservableCollection<T> AvailableMemberships { get; private set; }

        T IMembershipProvider<T>.NewItem { get; set; }

        public Func<IMembershipProvider<T>, bool> RemoveAllowed { get; set; }

        public MembershipProviderSample(IEnumerable<T> options)
        {
            AvailableMemberships = new ObservableCollection<T>(options);
            NewItem = new T();
            AddEnabled = true;
            RemoveAllowed = o => true;
        }
    }
}
#endif