using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Nieko.Infrastructure.ComponentModel.ViewModelling
{
    /// <summary>
    /// Contains nested classes defining ModelView mappings to entity classes
    /// </summary>
    public abstract class MappingDefinition
    {
        /// <summary>
        /// Find all nested types within this class that implement IModelViewDefinition
        /// and aggregate their C# code representations into a string
        /// </summary>
        /// <returns>C# ModelView code file text</returns>
        public string Generate()
        {
            var output = new StringBuilder();
            IModelViewDefinition definition;

            output.AppendLine("using System;");
            output.AppendLine("using System.Collections.Generic;");
            output.AppendLine();
            output.AppendLine("namespace " + GetType().Namespace);
            output.AppendLine("\t{");

            foreach (var type in GetType().GetNestedTypes()
                .Where(t => !(t.IsAbstract || t.IsInterface) && typeof(IModelViewDefinition).IsAssignableFrom(t))) 
            {
                definition = (Activator.CreateInstance(type) as IModelViewDefinition);
                output.AppendLine(definition.Generate());
            }

            output.AppendLine("}");

            return output.ToString();
        }
    }
}
