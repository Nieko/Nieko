using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Nieko.Infrastructure.Navigation.RecordNavigation;

namespace Nieko.Modules.Navigation.RecordNavigator
{
    public class EditStateCommand : ICommand
    {

        readonly Func<object, EditState, bool> _canEnterState;
        readonly Predicate<object> _canExecute;
        readonly Action<object, EditState> _execute;

        private EditState _EditState;

        public EditStateCommand(Action<object, EditState> execute, EditState editState, Func<object, EditState, bool> canEnterState)
            : this(execute, editState, canEnterState, null)
        {
        }

        public EditStateCommand(Action<object, EditState> execute, EditState editState, Func<object, EditState, bool> canEnterState, Predicate<object> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }
            if (editState == EditState.None)
            {
                throw new ArgumentException("editState parameter cannot be None");
            }
            if (canEnterState == null)
            {
                throw new ArgumentNullException("canEnterState");
            }

            _execute = execute;
            EditState = editState;
            _canEnterState = canEnterState;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public EditState EditState
        {
            get
            {
                return _EditState;
            }
            set
            {
                _EditState = value;
            }
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            if (!_canEnterState(parameter, EditState))
            {
                return false;
            }
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter, EditState);
        }

    }
}