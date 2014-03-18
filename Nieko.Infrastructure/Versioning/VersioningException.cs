using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Nieko.Infrastructure.Versioning
{
    /// <summary>
    /// Raised if versioning components violate any of the rules of the 
    /// interfaces they implement
    /// </summary>
    public class VersioningException : Exception
    {
        public VersioningException()
            : base()
        {
        }

        public VersioningException(string message)
            : base(message)
        {
        }

        public VersioningException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected VersioningException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}