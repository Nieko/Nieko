using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;
using Microsoft.CSharp;
using System.CodeDom;
using Nieko.Infrastructure.ViewModel;
using System.Data.Objects.DataClasses;
using System.Data.Metadata.Edm;

namespace Nieko.Infrastructure.ComponentModel.ViewModelling
{
    /// <summary>
    /// Strongly typed mapping definition that checks at compile time the
    /// mappings and allows definitions to be built from Expressions to entity properties 
    /// </summary>
    /// <typeparam name="T">Entity to define a ModelView for</typeparam>
    public abstract class ModelViewDefinition<T> : IModelViewDefinition
    {
        private List<string> _PropertyNames = new List<string>();
        private List<string> _PropertyTypes = new List<string>();
        private List<string> _NonNullablePropertyNames = new List<string>();

        public virtual string Name
        {
            get
            {
                return GetType().BasicName() + "LineItem";
            }
        }

        public virtual Type BaseType
        {
            get
            {
                return typeof(EditableViewModel);
            }
        }

        public virtual bool HasNotifyingFields
        {
            get
            {
                return BaseType != typeof(object);
            }
        }

        public string Generate()
        {
            var template = new ModelViewTemplate();

            template.Session = new Dictionary<string, object>();
            template.Session["ClassName"] = Name;
            template.Session["BaseType"] = BaseType;
            template.Session["HasNotifyingFields"] = HasNotifyingFields;
            template.Session["PropertyTypes"] = _PropertyTypes.ToArray();
            template.Session["PropertyNames"] = _PropertyNames.ToArray();
            template.Session["NonNullableProperties"] = _NonNullablePropertyNames.ToArray();

            template.Initialize();
            return template.TransformText();
        }

        public ModelViewDefinition()
        {
            DefineProperties(); 
        }

        protected void Add<TProperty>(string propertyName, Expression<Func<T, TProperty>> mapExpression)
        {
            string csharpTypeName;

            using (var p = new CSharpCodeProvider())
            {
                csharpTypeName = p.GetTypeOutput(new CodeTypeReference(typeof(TProperty)));
            }

            _PropertyNames.Add(propertyName);
            _PropertyTypes.Add(csharpTypeName);

            if (IsNonNullableScalar(mapExpression) || IsNonNullableReference(mapExpression))
            {
                _NonNullablePropertyNames.Add(propertyName);
            }
        }

        protected void Add<TProperty>(Expression<Func<T, TProperty>> mapExpression)
        {
            var segments = BindingHelper.Name(mapExpression).Split('.');

            if (segments.Length > 1)
            {
                Add(segments[segments.Length - 1], mapExpression);
            }
            else
            {
                Add(BindingHelper.Name(mapExpression), mapExpression);
            }
        }

        protected void Add<TProperty>(string propertyName)
        {
            string csharpTypeName;

            using (var p = new CSharpCodeProvider())
            {
                csharpTypeName = p.GetTypeOutput(new CodeTypeReference(typeof(TProperty)));
            }

            _PropertyNames.Add(propertyName);
            _PropertyTypes.Add(csharpTypeName);
        }

        protected void AddRef<TProperty>(Expression<Func<T, TProperty>> mapExpression)
        {
            var segments = BindingHelper.Name(mapExpression).Split('.');

            if (segments.Length > 1)
            {
                Add(segments[segments.Length - 2], mapExpression);
            }
            else
            {
                Add(BindingHelper.Name(mapExpression), mapExpression);
            }
        }

        protected abstract void DefineProperties();

        private bool IsNonNullableScalar<TProperty>(Expression<Func<T, TProperty>> mapExpression)
        {
            EdmScalarPropertyAttribute edmScalarDetails = null;

            if (typeof(MemberExpression) != mapExpression.Body.GetType())
            {
                return false;
            }

            var scalarProperty = typeof(T).GetProperty(BindingHelper.Name(mapExpression));

            if (scalarProperty == null)
            {
                return false;
            }

            edmScalarDetails = Attribute.GetCustomAttribute(scalarProperty, typeof(EdmScalarPropertyAttribute)) as EdmScalarPropertyAttribute;

            return edmScalarDetails != null && !edmScalarDetails.IsNullable; 
            
        }

        private bool IsNonNullableReference<TProperty>(Expression<Func<T, TProperty>> mapExpression)
        {
            EdmRelationshipNavigationPropertyAttribute navigationDetails = null;
            EdmRelationshipAttribute relationshipDetails = null;

            if (typeof(MemberExpression) != mapExpression.Body.GetType())
            {
                return false;
            }

            var referenceProperty = typeof(T).GetProperty(BindingHelper.Name(mapExpression));

            if (referenceProperty == null)
            {
                return false;
            }

            navigationDetails = Attribute.GetCustomAttribute(referenceProperty, typeof(EdmRelationshipNavigationPropertyAttribute)) as EdmRelationshipNavigationPropertyAttribute;
            if (navigationDetails == null)
            {
                return false;
            }

            relationshipDetails = Attribute.GetCustomAttributes(typeof(T).Assembly, typeof(EdmRelationshipAttribute))
                .FirstOrDefault(a =>
                    {
                        var details = (a as EdmRelationshipAttribute);

                        return details.RelationshipName == navigationDetails.RelationshipName &&
                            (
                                (details.Role2Type == typeof(T) && details.Role1Multiplicity == RelationshipMultiplicity.One) ||
                                (details.Role1Type == typeof(T) && details.Role2Multiplicity == RelationshipMultiplicity.One)
                            );
                    }) as EdmRelationshipAttribute;

            return relationshipDetails != null;
        }
    }
}
