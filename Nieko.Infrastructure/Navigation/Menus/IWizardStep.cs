using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Navigation.Menus
{
    public interface IWizardStep
    {
        string Caption { get; }
        bool IsCurrent { get; }
    }
}
}
