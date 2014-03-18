using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using Nieko.Infrastructure.ComponentModel;
using System.Runtime.Serialization;
using Nieko.Infrastructure.Windows.Data;

namespace Nieko.Infrastructure.ViewModel
{
    /// <summary>
    /// Base class for implementing <seealso cref="IViewModelBase"/>.
    /// </summary>
    public abstract class ViewModelBase : NotifyPropertyChangedBase, IViewModelBase
    {
        protected bool IsDisposing { get; private set; }

        public abstract void Load();

        public void Dispose() 
        {
            if (IsDisposing)
            {
                return;
            }

            IsDisposing = true;

            DisposeImpl();
            RaiseDisposing();
        }

        protected virtual void DisposeImpl()
        {
        }

        protected override bool ShouldInitialize
        {
            get
            {
                return base.ShouldInitialize && ! IsDisposing;
            }
        }

        public event EventHandler Disposing;

        private void RaiseDisposing()
        {
            var handler = Disposing;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
