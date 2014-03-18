#if DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Windows.Data;
using System.Windows;
using Nieko.Infrastructure.Data;
using System.ComponentModel;

namespace Nieko.Infrastructure.Styles.SampleData
{
    public class MembershipProviderLineItemSample : IMembershipProviderLineItem
    {
        public Visibility RemoveVisibility { get; set; }

        public PrimaryKey SourceKey { get; set; }

        public bool IsEditing { get { return false; } }

        public bool HasChanged { get; set; }

        public void BeginEdit() { }

        public void CancelEdit() { }

        public void EndEdit() { }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public MembershipProviderLineItemSample()
        {
            RemoveVisibility = Visibility.Visible;
            HasChanged = false;
        }
    }
}
#endif