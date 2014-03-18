using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Logging
{
    public enum SeverityCategory
    {
        /// <summary>
        /// Debug category.
        /// </summary>
        Debug,

        /// <summary>
        /// Exception category.
        /// </summary>
        Exception,

        /// <summary>
        /// Informational category.
        /// </summary>
        Info,

        /// <summary>
        /// Warning category.
        /// </summary>
        Warn
    }
}
