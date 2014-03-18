using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Nieko.Infrastructure.Data
{
    /// <summary>
    /// Thrown when an attempt is made to create a PrimaryKey for an
    /// object that has no Key Properties defined by a PrimaryKey Attribute
    /// </summary>
    public class InvalidKeysException : Exception
    {
        public InvalidKeysException()
            : base()
        {
        }

        public InvalidKeysException(string message)
            : base(message)
        {
        }

        public InvalidKeysException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected InvalidKeysException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

    }
}
