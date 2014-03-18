using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Security
{
    /// <summary>
    /// Functionality required of entity storing Menu security details
    /// </summary>
    public interface IMenuEntity
    {
        string Path { get; }
        IMenuRoleEntity RoleRequired { get; }
        bool HideIfDisabled { get; }
    }
}
