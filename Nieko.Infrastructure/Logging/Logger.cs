using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Logging
{
    /// <summary>
    /// As logger is a global service we provide a runtime initialized singleton
    /// rather than demand that classes declare ILogger as a dependency
    /// (i.e. through constructor injection)
    /// </summary>
    public static class Logger
    {
        public static ILogger Instance { get; private set; }

        /// <summary>
        /// Called at Container bootstrapping once ILogger has been resolved
        /// </summary>
        /// <param name="logger">Logger implementation</param>
        public static void SetLogger(ILogger logger)
        {
            if (Instance != null)
            {
                throw new InvalidOperationException("Cannot change logger once it has been already set");  
            }

            Instance = logger;
        }
    }
}
