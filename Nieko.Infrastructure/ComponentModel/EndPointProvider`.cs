using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace Nieko.Infrastructure.ComponentModel
{
    /// <summary>
    /// Common base class for IEndPointProviders. Provides a fluent interface for 
    /// declaring EndPoints
    /// </summary>
    /// <typeparam name="T">Type of the class inheriting from EndPointProvider[T]</typeparam>
    public abstract class EndPointProvider<T> : IEndPointProvider
        where T : EndPointProvider<T> 
    {
        private static EndPointFactory<T> _EndPointFactory = new EndPointFactory<T>();
        private List<EndPoint> _EndPoints;

        /// <summary>
        /// Fluent factory method for creating an EndPoint.
        /// </summary>
        /// <param name="parent">Parent EndPoint. Top-level user EndPoints should use EndPoint.Root</param>
        /// <param name="property">EndPoint name. Must be a static property on calling class i.e. () => Reports</param>
        /// <param name="type">Type of EndPoint</param>
        /// <returns></returns>
        protected static IEndPointFactory Get(EndPoint parent, Expression<Func<EndPoint>> property, EndPointType type)
        {
            return _EndPointFactory.Create<T>(parent, property, type);
        }

        public IEnumerable<EndPoint> GetEndPoints()
        {
            if (_EndPoints == null)
            {
                // Use of reflection acceptable as this section is only called once per concrete IEndPointProvider class
                _EndPoints = GetType().GetProperties(BindingFlags.Public | BindingFlags.Static)
                    .Where(p => p.PropertyType == typeof(EndPoint))
                    .Select(p => (EndPoint)p.GetValue(this, null))
                    .ToList();
            }

            return _EndPoints;
        }
    }
}
