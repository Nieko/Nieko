using System;

namespace Nieko.Infrastructure.Reflection
{
    /// <summary>
    /// Factory class for building an individual 
    /// plug-in instance
    /// </summary>
    public interface IPlugInFactory
    {
        object Instance { get; }
    }
}
