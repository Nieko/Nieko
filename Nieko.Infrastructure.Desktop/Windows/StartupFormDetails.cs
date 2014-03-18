using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.ViewModel;
using Nieko.Infrastructure.ComponentModel;

namespace Nieko.Infrastructure.Windows
{
    /// <summary>
    /// MVVM details of Home screen. The start-up form is responsible for 
    /// displaying and processing <seealso cref="IStartupNotifier"/>
    /// messages
    /// </summary>
    public sealed class StartupFormDetails
    {
        private Func<FormsProvider, ViewModelForm> _StartupFormBuilder = null;

        public static StartupFormDetails Default { get; private set; }

        public ViewModelForm GetAlternative(FormsProvider formsProvider)
        {
            return _StartupFormBuilder(formsProvider);
        }

        private StartupFormDetails() { }

        static StartupFormDetails()
        {
            Default = new StartupFormDetails()
            {
                _StartupFormBuilder = fp => null
            };
        }

        public static StartupFormDetails UseAlternative<TView, TViewModelInterface, TViewModel>()
            where TView : class, IViewBase<TViewModelInterface>, new()
            where TViewModelInterface : IViewModelBase
            where TViewModel : class, TViewModelInterface
        {
            var alternative = new StartupFormDetails()
            {
                _StartupFormBuilder = fp => fp.Get<TView, TViewModelInterface, TViewModel>(EndPoint.Root)
            };

            return alternative;
        }
    }
}
