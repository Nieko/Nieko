using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Workflow
{
    public interface IWorkState
    {
        object Work { get; }
        object State { get; set;}
        Action<object> ExecutionAction { get; }
    }
}
}
