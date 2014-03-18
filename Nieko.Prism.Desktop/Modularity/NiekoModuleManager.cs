using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Modularity;
using Nieko.Prism.Modularity;
using Microsoft.Practices.Prism.Logging;

namespace Nieko.Prism.Modularity
{
    public class NiekoModuleManager : ModuleManager
    {
        private IEnumerable<IModuleTypeLoader> _ModuleTypeLoaders;

        public override IEnumerable<IModuleTypeLoader> ModuleTypeLoaders
        {
            get
            {
                if (_ModuleTypeLoaders == null)
                {
                    _ModuleTypeLoaders = new List<IModuleTypeLoader>
                    {
                        new FileModuleTypeLoader(),
                        new MemoryModuleTypeLoader()
                    };
                }
                return _ModuleTypeLoaders;
            }
            set
            {
                _ModuleTypeLoaders = value;
            }
        }

        public NiekoModuleManager(IModuleInitializer moduleInitializer, IModuleCatalog moduleCatalog, ILoggerFacade loggerFacade) :
            base(moduleInitializer, moduleCatalog, loggerFacade) { }
    }
}
