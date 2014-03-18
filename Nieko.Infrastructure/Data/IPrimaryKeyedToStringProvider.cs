using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Data
{
    /// <summary>
    /// Classes implementing this provide their own method of turning their PrimaryKey object
    /// into a string representation
    /// </summary>
    public interface IPrimaryKeyedToStringProvider : IPrimaryKeyed
    {
        Func<string> PrimaryKeyToString { get; }
    }
}
