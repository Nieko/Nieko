using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Navigation.RecordNavigation;
using Nieko.Infrastructure;

namespace Nieko.Infrastructure.Navigation.RecordNavigation
{
    public interface IDataNavigatorFactory
    {
        IDataNavigator CreateInstance(INotifyDisposing viewModel, bool hasView);
    }
}
