namespace Nieko.Infrastructure.Security
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Principal;
    using System.Text;
    using Nieko.Infrastructure.Data;
    using Nieko.Infrastructure.EventAggregation;

    public abstract class EntitySecurityManager<TLoginEntity, TGroupEntity, TRoleEntity, TDataStore> : ISecurityManager
        where TDataStore : class, IDataStore 
    {
        private static IPrincipal _InvalidLogin;
        private IPrincipal _CurrentUser;
        private Func<TGroupEntity, IEnumerable<TRoleEntity>> _GroupRolesMethod;
        private Func<TLoginEntity, IEnumerable<TGroupEntity>> _LoginGroupsMethod;
        private Func<TLoginEntity, string> _LoginNameAccessor;
        private Func<TRoleEntity, string> _RoleNameAccessor;
        private IInfrastructureEventAggregator _EventAggregator;

        public static IPrincipal InvalidLogin
        {
            get
            {
                if (_InvalidLogin == null)
                {
                    _InvalidLogin = new GenericPrincipal(new GenericIdentity(BindingHelper.Name(() => InvalidLogin)), new string[] { });
                }
                return _InvalidLogin;
            }
        }

        public IPrincipal CurrentUser
        {
            get
            {
                return DataStoresManager.DoUnitOfWork<TDataStore, IPrincipal>((dataStore) =>
                {
                    if (_CurrentUser == null)
                    {
                        TLoginEntity login = GetCurrentLogin();

                        if (login == null)
                        {
                            return InvalidLogin;
                        }

                        GenericIdentity identity = new GenericIdentity(_LoginNameAccessor(login));

                        var roleNames = _LoginGroupsMethod(login)
                            .SelectMany(g => _GroupRolesMethod(g))
                            .Select(r => _RoleNameAccessor(r));

                        _CurrentUser = new GenericPrincipal(identity, roleNames.ToArray());
                    }

                    return _CurrentUser;
                });
            }
        }

        public IDataStoresManager DataStoresManager { get; private set; }

        protected bool IsInitialised { get; set; }

        public IDictionary<string, IRole> RolesByName { get; protected set; }

        public IRolesProvider RolesProvider { get; private set; }

        public EntitySecurityManager(IRolesProvider rolesProvider, IDataStoresManager dataStoresManager,
            Func<TLoginEntity, string> loginNameAccessor,
            Func<TLoginEntity, IEnumerable<TGroupEntity>> loginGroupsMethod,
            Func<TGroupEntity, IEnumerable<TRoleEntity>> groupRolesMethod,
            Func<TRoleEntity, string> roleNameAccessor,
            IInfrastructureEventAggregator eventAggregator)
        {
            RolesProvider = rolesProvider;
            DataStoresManager = dataStoresManager;
            _LoginNameAccessor = loginNameAccessor;
            _LoginGroupsMethod = loginGroupsMethod;
            _GroupRolesMethod = groupRolesMethod;
            _RoleNameAccessor = roleNameAccessor;
            eventAggregator.Subscribe<IInitializeSecurityEvent>((args) => Initialize());
            eventAggregator.Subscribe<IStartupNotificationsRequestEvent>(StartupNotificationsRequested);

            _EventAggregator = eventAggregator;
        }

        protected virtual void Initialize()
        {
            if (IsInitialised)
            {
                return;
            }

            RolesByName = RolesProvider.Roles.ToDictionary(r => r.Name);

            foreach (var role in RolesProvider.Roles)
            {
                if (!EntityExistsForRole(role))
                {
                    CreateRoleEntity(role);
                }
            }

            IsInitialised = true;
        }

        public virtual bool IsInRole(IRole role, IPrincipal principal)
        {
            return principal.IsInRole(role.Name);
        }

        protected abstract void CreateRoleEntity(IRole role);

        protected abstract bool EntityExistsForRole(IRole role);

        protected abstract TLoginEntity GetCurrentLogin();

        private void StartupNotificationsRequested(IStartupNotificationsRequestEvent args)
        {
            if (CurrentUser == _InvalidLogin)
            {
                args.CriticalNotifications.Enqueue(new StartupNotification("You do not have permissions to use this application. Please contact the system administrator if you require access."));
            }
        }
    }
}