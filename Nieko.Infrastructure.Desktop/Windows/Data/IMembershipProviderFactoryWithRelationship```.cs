using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Nieko.Infrastructure.ComponentModel;
using Nieko.Infrastructure.Windows.Data;
using Nieko.Infrastructure.Data;

namespace Nieko.Infrastructure.Windows.Data
{
    /// <summary>
    /// Step in membership graph factory to determine
    /// ModelView and mapping details
    /// </summary>
    /// <typeparam name="TParentEntity">Parent Entity</typeparam>
    /// <typeparam name="TEntity">Membership entity</typeparam>
    /// <typeparam name="TDataStore">Store for parent and member entities</typeparam>
    public interface IMembershipProviderFactoryWithRelationship<TParentEntity, TEntity, TDataStore>
        where TEntity : class, new()
        where TDataStore : class, IDataStore
    {
        /// <summary>
        /// Specifies membership ModelView type, using the default Type Mapper
        /// </summary>
        /// <typeparam name="T">Membership ModelView</typeparam>
        /// <returns>Next step in factory</returns>
        IMembershipProviderFactory<T, TParentEntity, TEntity, TDataStore> ForLinesOf<T>() 
            where T : class, IMembershipProviderLineItem, new();
        /// <summary>
        /// Specifies membership ModelView type, configuring the Type Mapper
        /// using the method provided
        /// </summary>
        /// <typeparam name="T">Membership ModelView</typeparam>
        /// <param name="typeMapperSetup">Type Mapper configuration</param>
        /// <returns>Next step in factory</returns>
        IMembershipProviderFactory<T, TParentEntity, TEntity, TDataStore> InitializingTypeMapperBy<T>(Action<ITypeMapper<T, TEntity>> typeMapperSetup) 
            where T : class, IMembershipProviderLineItem, new();
    }
}