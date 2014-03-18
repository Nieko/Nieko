using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Nieko.Infrastructure.Navigation.Menus;

namespace Nieko.Modules.Navigation.Menus
{
    internal class Menu : IMenu
    {
        private Action<IMenu> _ChangeMenuAction;
        private IMenuNavigator _MenuNavigator;
        
        public event EventHandler<NavigationEventArgs> BeforeNavigation;

        public string Caption { get; set; }

        public IMenu Parent { get; set; }

        public int Position { get; set; }

        public MenuState State { get; private set; }

        public IList<IMenu> SubMenus { get; private set; }

        public Menu(IMenuNavigator menuNavigator)
        {
            _MenuNavigator = menuNavigator;
            SubMenus = new List<IMenu>();
        }

        public bool Navigate()
        {
            _ChangeMenuAction(this);
            return true;
        }

        public void OnBeforeNavigation()
        {
            EventHandler<NavigationEventArgs> handler = BeforeNavigation;
            NavigationEventArgs eventArgs = new NavigationEventArgs();
            if (handler != null)
            {
                handler(this, eventArgs);
            }

            if (!eventArgs.Cancel)
            {
                Navigate();
            }
        }

        internal void SetChangeMenuAction(Action<IMenu> changeMenuAction)
        {
            _ChangeMenuAction = changeMenuAction;
        }

        private void DoNavigation(object parameters)
        {
            OnBeforeNavigation();
            Navigate();
        }
    }
}