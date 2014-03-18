using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Data
{
    /// <summary>
    /// Indicates a reference property is retrieved from a different
    /// Data Store
    /// </summary>
    public class CrossContextAttribute : Attribute
    {
        public Type DataStore { get; private set; }

        public CrossContextAttribute(Type dataStore)
        {
            DataStore = dataStore;
        }
    }
}
