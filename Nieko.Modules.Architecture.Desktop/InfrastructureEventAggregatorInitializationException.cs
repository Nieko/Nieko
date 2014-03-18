using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Modules.Architecture
{
    public class InfrastructureEventAggregatorInitializationException : Exception
    {
        public InfrastructureEventAggregatorInitializationException(string message) : base(message) { }
    }
}
