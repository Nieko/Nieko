using System;

namespace Nieko.Infrastructure.Web.Modularity
{
    public interface IModuleLoader
    {
        ModuleInfo Add<TModule>() where TModule : IModule;
        void Load();
    }
}
}
