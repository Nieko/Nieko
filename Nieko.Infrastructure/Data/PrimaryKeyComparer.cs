using System;
using System.Collections.Generic;

namespace Nieko.Infrastructure.Data
{
    /// <summary>
    /// IEqualityComparer implementation for PrimaryKeys
    /// </summary>
    public class PrimaryKeyComparer : EqualityComparer<PrimaryKey>
    {
        public static PrimaryKeyComparer Instance = new PrimaryKeyComparer();

        public override bool Equals(PrimaryKey x, PrimaryKey y)
        {
            return x.Equals(y);
        }

        public override int GetHashCode(PrimaryKey obj)
        {
            return obj.GetHashCode();
        }
    }
}