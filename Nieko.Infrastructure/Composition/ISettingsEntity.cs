using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Composition
{
    /// <summary>
    /// Entity functionality for single setting
    /// </summary>
    public interface ISettingsEntity
    {
        string Name { get; set; }
        string Value { get; set; }
    }
}
