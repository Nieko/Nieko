using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Nieko.Infrastructure.Reflection;
using Nieko.Infrastructure.Collections;
using Nieko.Infrastructure.Navigation.Menus;
using Nieko.Infrastructure.EventAggregation;
using Nieko.Infrastructure.Security;
using Nieko.Infrastructure.ComponentModel;
using Nieko.Infrastructure.Windows;

namespace Nieko.Modules.Navigation.Menus
{
    public class MenuNavigator : IMenuNavigator
    {
        private bool _ApplicationInitialized = false;
        private IEnumerable<IEndPointProvider> _Providers = null;
        private IMenu _CurrentMenu;
        private IMenu _RootMenu;
        private HashSet<KeyValuePair<string, IMenu>> _UnattachedMenus;
        private IFormsManager _FormsManager;

        public event EventHandler CurrentMenuChanged;

        public MenuNavigator(IInfrastructureEventAggregator eventAggregator, IPluginFinder pluginFinder, IFormsManager formsManager)
        {
            _UnattachedMenus = new HashSet<KeyValuePair<string, IMenu>>();
            _FormsManager = formsManager;

            pluginFinder.RegisterCreatePluginsCallBack<IEndPointProvider>(ProcessPlugins);
            eventAggregator.Subscribe<IApplicationInitializedEvent>((args) => ApplicationInitialized());
        }

        public virtual IMenu CurrentMenu
        {
            get
            {
                return _CurrentMenu;
            }
            protected set
            {
                if (_CurrentMenu == value)
                {
                    return;
                }
                _CurrentMenu = value;
                RaiseCurrentMenuChanged();
            }
        }

        public virtual IMenu RootMenu
        {
            get
            {
                return _RootMenu;
            }
            protected set
            {
                if (_RootMenu != null)
                {
                    throw new InvalidOperationException("RootMenu has already been set");
                }
                _RootMenu = value;
            }
        }

        protected virtual void RaiseCurrentMenuChanged()
        {
            var handler = CurrentMenuChanged;

            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        private void ProcessRegistration(MenuDefinition menuData)
        {
            Menu menuItem = new Menu(this);
            IMenu parentItem = null;

            menuItem.Caption = menuData.Caption;
            menuItem.Position = menuData.Position;

            if (menuData.ParentMenuPath != string.Empty)
            {
                parentItem = FindMenu(menuData.ParentMenuPath);
                if (parentItem != null)
                {
                    menuItem.Parent = parentItem;
                    parentItem.SubMenus.Add(menuItem);
                }
                else
                {
                    _UnattachedMenus.Add(new KeyValuePair<string, IMenu>(menuData.ParentMenuPath, menuItem));
                }
            }
            else
            {
                RootMenu = menuItem;
            }

            if ((menuData.ParentMenuPath == string.Empty || parentItem != null)
                && _UnattachedMenus.Count != 0)
            {
                AttemptAttachMenus();
            }

            menuItem.SetChangeMenuAction(m =>
                {
                    if (_FormsManager.Show(menuData.EndPoint))
                    {
                        CurrentMenu = m;
                    }
                });
        }

        private IMenu FindMenu(string menuPath)
        {
            IMenu currentLevel = RootMenu;
            IList<string> segments;

            if (menuPath == "/")
            {
                return currentLevel;
            }

            segments = menuPath.Split('/').ToList();
            segments.RemoveAt(0);

            foreach(var segment in segments)
            {
                if (currentLevel == null)
                {
                    return null;
                }

                currentLevel = currentLevel.SubMenus.FirstOrDefault(subMenu => subMenu.Caption == segment);
            }

            return currentLevel;
        }

        private void AttemptAttachMenus()
        {
            int lastUnattachedCount = 0;
            IMenu parentMenu;
            HashSet<IMenu> changedMenuParents = new HashSet<IMenu>();
            bool currentMenuChanged = false;

            while (lastUnattachedCount != _UnattachedMenus.Count)
            {
                lastUnattachedCount = _UnattachedMenus.Count;

                foreach (var pair in _UnattachedMenus.ToList())
                {
                    parentMenu = FindMenu(pair.Key);
                    if (parentMenu == null)
                    {
                        continue;
                    }

                    parentMenu.SubMenus.Add(pair.Value);
                    pair.Value.Parent = parentMenu;
                    currentMenuChanged = currentMenuChanged || (parentMenu == CurrentMenu);

                    if (!changedMenuParents.Contains(parentMenu))
                    {
                        changedMenuParents.Add(parentMenu);
                    }

                    _UnattachedMenus.Remove(pair);
                }
            }

            ReorderMenus(changedMenuParents);
        }

        private void ProcessPlugins(IEnumerable<IEndPointProvider> providers)
        {
            _Providers = providers;
            RegisterMenus();
        }

        private void ApplicationInitialized()
        {
            _ApplicationInitialized = true;
            RegisterMenus();
        }

        private void RegisterMenus()
        {
            if (_Providers == null || !_ApplicationInitialized)
            {
                return;
            }

            var endPoints = _Providers.SelectMany(p => p.GetEndPoints());
            ProcessRegistration(new MenuDefinition(EndPoint.Root));

            foreach (var definition in endPoints
                .Where(ep => ep.CreateMenuEntry == true)
                .Select(ep => new MenuDefinition(ep)))
            {
                ProcessRegistration(definition);
            }

            if (_UnattachedMenus.Count > 0)
            {
                throw new KeyNotFoundException("Unable to resolve menu registrations");
            }

            RootMenu.Navigate();
        }

        private void ReorderMenus(IEnumerable<IMenu> changedMenuParents)
        {
            IEnumerable<IMenu> orderedChildren;

            foreach (var parentMenu in changedMenuParents)
            {
                orderedChildren = parentMenu.SubMenus.OrderBy(childMenu => childMenu.Position).ToList();

                parentMenu.SubMenus.Clear();
                foreach (var childMenu in orderedChildren)
                {
                    parentMenu.SubMenus.Add(childMenu);
                }
            }
        }

        public bool NavigateTo(EndPoint destination)
        {
            if (!destination.CreateMenuEntry)
            {
                throw new ArgumentException(destination.GetFullPath() + " does not have a menu entry");
            }

            var menu = FindMenu(destination.GetMenuPath());

            if (menu == null)
            {
                return false;
            }

            return menu.Navigate(); 
        }
    }
}