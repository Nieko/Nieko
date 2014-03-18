using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Workflow;
using System.ComponentModel;

namespace Nieko.Infrastructure.Navigation.Menus
{
    public interface IWizardNavigator : INotifyPropertyChanged
    {
        bool BackAllowed { get; }
        bool NextAllowed { get; }
        void StartWizard<TState>(IStateMachine<TState> stateMachine, TState initialState, Action<TState, bool> resultsReceiver) where TState : ICloneable;
        void Back();
        void Next();
        void Finish();
        void Cancel();
    }
}
}
