using Nieko.Infrastructure.Windows.Data;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Navigation.RecordNavigation
{
    /// <summary>
    /// Provides sorting and filters against a IPersistedView
    /// </summary>
    /// <remarks>
    /// Changes to filters and sort orders are effected on <see cref="Apply"/>
    /// </remarks>
    public interface IRecordSearch
    {
        /// <summary>
        /// Filters applied against line item / view model representation level
        /// </summary>
        IList<IRecordFilterGroup> FilterGroups { get; }
        /// <summary>
        /// Sorting (applied at line item / view model level)
        /// </summary>
        IList<SortDescription> SortDescriptions { get; }
        /// <summary>
        /// Limits results to the number of items indicated.
        /// If zero, return all items
        /// </summary>
        long Take { get; set; }
        /// <summary>
        /// Clears all current filters and sort descriptions and
        /// finds the next matching item matching predicate (at line item / view model level)
        /// </summary>
        /// <remarks>
        /// Implementation should apply search immediately
        /// </remarks>
        /// <typeparam name="T">Type to cast line items to for filtering against</typeparam>
        /// <param name="filter">Filter to appy</param>
        /// <returns>True if a matching item is found</returns>
        bool QuickFind<T>(Func<T, bool> filter)
            where T : class;
        /// <summary>
        /// Applies the current filters and sorting
        /// </summary>
        void Apply();
    }
}
