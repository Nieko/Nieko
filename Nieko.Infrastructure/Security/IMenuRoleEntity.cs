using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Security
{
    /// <summary>
    /// Functionality required of entity storing Role security details
    /// </summary>
    public interface IMenuRoleEntity
    {
        string Name { get; set; }
        IRole ToRole(IRolesProvider rolesProvider);
        System.Collections.IEnumerable MenuItems { get; } 
    }
}
