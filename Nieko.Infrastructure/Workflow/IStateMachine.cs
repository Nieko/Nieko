using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Nieko.Infrastructure.Workflow
{
    public interface IStateMachine<TState> : INotifyPropertyChanged
        where TState : ICloneable
    {
        IPlan ExecutionPlan { get; set; }
        IWorkState CurrentWorkState { get; }
        bool BackAllowed { get; }
        bool NextAllowed { get; }
        void Start(TState initialState, Action<TState, bool> resultsReceiver);
        void Back();
        void Next();
        void Finish();
    }
}
}
