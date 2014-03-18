using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Security;

namespace Nieko.Infrastructure.Security
{
    /// <summary>
    /// Core infrastructure security roles
    /// </summary>
    [RoleContainer]
    public class SystemRoles
    {
        private static RoleExpression _Administrators;
        private static RoleExpression _PermissionsMaintainers;
        private static RoleExpression _Users;

        public static RoleExpression Administrators
        {
            get
            {
                if (_Administrators == null)
                {
                    _Administrators = new RoleExpression(() => Administrators, new IRole[]{Users});
                }
                return _Administrators;
            }
        }

        public static RoleExpression PermissionsMaintainers
        {
            get
            {
                if (_PermissionsMaintainers == null)
                {
                    _PermissionsMaintainers = new RoleExpression(() => PermissionsMaintainers, new IRole[]{Users});
                }
                return _PermissionsMaintainers;
            }
        }

        public static RoleExpression Users
        {
            get
            {
                if (_Users == null)
                {
                    _Users = new RoleExpression(() => Users);
                }
                return _Users;
            }
        }

    }
}