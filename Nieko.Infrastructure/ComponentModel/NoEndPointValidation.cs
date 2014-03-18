using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.ComponentModel
{
    public class NoEndPointValidation : IEndPointValidation
    {
        public Func<EndPoint, bool> CanAddCheck
        {
            get { return ep => true; }
        }
    }
}
