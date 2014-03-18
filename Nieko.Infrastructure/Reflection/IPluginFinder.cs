using System;
using System.Collections.Generic;

namespace Nieko.Infrastructure.Reflection
{
    /// <summary>
    /// Functionality for plug-in discovery, instantiation and delivery.
    /// </summary>
    // TODO investigate implementation using MEF
    public interface IPluginFinder
    {
        /// <summary>
        /// Registers with the finder interest in receiving all plug-ins
        /// of type <typeparamref name="T"/>. The provided <paramref name="callBack"/> will be
        /// executed either at <seealso cref="IInitializePluginFrameworkEvent"/> or, if event already published,
        /// immediately
        /// </summary>
        /// <remarks>
        /// A single instance is created for each concrete type implementing <typeparamref name="T"/>
        /// </remarks>
        /// <typeparam name="T">Base type implemented by plug-ins</typeparam>
        /// <param name="callBack">Processing call back</param>
        void RegisterCreatePluginsCallBack<T>(Action<IEnumerable<T>> callBack)
            where T : class;
    }
}
