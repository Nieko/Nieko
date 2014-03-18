using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Data
{
    /// <summary>
    /// Indicates an alternate Primary Key Filter Expression Provider
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)] 
    public class FilterExpressionProviderAttribute : Attribute
    {
        public Type Provider { get; private set; }

        public FilterExpressionProviderAttribute(Type provider)
        {
            Provider = provider;
        }
    }
}
