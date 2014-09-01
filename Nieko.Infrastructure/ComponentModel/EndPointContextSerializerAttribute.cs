using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.ComponentModel
{
    /// <summary>
    /// Declares for a class to be used as MetaContext for an EndPoint
    /// how it will be serialized / deserialized
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EndPointContextSerializerAttribute : Attribute
    {
        internal static Dictionary<Type, Type> DataTypeSerializers { get; private set; }

        public Type Serializer { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serializer">Class to provide serialization. Must implement <see cref="IEndPointContextSerializer"/></param>
        public EndPointContextSerializerAttribute(Type serializer)
        {
            if (!typeof(IEndPointContextSerializer).IsAssignableFrom(serializer))
            {
                throw new ArgumentException("Serializer must implement " + typeof(IEndPointContextSerializer).Name);
            }

            Serializer = serializer;
        }

        static EndPointContextSerializerAttribute()
        {
            DataTypeSerializers = new Dictionary<Type, Type>();
        }
    }
}
