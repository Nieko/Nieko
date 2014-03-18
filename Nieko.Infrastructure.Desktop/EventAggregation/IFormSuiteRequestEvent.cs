using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Windows;

namespace Nieko.Infrastructure.EventAggregation
{
    /// <summary>
    /// Used by multi-form suite screens to request and supply
    /// the containing screen to the individual forms.
    /// </summary>
    /// <remarks>
    /// By use of infrastructure events, supply can be provided at a 
    /// point where the Form Suite is properly initialized
    /// </remarks>
    public interface IFormSuiteRequestEvent : IInfrastructureEvent
    {
        /// <summary>
        /// Form Suite screen to request an instance of
        /// </summary>
        Type TypeRequested { get; }
        /// <summary>
        /// Form Suite satisfying the <seealso cref="TypeRequested"/>
        /// </summary>
        object Result { get; set; }
        /// <summary>
        /// Sets the <seealso cref="TypeRequested"/> to <typeparamref name="T"/>
        /// May only be called once
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void Initialize<T>()
            where T : IFormsSuite;
    }
}
