using System;
using Nieko.Infrastructure.ViewModel;

namespace Nieko.Infrastructure.Windows.Background
{
    public interface IDesktopViewModel : IViewModelBase
    {
        string ApplicationName { get; }
    }
}
