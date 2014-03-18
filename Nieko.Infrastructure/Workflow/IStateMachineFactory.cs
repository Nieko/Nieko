using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Workflow
{
    public interface IStateMachineFactory
    {
        IStateMachineFactory<TFinalState, TFinalState, TFinalState> Create<TFinalState>() where TFinalState : ICloneable;
    }

    public interface IStateMachineFactory<TFinalState, TPreviousState, TNextState>
        where TFinalState : ICloneable
        where TPreviousState : TFinalState, ICloneable
        where TNextState : TFinalState, ICloneable
    {
        IStateMachineFactory<TFinalState, TNextState, TWorkState> AddWork<TWorkState, TWork>()
            where TWorkState : TFinalState, ICloneable
            where TWork : IWork<TWorkState>, new();

        IStateMachine<TFinalState> Build();
    }
}
}
