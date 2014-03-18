using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.ComponentModel
{
    /// <summary>
    /// Implemented by classes wishing to declare EndPoints.
    /// Also used within EndPoint framework to avoid working with
    /// generics present in EndPointProvider[T] base class
    /// </summary>
    public interface IEndPointProvider
    {
        IEnumerable<EndPoint> GetEndPoints();
    }
}
