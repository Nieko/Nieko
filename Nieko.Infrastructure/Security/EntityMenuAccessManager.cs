using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Data;
using System.Data.Objects;
using Nieko.Infrastructure.Collections;
using Nieko.Infrastructure.Navigation.Menus;

namespace Nieko.Infrastructure.Security
{
    public abstract class EntityMenuAccessManager<TMenuEntity, TMenuRoleEntity, TDataStore> : IMenuAccessManager
        where TMenuEntity :IMenuEntity
        where TMenuRoleEntity : IMenuRoleEntity
        where TDataStore : class, IDataStore 
    {
        private bool _IsLoaded;
        private Dictionary<string, IRole> _RolesRequiredByPath;
        private HashSet<string> _HiddenIfDisabledMenus;

        protected IDataStoresManager DataStoresManager { get; private set; }
        protected IRolesProvider RolesProvider { get; private set; }
        protected ISecurityManager SecurityManager { get; private set; }
        protected IMenuNavigator MenuNavigator { get; private set; }
        
        public EntityMenuAccessManager(IDataStoresManager dataStoresManager, IRolesProvider rolesProvider, ISecurityManager securityManager, IMenuNavigator menuNavigator)
        {
            DataStoresManager = dataStoresManager;
            RolesProvider = rolesProvider;
            SecurityManager = securityManager;
            MenuNavigator = menuNavigator;
            _IsLoaded = false;
            _RolesRequiredByPath = new Dictionary<string, IRole>();
        }

        public bool HideIfDisabled(IMenu menu)
        {
            CheckLoaded();

            return _HiddenIfDisabledMenus.Contains(menu.GetFullPath());
        }

        public virtual bool HasAccess(IMenu menu, System.Security.Principal.IPrincipal principal)
        {
            CheckLoaded();

            var role = _RolesRequiredByPath[menu.GetFullPath()];

            return SecurityManager.IsInRole(role, principal);
        }

        private void CheckLoaded()
        {
            if (_IsLoaded)
            {
                return;
            }

            DataStoresManager.DoUnitOfWork<TDataStore>((dataStore) =>
            {
                var query = (dataStore.GetItems<TMenuEntity>() as ObjectQuery<TMenuEntity>)
                    .Include(BindingHelper.Name((TMenuEntity o) => o.RoleRequired))
                    .ToList();

                foreach (var item in query)
                {
                    _RolesRequiredByPath.Add(item.Path, item.RoleRequired.ToRole(RolesProvider));
                }

                _HiddenIfDisabledMenus = new HashSet<string>(query
                    .Where(entity => entity.HideIfDisabled)
                    .Select(entity => entity.Path));
            });

            _IsLoaded = true;
        }

        protected abstract TMenuRoleEntity CreateRoleEntity(IRole role);
        protected abstract TMenuEntity CreateMenuEntity(IMenu menu);

        public void UpdateFromDeclarations()
        {
            CheckLoaded();

            UpdateRolesFromDeclarations();
            UpdateMenusFromDeclarations();
        }

        private void UpdateRolesFromDeclarations()
        {
            var rolesByName = RolesProvider.Roles
                .ToDictionary(role => role.Name);
            IDictionary<string, TMenuRoleEntity> entitiesByName;

            DataStoresManager.DoUnitOfWork<TDataStore>((dataStore) =>
            {
                foreach (var removedRole in dataStore.GetItems<TMenuRoleEntity>()
                    .ToList()
                    .Where(entity => !rolesByName.ContainsKey(entity.Name)))
                {
                    dataStore.Delete<TMenuRoleEntity>(removedRole);
                }

                entitiesByName = dataStore.GetItems<TMenuRoleEntity>()
                    .ToDictionary(entity => entity.Name);

                foreach (var addedRole in RolesProvider.Roles
                    .Where(role => !entitiesByName.ContainsKey(role.Name)))
                {
                    dataStore.Save<TMenuRoleEntity>(CreateRoleEntity(addedRole));
                }
            });
        }

        private void UpdateMenusFromDeclarations()
        {
            var menusByPath = new Dictionary<string, IMenu>();
            Dictionary<string, TMenuEntity> entitiesByPath; 

            Action<IMenu> menuAppender = null;

            menuAppender = (menu) =>
                {
                    menusByPath.Add(menu.GetFullPath(), menu); 
                    foreach (var subMenu in menu.SubMenus)
                    {
                        menuAppender(subMenu);
                    }
                };

            menuAppender(MenuNavigator.RootMenu);

            DataStoresManager.DoUnitOfWork<TDataStore>((dataStore) =>
            {
                foreach (var removed in dataStore.GetItems<TMenuEntity>()
                    .ToList()
                    .Where(entity => !menusByPath.ContainsKey(entity.Path)))
                {
                    dataStore.Delete<TMenuEntity>(removed);
                }

                entitiesByPath = dataStore.GetItems<TMenuEntity>()
                    .ToDictionary(entity => entity.Path);

                foreach (var menuPath in menusByPath.Keys.
                    Where(path => !entitiesByPath.ContainsKey(path)))
                {
                    dataStore.Save(CreateMenuEntity(menusByPath[menuPath]));
                }
            });
        }
    }
}
