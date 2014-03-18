using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Nieko.Infrastructure.Composition
{
    public interface IContainerModule
    {
        DependantModule Dependant { get; }
        IContainerAdapter Container { get; set; }
        void Initialize();
    }
}
