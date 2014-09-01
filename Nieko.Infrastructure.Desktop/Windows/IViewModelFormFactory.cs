using Nieko.Infrastructure.ComponentModel;
using Nieko.Infrastructure.Composition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Windows
{
    public interface IViewModelFormFactory
    {
        Func<T> GetViewBuilder<T>();
        Func<T> GetViewModelBuilder<T>();
    }
}
