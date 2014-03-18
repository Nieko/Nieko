using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Collections;
using System.Reflection;

namespace System.Data.Objects.DataClasses
{
    public static class EntityTypeExtensions
    {
        private static Dictionary<Type, IEnumerable<string>> _EntityReferencesByType = new Dictionary<Type, IEnumerable<string>>();

        private static List<string> _EntityObjectMappingExceptions;

        private static List<string> EntityObjectMappingExceptions
        {
            get
            {
                if (_EntityObjectMappingExceptions == null)
                {
                    _EntityObjectMappingExceptions =
                        typeof(EntityObject).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Select(p => p.Name)
                        .ToList();

                    _EntityObjectMappingExceptions.Add("Id");
                }

                return _EntityObjectMappingExceptions;
            }
        }

        /// <summary>
        /// Returns the names of all properties (reference and value) on a type
        /// and excludes those part of EntityObject or its implementation
        /// </summary>
        /// <param name="entityType">Type to retrieve properties from</param>
        /// <returns>Names of matching properties</returns>
        public static ISet<string> NonDataProperties(this Type entityType)
        {
            IEnumerable<string> entityReferenceExceptions = null;

            if (!_EntityReferencesByType.TryGetValue(entityType, out entityReferenceExceptions))
            {
                entityReferenceExceptions = entityType.GetProperties()
                    .Where(p => typeof(RelatedEnd).IsAssignableFrom(p.PropertyType))
                    .Select(p => p.Name);

                _EntityReferencesByType.Add(entityType, entityReferenceExceptions); 
            }

            return new HashSet<string>(entityReferenceExceptions.Union(EntityObjectMappingExceptions));
        }
    }
}
