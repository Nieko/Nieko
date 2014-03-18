using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.ComponentModel.ViewModelling
{
    /// <summary>
    /// Defines a mapped ModelView type and its generation
    /// </summary>
    /// <remarks>
    /// A class inheriting from MappingDefinition will contain nested classes 
    /// implementing IModelViewDefinition
    /// </remarks>
    public interface IModelViewDefinition
    {
        /// <summary>
        /// Name of ModelView to generate
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Generates the C# code representing the ModelView class
        /// </summary>
        /// <returns></returns>
        string Generate();
    }
}
