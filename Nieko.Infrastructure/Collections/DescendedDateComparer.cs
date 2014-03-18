using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Collections
{
    /// <summary>
    /// Descending order IComparer[T] Implementation for DateTime values
    /// </summary>
    public class DescendedDateComparer : IComparer<DateTime>
    {
        public int Compare(DateTime x, DateTime y)
        {
            // use the default comparer to do the original comparison for DateTimes
            int ascendingResult = Comparer<DateTime>.Default.Compare(x, y);

            // turn the result around
            return 0 - ascendingResult;
        }
    }
}
