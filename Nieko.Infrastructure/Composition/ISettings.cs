using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Collections;

namespace Nieko.Infrastructure.Composition
{
    /// <summary>
    /// Global persistable settings
    /// </summary>
    public interface ISettings
    {
        void Save();
        void Refresh();
    }
}
