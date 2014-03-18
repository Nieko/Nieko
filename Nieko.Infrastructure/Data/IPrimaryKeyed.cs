using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Data
{
    /// <summary>
    /// An object that has a set of one or properties that
    /// uniquely identify it against other objects of the same
    /// type
    /// </summary>
    public interface IPrimaryKeyed : IComparable
    {
        PrimaryKey PrimaryKey { get; }
    }
}
