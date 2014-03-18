using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Workflow
{
    public interface IWork<TState>
        where TState : ICloneable
    {
        string Caption { get; }
        void Execute(TState state);
    }
}
}
