using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Windows.Data
{
    public enum PersistedViewPersistAction
    {
        AfterSave,
        BeforeDelete
    }

    public class PersistedViewPersistingEventArgs : EventArgs
    {
        public object LineItem { get; private set; }

        public object Entity { get; private set; }

        public PersistedViewPersistAction Action { get; private set; }

        public PersistedViewPersistingEventArgs(object lineItem, object entity, PersistedViewPersistAction action)
        {
            LineItem = lineItem;
            Entity = entity;
            Action = action;
        }
    }
}
