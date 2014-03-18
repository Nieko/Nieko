using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Nieko.Infrastructure.Reflection
{
    /// <summary>
    /// An exception raised at plug-in resolution
    /// </summary>
    public class PluginException : Exception
    {
        public PluginException() : base() { }

        public PluginException(string message) : base(message) { }

        protected PluginException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public PluginException(string message, Exception innerException) : base(message, innerException) { }
    }
}

