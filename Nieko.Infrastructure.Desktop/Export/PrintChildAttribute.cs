using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Export
{
    /// <summary>
    /// Indicates that a visual element should print its content child,
    /// not the element itself
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)] 
    public class PrintChildAttribute : Attribute
    {
    }
}
