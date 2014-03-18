using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.ComponentModel
{
    /// <summary>
    /// Functionality required in a class providing serialization
    /// of EndPoint MetaContext data
    /// </summary>
    public interface IEndPointContextSerializer
    {
        string Serialize(object context);
        object Deserialize(string data);
    }
}
