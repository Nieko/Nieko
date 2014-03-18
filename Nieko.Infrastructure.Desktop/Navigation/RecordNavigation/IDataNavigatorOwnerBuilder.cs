using Nieko.Infrastructure.Data;
using System.Collections.Generic;
using System;
using Nieko.Infrastructure.ComponentModel;
using Nieko.Infrastructure.Windows.Data;

namespace Nieko.Infrastructure.Navigation.RecordNavigation
{
    /// <summary>
    /// Fluent Factory interface for creating a Data Navigator Owner
    /// </summary>
    /// <remarks>
    /// Used by IPersistedView implementations at initialization
    /// to create remaining necessary Data Navigation objects
    /// for their tier
    /// </remarks>
    public interface IDataNavigatorOwnerBuilder
    {
        /// <summary>
        /// Provides the ViewModel that determines the lifetime of 
        /// the IDataNavigationOwner
        /// </summary>
        /// <param name="lifetimeContext">Object with encompassing lifetime</param>
        /// <returns>Current instance</returns>
        IDataNavigatorOwnerBuilderOwned CreateDataNavigator(INotifyDisposing lifetimeContext);
    }

    /// <summary>
    /// Fluent Factory interface for creating a Data Navigator Owner
    /// </summary>
    public interface IDataNavigatorOwnerBuilderOwned
    {
        /// <summary>
        /// Persistence to be used by the IDataNavigationOwner
        /// </summary>
        /// <typeparam name="T">Editable entity Mirror</typeparam>
        /// <param name="persistedView">Persistence</param>
        /// <returns>Current instance</returns>
        IDataNavigatorOwnerBuilderWithView UsingPersistedView<T>(IPersistedView<T> persistedView)
            where T : IEditableMirrorObject;
    }

    /// <summary>
    /// Fluent Factory interface for creating a Data Navigator Owner
    /// </summary>
    public interface IDataNavigatorOwnerBuilderWithView
    {
        /// <summary>
        /// The owner of the next higher editable tier
        /// </summary>
        /// <remarks>
        /// Also used as the lifetime determiner for the IDataNavigatorOwner
        /// instance built
        /// </remarks>
        /// <param name="parent">Tier owner</param>
        /// <returns>Current instance</returns>
        IDataNavigatorOwnerBuilderWithView WithParent(IDataNavigatorOwner parent);
        /// <summary>
        /// Configures the visual user interface for navigation and
        /// editing
        /// </summary>
        /// <param name="config">Configuration</param>
        /// <returns>Current instance</returns>
        IDataNavigatorOwnerBuilderWithView ProvidingUIControlAt(IUIConfig config);
        /// <summary>
        /// Create the IDataNavigatorOwner
        /// </summary>
        /// <returns>Created instance</returns>
        IDataNavigatorOwner Build();
    }
}
