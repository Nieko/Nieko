using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.ComponentModel
{
    /// <summary>
    /// Controlling behaviour for creation of EndPoints.
    /// </summary>
    /// <remarks>
    /// Default implementation is NoEndPointValidation; if alternative validation
    /// is required implement IEndPointValidation in one (and one only) other class
    /// 
    /// If multiple alternative implementations are found, an exception will be raised 
    /// <see cref="Nieko.Infrastructure.ComponentModel.EndPointInitialization"/>
    /// </remarks>
    public interface IEndPointValidation
    {
        Func<EndPoint, bool> CanAddCheck { get; }
    }
}
