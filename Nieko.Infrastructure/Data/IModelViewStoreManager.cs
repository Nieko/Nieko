using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Data
{
    /// <summary>
    /// Temporally aware manager of ModelViewStores
    /// </summary>
    /// <remarks>
    /// As business rules are dynamic but, worse still,
    /// potentially time period dependant, this class
    /// allows for changes in business rules to occur and be
    /// correctly applied
    /// </remarks>
    public interface IModelViewStoresManager
    {
        /// <summary>
        /// Get the ModelViewStore implementing T that is appropriate for today
        /// </summary>
        /// <typeparam name="T">Base type of store</typeparam>
        /// <returns>ModelView Store</returns>
        T GetModelViewStore<T>() where T : IModelViewStore;
        /// <summary>
        /// Get the ModelViewStore implementing T that is appropriate for <paramref name="ruleSetDate"/>
        /// </summary>
        /// <typeparam name="T">Base type of store</typeparam>
        /// <param name="ruleSetDate">Date of Rule-set</param>
        /// <returns>ModelView Store</returns>
        T GetModelViewStore<T>(DateTime ruleSetDate) where T : IModelViewStore;
        /// <summary>
        /// Does unit of work against ModelViewStore implementing T that is appropriate for today
        /// </summary>
        /// <typeparam name="T">Base type of store</typeparam>
        /// <param name="work">Work to complete</param>
        void DoUnitOfWork<T>(Action<T> work) where T : IModelViewStore;
        /// <summary>
        /// Does unit of work against ModelViewStore implementing T that is appropriate for <paramref name="ruleSetDate"/>
        /// </summary>
        /// <typeparam name="T">Base type of store</typeparam>
        /// <param name="ruleSetDate">Date of Rule-set</param>
        /// <param name="work">Work to complete</param>
        void DoUnitOfWork<T>(DateTime ruleSetDate, Action<T> work) where T : IModelViewStore;
    }
}
