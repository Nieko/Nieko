using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Versioning
{
    /// <summary>
    /// Functionality implemented by entity persisting
    /// a versioned component
    /// </summary>
    public interface IComponentEntity
    {
        string Name { get; set; }
    }
}