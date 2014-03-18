using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.ComponentModel
{
    public class NotEditableWrapper<T> : IEditableMirrorObject
    {
        public T Instance { get; set; }

        public Data.PrimaryKey SourceKey { get; set; }

        public bool IsEditing
        {
            get 
            {
                return false;
            }
        }

        public bool HasChanged 
        {
            get
            {
                return false;
            }
            set
            {
                throw new InvalidOperationException("Object is not editable");
            }
        }

        public void BeginEdit()
        {
            throw new InvalidOperationException("Object is not editable");
        }

        public void CancelEdit()
        {
            throw new InvalidOperationException("Object is not editable");
        }

        public void EndEdit()
        {
            throw new InvalidOperationException("Object is not editable");
        }
    }
}