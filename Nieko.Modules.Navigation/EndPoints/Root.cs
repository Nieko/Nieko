using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Windows;
using Nieko.Infrastructure.Windows.Background;
using Nieko.Infrastructure.ComponentModel;
using Nieko.Infrastructure.Composition;
using Nieko.Infrastructure;

namespace Nieko.Modules.Navigation.EndPoints
{
    public class Root : FormsProvider
    {
        private StartupFormDetails _StartupForm;

        public ViewModelForm InitialForm
        {
            get
            {
                var form = _StartupForm.GetAlternative(this);

                if (form == null)
                {
                    form = Get<Desktop, IDesktopViewModel, DesktopViewModel>(EndPoint.Root);
                }

                return form;
            }
        }

        public Root(IViewModelFormFactory factory, IApplicationDetails appDetails)
            : base(factory)
        {
            _StartupForm = appDetails.StartupForm;
        }
    }
}
