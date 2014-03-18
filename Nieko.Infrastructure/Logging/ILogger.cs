using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Logging
{
    /// <summary>
    /// Simplified Logger facade
    /// </summary>
    /// <remarks>
    /// It is the responsibility of the application using this framework to 
    /// provide an implementation (i.e. via Prism & NLog)
    /// </remarks>
    public interface ILogger
    {
        /// <summary>
        /// Logs a message with the default Category
        /// </summary>
        /// <param name="message">Log</param>
        void Log(string message);
        /// <summary>
        /// Logs a message
        /// </summary>
        /// <param name="message">Log</param>
        /// <param name="category">Severity Category</param>
        void Log(string message, SeverityCategory category);
        /// <summary>
        /// Logs an exception with prefixing heading 
        /// </summary>
        /// <param name="prefix">Heading to prefix log with</param>
        /// <param name="e">Exception to log</param>
        void LogException(string prefix, Exception e);

        /// <summary>
        /// Logs an exception with a default prefixing heading 
        /// </summary>
        /// <param name="e">Exception to log</param>
        void LogException(Exception e);
    }
}
