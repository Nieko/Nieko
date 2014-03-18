using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.ComponentModel.ViewModelling
{
    /// <summary>
    /// Base model view definition whose generated classes should
    /// be treated as data objects rather than ModelViews
    /// </summary>
    /// <remarks>
    /// Generated classes do NOT implement <seealso cref="IEditableMirrorObject"/>, hold the PrimaryKey
    /// of a source entity or contain any other change mechanisms
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public abstract class DataDefinition<T> : ModelViewDefinition<T>
    {
        public override Type BaseType
        {
            get
            {
                return typeof(object);
            }
        }

        public override string Name
        {
            get
            {
                return GetType().BasicName(); 
            }
        }
    }
}
