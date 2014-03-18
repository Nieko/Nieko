using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Windows.Data
{
    public interface INotifyModelViewGraphNodeLoaded
    {
        event EventHandler Loaded;
    }
}
