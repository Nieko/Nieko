using Nieko.Infrastructure.Collections;
using Nieko.Infrastructure.ComponentModel;
using Nieko.Infrastructure.Navigation.RecordNavigation;
using Nieko.Infrastructure.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Data.Objects.DataClasses;
using System.ComponentModel;
using Nieko.Infrastructure.Data;
using System.Linq.Expressions;
using Nieko.Infrastructure.ViewModel;

namespace Nieko.Modules.Navigation.RecordNavigator
{
    public class RecordFilter : NotifyPropertyChangedBase, IRecordFilter
    {
        private static MethodInfo _BuildExpressionMethod;
        private static Dictionary<string, Func<Expression, ConstantExpression, BinaryExpression>> _ComparisonsByName;
        private static List<string> _ComparisonOptions;
        private static HashSet<string> _NonDataProperties;

        private Type _BaseItemType;
        private Func<Type, ParameterExpression, BinaryExpression> _BuildExpressionAction = null;
        private List<string> _PropertiesPath = new List<string>();
        private List<string> _CompletePath = new List<string>();
        private List<Type> _TypesPath = new List<Type>();
        private ObservableRangeCollection<string> _PathOptions = new ObservableRangeCollection<string>();
        private Dictionary<string, string> _PropertiesByPath = new Dictionary<string,string>();
        private Dictionary<string, Type> _PropertyTypesByName = new Dictionary<string,Type>();

        public static string NoPropertySelected { get { return string.Empty; } }

        public string Description
        {
            get
            {
                return Get(() => Description);
            }
            set
            {
                Set(() => Description, value);
            }
        }

        public string Path
        {
            get
            {
                return Get(() => Path);
            }
            set
            {
                Set(() => Path, value, () =>
                    {
                        UpdateOptions();
                        Description = Path;
                    });
            }
        }

        public string Comparison
        {
            get
            {
                return Get(() => Comparison);
            }
            set
            {
                Set(() => Comparison, value);
            }
        }

        public string Filter
        {
            get
            {
                return Get(() => Filter);
            }
            set
            {
                Set(() => Filter, value);
            }
        }

        public IList<string> PathOptions
        {
            get
            {
                return _PathOptions;
            }
        }

        public IList<string> ComparisonOptions
        {
            get
            {
                return _ComparisonOptions;
            }
        }

        public void Initialize(Type baseItemType)
        {
            _BaseItemType = baseItemType;

            _BuildExpressionAction = (t, p) =>
                {
                    return (BinaryExpression)_BuildExpressionMethod
                        .MakeGenericMethod(baseItemType, t)
                        .Invoke(this, new []{p});
                };

            UpdateOptions();
        }

        public RecordFilter()
        {
            SetDefault(() => Description, string.Empty);
            SetDefault(() => Path, string.Empty);
            SetDefault(() => Comparison, "=");
            SetDefault(() => Filter, string.Empty);
        }

        static RecordFilter()
        {
            _BuildExpressionMethod = typeof(RecordFilter)
                .GetMethod("BuildExpression");

            _ComparisonsByName = new Dictionary<string, Func<Expression, ConstantExpression, BinaryExpression>>()
            {
                { "=", (l,r) => Expression.Equal(l,r) },
                { ">", (l,r) => Expression.GreaterThan(l,r) },
                { ">=", (l,r) => Expression.GreaterThanOrEqual(l,r) },
                { "<", (l,r) => Expression.LessThan(l,r) },
                { "<=", (l,r) => Expression.LessThanOrEqual(l,r) },
                { "!=", (l,r) => Expression.NotEqual(l,r) },
                { 
                    "Like",
                    (l,r) =>
                    {
                        var regex = new System.Text.RegularExpressions.Regex(r.Value.ToString().Replace("*", ".+?"));
                        Func<string, bool> isLike = ls =>
                            {
                                return regex.IsMatch(ls);
                            };

                        return Expression.Equal(
                            Expression.Convert(
                                Expression.Call(Expression.Constant(isLike.Target), isLike.Method, l),
                                typeof(bool)),
                            Expression.Constant(true));
                    }
                }
            };
            _ComparisonOptions = _ComparisonsByName.Keys
                .ToList();

            _NonDataProperties = new HashSet<string>(
                new[] 
                {
                    typeof(IEditableMirrorObject),
                    typeof(INotifyPropertyChanged),
                    typeof(EditableViewModel),
                    typeof(IDataErrorInfo)
                }
                .SelectMany(p => p
                    .GetProperties(BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public))
                .Select(p => p.Name)
                .Distinct());
        }

        private void UpdateOptions()
        {
            if(!(Path == NoPropertySelected ||
                        (_PropertiesByPath.ContainsKey(Path)  
                        && !_PropertyTypesByName[_PropertiesByPath[Path]].IsPrimitive
                        && !_PropertyTypesByName[_PropertiesByPath[Path]].IsBoxedType())
                ))
            {
                return;
            }

            Type currentType;

            if(Path == NoPropertySelected)
            {
                currentType = _BaseItemType;

                _TypesPath.Clear();
                _CompletePath.Clear();
                _PropertiesPath.Clear();
            }
            else
            {
                currentType = _PropertyTypesByName[_PropertiesByPath[Path]];

                _CompletePath.Add(Path);
                _PropertiesPath.Add(_PropertiesByPath[Path]);
            }

            _TypesPath.Add(currentType);

            var pathPrefix =
                _CompletePath.Any() ?
                _CompletePath
                .Aggregate((total, current) => total + "." + current) :
                string.Empty;
            string path;
            var newPathOptions = new List<string>();

            _PathOptions.Clear();
            _PropertiesByPath.Clear();
            _PropertyTypesByName.Clear();

            foreach(var property in currentType.GetProperties(BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance) 
                .Where(p => !_NonDataProperties.Contains(p.Name)))
            {
                string description;
                var descriptionAttribute = (DescriptionAttribute)Attribute.GetCustomAttribute(property, typeof(DescriptionAttribute));
  
                if(descriptionAttribute == null)
                {
                    description = property.Name; 
                }
                else
                {
                    description = descriptionAttribute.Description;
                }

                path = (string.IsNullOrEmpty(pathPrefix) ?
                    "" :
                    pathPrefix + ".") + description;

                newPathOptions.Add(path);
                _PropertiesByPath.Add(path, property.Name);
                _PropertyTypesByName.Add(property.Name, property.PropertyType);
            }

            _PathOptions.AddRange(newPathOptions);

            if(typeof(IPrimaryKeyed).IsAssignableFrom(currentType))
            {
                var keyProperties = PrimaryKey.GetKeyProperties(currentType)
                    .Select(kp => new 
                    { 
                        Name = _PropertiesByPath
                            .First(pbp => pbp.Value == kp.Key)
                            .Key,
                        PropertyType = _PropertyTypesByName[kp.Key] 
                    });

                var primitiveKeys = keyProperties
                    .Where(kp => kp.PropertyType.IsPrimitive || kp.PropertyType.IsBoxedType());

                if(primitiveKeys.Any())
                {
                    Path = primitiveKeys.First().Name;
                }
                else
                {
                    Path = keyProperties.First().Name;
                }
            }
            else
            {
                var primitiveProperties = _PathOptions
                    .Select(po => new { Path = po, PropertyType = _PropertyTypesByName[_PropertiesByPath[po]] })
                    .Where(pt => pt.PropertyType.IsPrimitive || pt.PropertyType.IsBoxedType());

                if(primitiveProperties.Any())
                {
                    Path = primitiveProperties
                        .First()
                        .Path; 
                }
                else
                {
                    Path = _PathOptions[0];
                }
            }
        }

        public BinaryExpression BuildPathExpression(ParameterExpression itemParameter)
        {
            return _BuildExpressionAction(_PropertyTypesByName[_PropertiesByPath[Path]], itemParameter);
        }

        public BinaryExpression BuildExpression<T, TPathValue>(ParameterExpression itemParameter)
        {
            Expression currentLevel = itemParameter;
            var lastPropertyType = _PropertyTypesByName[_PropertiesByPath[Path]];

            var typeHierachy =
                new [] { _BaseItemType }
                .Union(_TypesPath)
                .Union(new [] { lastPropertyType })
                .ToList();
            var propertiesHierachy =
                _PropertiesPath
                .Union( new [] { _PropertiesByPath[Path] })
                .ToList();

            for(int i = 0;i < propertiesHierachy.Count;i++ )
            {
                var parentType = i > 0 ?
                    typeHierachy[i - 1] :
                    _BaseItemType;
                var propertyType = typeHierachy[i + 1];

                //Expression newLevel = Expression.Lambda(
                //    typeof(Func<,>).MakeGenericType(parentType, propertyType),
                //    Expression.PropertyOrField(currentLevel, propertiesHierachy[i]),
                //    i == 0 ? new[] { parameter } : new ParameterExpression[] { });

                Expression newLevel =
                    Expression.PropertyOrField(currentLevel, propertiesHierachy[i]);

                currentLevel = newLevel;
            }

            ConstantExpression filterExpression;

            try
            {
                var actualValue = TypeDescriptor.GetConverter(lastPropertyType).ConvertFromInvariantString(Filter);
                filterExpression = Expression.Constant(actualValue, lastPropertyType);
            }
            catch(NotSupportedException)
            {
                //nom nom Eat Exception nom nom
                filterExpression = lastPropertyType.AsDefaultExpression();
            }

            return _ComparisonsByName[Comparison](currentLevel, filterExpression);
        }
    }
}
