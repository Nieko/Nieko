using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Nieko.Infrastructure.Navigation.RecordNavigation;

namespace Nieko.Modules.Navigation.RecordNavigator
{
    public class RecordNavigationCommand : ICommand
    {

        readonly Predicate<object> _canExecute;
        readonly Func<object, RecordNavigation, bool> _canNavigate;
        readonly Action<object, RecordNavigation> _execute;

        private RecordNavigation _Navigation;

        public RecordNavigationCommand(Action<object, RecordNavigation> execute, RecordNavigation navigation, Func<object, RecordNavigation, bool> canNavigate)
            : this(execute, navigation, canNavigate, null)
        {
        }

        public RecordNavigationCommand(Action<object, RecordNavigation> execute, RecordNavigation navigation, Func<object, RecordNavigation, bool> canNavigate, Predicate<object> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }
            if (navigation == RecordNavigation.None)
            {
                throw new ArgumentException("navigation parameter cannot be None");
            }
            if (canNavigate == null)
            {
                throw new ArgumentNullException("canNavigate");
            }

            _execute = execute;
            Navigation = navigation;
            _canNavigate = canNavigate;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RecordNavigation Navigation
        {
            get
            {
                return _Navigation;
            }
            set
            {
                _Navigation = value;
            }
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            if (!_canNavigate(parameter, Navigation))
            {
                return false;
            }
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter, Navigation);
        }

    }
}