using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Data
{
    public abstract class PrimaryKeyedBase<T> : IPrimaryKeyed, IEquatable<T>
        where T : PrimaryKeyedBase<T>
    {
        private PrimaryKey _PrimaryKey;

        public PrimaryKey PrimaryKey
        {
            get 
            {
                if (_PrimaryKey == null)
                {
                    _PrimaryKey = new PrimaryKey(this);
                }
                return _PrimaryKey; 
            }
        }

        public int CompareTo(object obj)
        {
            var otherKeyed = (obj as IPrimaryKeyed);

            if (otherKeyed == null)
            {
                return 1;
            }

            return PrimaryKey.CompareTo(otherKeyed.PrimaryKey); 
        }

        public bool Equals(T other)
        {
            return other != null && PrimaryKey.Equals(other.PrimaryKey); 
        }

        public override int GetHashCode()
        {
            return PrimaryKey.GetHashCode(); 
        }
    }
}
