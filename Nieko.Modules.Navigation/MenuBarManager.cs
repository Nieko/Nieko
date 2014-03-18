using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Navigation;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Commands;
using Nieko.Infrastructure.ComponentModel;

namespace Nieko.Modules.Navigation
{
    public class MenuBarManager : NotifyPropertyChangedBase, IMenuBarManager
    {
        private IViewNavigator _RegionNavigator;
        private Dictionary<EndPoint, Stack<Action>> _CleanupsByOwner;
        private IWeakEventRouter _NavigatingRouter;

        public bool IsVisible
        {
            get
            {
                return Get(() => IsVisible);
            }
            set
            {
                Set(() => IsVisible, value);
            }
        }

        public Menu MenuBar { get; set; }

        public MenuBarManager(IViewNavigator regionNavigator)
        {
            _RegionNavigator = regionNavigator;

            _NavigatingRouter = WeakEventRouter.CreateInstance(this,
                regionNavigator,
                () => default(NavigationEventArgs), 
                (o, d) =>  o.Navigating += d.Handler,
                (o, d) => o.Navigating -= d.Handler,
                Navigating);

            _CleanupsByOwner = new Dictionary<EndPoint, Stack<Action>>();
        }

        private ICommand Create(string path, Action<MenuItem> initialization, EndPoint owner, Func<ICommand> commandFactory)
        {
            var command = commandFactory();
            var menuTree = path.Split('\\');
            MenuItem parentMenuItem;
            MenuItem child = null;
            ItemCollection children;
            Stack<Action> cleanup; 

            if(!_CleanupsByOwner.TryGetValue(owner, out cleanup))
            {
                cleanup = new Stack<Action>();
                _CleanupsByOwner.Add(owner, cleanup);
            }

            parentMenuItem = MenuBar.Items.OfType<MenuItem>().FirstOrDefault(mi => (string)mi.Header == menuTree[0]);

            if (parentMenuItem == null)
            {
                parentMenuItem = new MenuItem() { Header = menuTree[0] };
                MenuBar.Items.Add(parentMenuItem);
            }
            children = parentMenuItem.Items;

            cleanup.Push(() =>
                {
                    var parentChildren = parentMenuItem.ItemsSource as List<MenuItem>;
                    if (parentChildren.Count == 1)
                    {
                        parentMenuItem.ItemsSource = null;
                        parentChildren.Clear();
                        MenuBar.Items.Remove(parentMenuItem);
                    }
                });

            for (int i = 1; i < menuTree.Length; i++)
            {
                child = children.OfType<MenuItem>().FirstOrDefault(c => (string)c.Header == menuTree[i]);
                if (child == null)
                {
                    child = new MenuItem();
                    child.Header = menuTree[i];
                    children.Add(child);

                    children = child.Items;
                }
                else
                {
                    if (i == menuTree.Length - 1)
                    {
                        throw new ArgumentException("MenuItem " + path + " already exists"); 
                    }
                    children = child.Items;
                }

                var cleanupChild = child;
                cleanup.Push(() =>
                {
                    var cleanupChildren = cleanupChild.Items;

                    if (cleanupChildren.Count == 1)
                    {
                        cleanupChildren.Clear();
                    }
                });
            }

            child.Command = command;
            initialization(child);

            return command;
        }

        public ICommand Create(string path, Action<MenuItem> initialization, EndPoint owner, Action<object> eventMethod)
        {
            return Create(path, initialization, owner, () => new DelegateCommand<object>(eventMethod)); 
        }

        public ICommand Create(string path, EndPoint owner, Action<object> eventMethod)
        {
            return Create(path, mi => { }, owner, () => new DelegateCommand<object>(eventMethod)); 
        }

        public ICommand Create(string path, EndPoint owner, ICommand command)
        {
            return Create(path, mi => { }, owner, () => command);
        }

        public ICommand Create(string path, Action<MenuItem> initialization, EndPoint owner, ICommand command)
        {
            return Create(path, initialization, owner, () => command);
        }

        private void Navigating(IMenuBarManager barManager, IViewNavigator navigator, NavigationEventArgs e)
        {
            Stack<Action> cleanup;

            foreach (var owner in _CleanupsByOwner
                .Where(kvp => kvp.Key != EndPoint.Root && kvp.Key != e.Destination)
                .Select(kvp => kvp.Key)
                .ToList())
            {
                cleanup = _CleanupsByOwner[owner];

                while (cleanup.Count != 0)
                {
                    cleanup.Pop()();
                }

                _CleanupsByOwner.Remove(owner);
            }
        }
    }
}
