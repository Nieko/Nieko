using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Nieko.Infrastructure.ComponentModel
{
    /// <summary>
    /// Resulting UI indicator for an EndPoint
    /// </summary>
    public enum EndPointType
    {
        Root,
        Form,
        Report,
        ReportPage,
        SubMenu
    }

    public static class EndPointTypeExtensions
    {
        public static bool CanHaveForm(this EndPointType type)
        {
            return type != EndPointType.SubMenu;
        }
    }

    /// <summary>
    /// Destination for a navigation action
    /// </summary>
    /// <remarks>
    /// EndPoints define in abstract the navigation structure of an application.
    /// EndPoints are declared as static properties on a class implementing
    /// IEndPointProvider (usually in an Infrastructure assembly).
    /// An EndPoint is passed as the determining parameter for a navigation request.
    /// The actual action of an EndPoint can then be implemented (where required) 
    /// in a module assembly enabling loose coupling between navigation and UI 
    /// artefacts (i.e. MVVM concerns, data dependencies etc.)
    /// </remarks>
    public class EndPoint
    {
        internal static Func<EndPoint, bool> CanAddCheck { get; set; }

        /// <summary>
        /// The class in which the static property declaring the EndPoint is a member 
        /// </summary>
        public Type CreatedBy { get; internal set; }

        internal List<EndPoint> ChildrenInternal { get; private set; }

        /// <summary>
        /// Name of the EndPoint. Normally the name of the declaring static property
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Display text.
        /// </summary>
        public string Description { get; internal set; }

        /// <summary>
        /// Whether or not this EndPoint requires a entry in the applications menu system
        /// </summary>
        public bool CreateMenuEntry { get; internal set; }

        /// <summary>
        /// Display priority
        /// </summary>
        public short Ordinal { get; internal set; }

        /// <summary>
        /// Type of UI action this EndPoint indicates
        /// </summary>
        public EndPointType Type { get; internal set; }

        /// <summary>
        /// EndPoint having this as a child
        /// </summary>
        public EndPoint Parent { get; internal set; }

        /// <summary>
        /// EndPoints with this as a parent
        /// </summary>
        public ReadOnlyCollection<EndPoint> Children { get; private set; }

        /// <summary>
        /// Data to be passed to the action implementing the EndPoint
        /// </summary>
        public object MetaContext { get; set; }

        /// <summary>
        /// Top-most member of the EndPoints' hierarchy
        /// </summary>
        public static EndPoint Root { get; private set; }

        /// <summary>
        /// Direct public construction of EndPoints is discouraged so that they may be validated 
        /// by the IEndPointValidation and exist properly as static properties within
        /// IEndPointProvider classes
        /// </summary>
        internal EndPoint()
        {
            Description = string.Empty;
            Ordinal = 0;
            ChildrenInternal = new List<EndPoint>();
            Children = new ReadOnlyCollection<EndPoint>(ChildrenInternal);
        }

        static EndPoint()
        {
            Root = new EndPoint()
            {
                CreatedBy = typeof(EndPoint),
                Name = "Root",
                Description = "(Root)",
                CreateMenuEntry = false,
                Type = EndPointType.Root,
                Parent = null
            };
        }

        private string GetPath(bool menuItemsOnly)
        {
            if(this == Root)
            {
                return "/";
            }

            return Parent.GetPath(menuItemsOnly) +
                ((CreateMenuEntry || !menuItemsOnly) ?
                    (Parent == Root ? string.Empty : "/") + Description :
                    string.Empty);
        }

        /// <summary>
        /// Full backslash (/) delimited description of hierarchy to this EndPoint
        /// </summary>
        /// <returns></returns>
        public string GetFullPath()
        {
            return GetPath(false);
        }

        /// <summary>
        /// Backslash (/) delimited description of hierarchy to this EndPoint requiring Menu entries
        /// </summary>
        /// <returns></returns>
        public string GetMenuPath()
        {
            return GetPath(true);
        }

        /// <summary>
        /// Create a object for use in serializing / de-serializing EndPoint <seealso cref="MetaContext"/>
        /// </summary>
        /// <param name="dataType">Type of MetaContext object</param>
        /// <returns>New instance of Serializer</returns>
        public static IEndPointContextSerializer CreateContextSerializer(Type dataType)
        {
            Type serializerType;

            if (!EndPointContextSerializerAttribute.DataTypeSerializers.TryGetValue(dataType, out serializerType))
            {
                serializerType = (Attribute.GetCustomAttribute(dataType, typeof(EndPointContextSerializerAttribute)) as EndPointContextSerializerAttribute)
                    .Serializer;

                EndPointContextSerializerAttribute.DataTypeSerializers.Add(dataType, serializerType);
            }

            return Activator.CreateInstance(serializerType) as IEndPointContextSerializer;
        }
    }
}
