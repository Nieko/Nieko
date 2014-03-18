using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Versioning
{
    /// <summary>
    /// Entity providing persistence of version data
    /// </summary>
    public interface IVersionEntity
    {
        int Build { get; set; }
        int Major { get; set; }
        int Minor { get; set; }
        int Revision { get; set; }
        DateTime Upgraded { get; set; }
        IComponentEntity Component { get; set; }
    }
}