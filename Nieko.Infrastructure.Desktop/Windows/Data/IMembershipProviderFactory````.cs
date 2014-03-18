using System;
using Nieko.Infrastructure.Data;

namespace Nieko.Infrastructure.Windows.Data
{
    /// <summary>
    /// Final step in membership graph factory
    /// </summary>
    /// <typeparam name="T">Membership ModelBiew</typeparam>
    /// <typeparam name="TParentEntity">Parent Entity</typeparam>
    /// <typeparam name="TEntity">Membership entity</typeparam>
    /// <typeparam name="TDataStore">Store for parent and member entities</typeparam>
    public interface IMembershipProviderFactory<T, TParentEntity, TEntity, TDataStore>
        where T : class, IMembershipProviderLineItem, new()
        where TEntity : class, new()
        where TDataStore : class, IDataStore
    {
        /// <summary>
        /// Build IMembershipProvider using parameters provided to this factory
        /// </summary>
        /// <returns>IMembershipProvider for this tier</returns>
        IMembershipProvider<T> Build();
        /// <summary>
        /// Build IMembershipProvider using parameters provided to this factory,
        /// specifying how removal permission is determined
        /// </summary>
        /// <returns>IMembershipProvider for this tier</returns>
        IMembershipProvider<T> Build(Func<IMembershipProvider<T>, bool> removeAllowed);
    }
}
