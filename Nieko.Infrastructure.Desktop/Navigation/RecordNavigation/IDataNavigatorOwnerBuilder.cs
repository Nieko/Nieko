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
    public interface ITierCoordinatorBuilder
    {
        /// <summary>
        /// Provides the ViewModel that determines the lifetime of 
        /// the IDataNavigationOwner
        /// </summary>
        /// <param name="lifetimeContext">Object with encompassing lifetime</param>
        /// <returns>Current instance</returns>
        ITierCoordinatorBuilderOwned CreateDataNavigator(INotifyDisposing lifetimeContext);
    }

    /// <summary>
    /// Fluent Factory interface for creating a Data Navigator Owner
    /// </summary>
    public interface ITierCoordinatorBuilderOwned
    {
        /// <summary>
        /// Persistence to be used by the IDataNavigationOwner
        /// </summary>
        /// <typeparam name="T">Editable entity Mirror</typeparam>
        /// <param name="persistedView">Persistence</param>
        /// <returns>Current instance</returns>
        ITierCoordinatorBuilderWithView UsingPersistedView<T>(IPersistedView<T> persistedView)
            where T : IEditableMirrorObject;
    }

    /// <summary>
    /// Fluent Factory interface for creating a Data Navigator Owner
    /// </summary>
    public interface ITierCoordinatorBuilderWithView
    {
        /// <summary>
        /// The owner of the next higher editable tier
        /// </summary>
        /// <remarks>
        /// Also used as the lifetime determiner for the ITierCoordinator
        /// instance built
        /// </remarks>
        /// <param name="parent">Tier owner</param>
        /// <returns>Current instance</returns>
        ITierCoordinatorBuilderWithView WithParent(ITierCoordinator parent);
        /// <summary>
        /// Configures the visual user interface for navigation and
        /// editing
        /// </summary>
        /// <param name="config">Configuration</param>
        /// <returns>Current instance</returns>
        ITierCoordinatorBuilderWithView ProvidingUIControlAt(IUIConfig config);
        /// <summary>
        /// Create the ITierCoordinator
        /// </summary>
        /// <returns>Created instance</returns>
        ITierCoordinator Build();
    }
}
