using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Security
{
    /// <summary>
    /// Flags a class as having properties that define roles
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class RoleContainerAttribute : Attribute
    {
    }
}