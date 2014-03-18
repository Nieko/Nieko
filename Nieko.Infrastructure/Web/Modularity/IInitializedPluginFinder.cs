using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Reflection;

namespace Nieko.Infrastructure.Web.Modularity
{
    public interface IInitializedPluginFinder : IPluginFinder
    {
        void Initialize();
    }
}
}
