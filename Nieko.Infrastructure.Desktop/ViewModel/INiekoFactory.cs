using Nieko.Infrastructure.ComponentModel;
using Nieko.Infrastructure.Windows.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Nieko.Infrastructure.ViewModel
{
    public interface INiekoFactory
    {
        Func<T> GetViewFactory<T>();
        Func<T> GetViewModelFactory<T>();
        IGraphFactory ForEndPoint(Expression<Func<EndPoint>> endPoint);
    }
}
