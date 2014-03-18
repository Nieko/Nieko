using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Workflow
{
    public interface IPlan
    {
        IList<IWorkState> WorkActions { get; }
    }
}
}
