using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Nieko.Infrastructure.Data;
using System.Xml.Serialization;

namespace Nieko.Infrastructure.ComponentModel
{
    /// <summary>
    /// Model View of an entity allowing commit, rollback and 
    /// mapping to and from source entity.
    /// </summary>
    public interface IEditableMirrorObject : IEditableObject
    {
        [XmlIgnore]
        PrimaryKey SourceKey { get; set; }
        [XmlIgnore]
        bool IsReadOnly { get; }
        [XmlIgnore]
        bool IsEditing { get; }
        [XmlIgnore]
        bool SuppressNotifications { get; set; }
        [XmlIgnore]
        bool HasChanged { get; set; }
    }
}