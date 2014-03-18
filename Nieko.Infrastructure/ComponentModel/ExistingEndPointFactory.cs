using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.ComponentModel
{
    internal class ExistingEndPointFactory : IEndPointFactory 
    {
        private EndPoint _Existing;

        public ExistingEndPointFactory(EndPoint existing)
        {
            if (existing == null)
            {
                throw new ArgumentNullException("existing");
            }

            _Existing = existing;
        }

        public IEndPointFactoryCreateMenuEntry Description(string description)
        {
            return this;
        }

        public IEndPointFactoryOrdinal CreateMenuEntry(bool createMenuEntry)
        {
            return this;
        }

        public IEndPointFactoryFinish Ordinal(short ordinal)
        {
            return this;
        }

        public EndPoint Build()
        {
            return _Existing;
        }
    }
}
