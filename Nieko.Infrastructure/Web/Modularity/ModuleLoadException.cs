using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Nieko.Infrastructure.Web.Modularity
{
    public class ModuleLoadException : Exception
    {
        public ModuleLoadException() : base() { }

        public ModuleLoadException(string message) : base(message) { }

        protected ModuleLoadException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public ModuleLoadException(string message, Exception innerException) : base(message, innerException) { }

    }
}
}
