using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Prism.Unity
{
    public sealed class SingletonImplementation<T>
    {
        internal Type Implementation { get; private set; }

        internal SingletonImplementation()
        {
            Implementation = null;
        }

        public void Set<TImplementation>()
            where TImplementation : class, T
        {
            if (typeof(TImplementation).IsAbstract ||
                typeof(TImplementation).IsGenericTypeDefinition ||
                typeof(TImplementation).IsInterface)
            {
                throw new ArgumentException(typeof(TImplementation).FullName + " must be a concrete type"); 
            }

            if (Implementation != null)
            {
                throw new InvalidOperationException("Implementation already set");
            }

            Implementation = typeof(TImplementation);
        }
    }
}
