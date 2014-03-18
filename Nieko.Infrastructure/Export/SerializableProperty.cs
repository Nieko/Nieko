using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Linq.Expressions;

namespace Nieko.Infrastructure.Export
{
    internal class SerializableProperty
    {
        private Func<Func<object, object>> _AccessorBuilder = null;
        private Func<object, object> _Accessor;

        public string Name { get; private set; }

        public Func<object, object> Accessor 
        {
            get
            {
                if (_Accessor == null)
                {
                    _Accessor = _AccessorBuilder();
                    _AccessorBuilder = null;
                }

                return _Accessor;
            }
        }

        public bool IsPrimitive { get; private set; }

        internal SerializableProperty(PropertyInfo propertyInfo)
        {
            var treatAsPrimitive = (propertyInfo.PropertyType.IsPrimitive || propertyInfo.PropertyType.IsBoxedType());

            Name = propertyInfo.Name;
            _AccessorBuilder = () => BuildAccessor(propertyInfo);
            IsPrimitive = treatAsPrimitive || (propertyInfo.PropertyType != typeof(object) && !typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType) && propertyInfo.PropertyType.IsFrameworkType());
        }

        private Func<object, object> BuildAccessor(PropertyInfo propertyInfo)
        {
            var instanceParameter = Expression.Parameter(typeof(object), "o");

            return Expression.Lambda<Func<object, object>>(
                Expression.Convert( 
                    Expression.PropertyOrField(
                        Expression.Convert(
                            instanceParameter,
                            propertyInfo.DeclaringType),
                        propertyInfo.Name),
                    typeof(object)),
                    instanceParameter)
                        .Compile(); 
        }
    }
}
