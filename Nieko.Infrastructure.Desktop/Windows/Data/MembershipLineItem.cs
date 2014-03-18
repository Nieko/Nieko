using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.ViewModel;
using Nieko.Infrastructure.Data;
using System.Windows;

namespace Nieko.Infrastructure.Windows.Data
{
    /// <summary>
    /// Base implementation of <seealso cref="IMembershipProviderLineItem"/>
    /// </summary>
    public abstract class MembershipLineItem : EditableViewModel, IMembershipProviderLineItem
    {
        public override bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public Visibility RemoveVisibility
        {
            get
            {
                return Get(() => RemoveVisibility);
            }
            set
            {
                Set(() => RemoveVisibility, value);
            }
        }
    }
}
