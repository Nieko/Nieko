using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace Nieko.Infrastructure.Export
{
    /// <summary>
    /// Converts an object graph into xml and sends it to a given stream.
    /// </summary>
    /// <remarks>
    /// Export is executed as a dump of the graph to text
    /// rather than as an accurate representation of the objects
    /// </remarks>
    public class XmlExporter
    {
        private static Dictionary<Type, List<SerializableProperty>> _PropertiesByType = new Dictionary<Type, List<SerializableProperty>>();

        private int _TabLevel;
   
        public void Export(StreamWriter streamWriter, object root)
        {
            var stringBuilder = new StringBuilder();
            var data = root;

            if (data is IExportRootOverridden)
            {
                data = (data as IExportRootOverridden).GetExportRoot();
            }

            stringBuilder.AppendLine(@"<?xml version=""1.0"" encoding=""utf-16""?>");

            if (data != null)
            {
                stringBuilder.AppendLine("<" + data.GetType().BasicName() + @" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">");
                _TabLevel = 1;
                ExtractData(data, stringBuilder);
                stringBuilder.AppendLine("</" + data.GetType().BasicName() + ">");
            }

            streamWriter.WriteLine(stringBuilder.ToString());
            streamWriter.Flush();
        }

        private void ExtractData(object instance, StringBuilder stringBuilder)
        {
            object value;
            var properties = GetSerializableAccessors(instance.GetType());
            bool isPrimitive = false;

            foreach (var property in properties)
            {
                stringBuilder.Append(new String('\t', _TabLevel) + "<" + property.Name + ">");
                _TabLevel++;
                value = property.Accessor(instance);
                isPrimitive = (value == null && property.IsPrimitive) ||
                    (value.GetType().IsPrimitive || value.GetType().IsBoxedType())
                    || (!typeof(IEnumerable).IsAssignableFrom(value.GetType()) && value.GetType().IsFrameworkType());

                if (isPrimitive)
                {
                    stringBuilder.Append(value == null ? string.Empty : value.ToString());
                    _TabLevel--;
                }
                else
                {
                    stringBuilder.AppendLine();
                    if (value != null)
                    {
                        if (typeof(IEnumerable).IsAssignableFrom(value.GetType()))
                        {
                            foreach (var child in (value as IEnumerable))
                            {
                                stringBuilder.Append(new String('\t', _TabLevel) + "<" + child.GetType().BasicName() + ">");
                                if (child.GetType().IsPrimitive || child.GetType().IsBoxedType())
                                {
                                    stringBuilder.Append(child == null ? string.Empty : child.ToString());
                                }
                                else
                                {
                                    stringBuilder.AppendLine();
                                    _TabLevel++;
                                    ExtractData(child, stringBuilder);
                                    _TabLevel--;
                                    stringBuilder.Append(new String('\t', _TabLevel));
                                }
                                stringBuilder.AppendLine("</" + child.GetType().BasicName() + ">");
                            }
                        }
                        else
                        {
                            ExtractData(value, stringBuilder);
                        }
                    }
                    _TabLevel--;
                    stringBuilder.Append(new String('\t', _TabLevel));
                }
                
                stringBuilder.AppendLine("</" + property.Name + ">");
            }
        }

        private static List<SerializableProperty> GetSerializableAccessors(Type type)
        {
            List<SerializableProperty> properties = null;

            if (!_PropertiesByType.TryGetValue(type, out properties))
            {
                properties = type.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public)
                    .Where(p => !(Attribute.IsDefined(p, typeof(NonSerializedAttribute)) || Attribute.IsDefined(p, typeof(XmlIgnoreAttribute))) &&
                        (Attribute.IsDefined(p.PropertyType, typeof(SerializableAttribute)) ||
                        p.PropertyType.IsBoxedType() || p.PropertyType.IsPrimitive ||   
                        typeof(IEnumerable).IsAssignableFrom(p.PropertyType)) &&
                        ! p.GetIndexParameters().Any())
                    .Select(p => new SerializableProperty(p))
                    .ToList();

                _PropertiesByType.Add(type, properties); 
            }

            return properties;
        }
    }
}
