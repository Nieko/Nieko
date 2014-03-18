using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Composition;
using System.Linq.Expressions;
using System.Reflection;

namespace Nieko.Modules.Architecture
{
    internal class GenericSupplierBuilder : IGenericSupplierBuilder
    {
        private Func<Type, object> _SupplierBuilder;
        private MethodInfo _ObjectSupplier = null;
        
        public GenericSupplierBuilder(Func<Type, object> supplierBuilder)
        {
            _SupplierBuilder = supplierBuilder;
        }

        public Func<T> BuildSupplier<T>()
        {
            return (_SupplierBuilder(typeof(T)) as Func<T>);
        }

        public Func<object> BuildSupplier(Type type)
        {
            if (_ObjectSupplier == null)
            {
                _ObjectSupplier = GetType().GetMethod("GetObjectFactory"); 
            }

            var typeObjectFactory = Expression.Lambda<Func<Func<object>>>(
                Expression.Call(
                    Expression.Constant(this),
                    _ObjectSupplier.MakeGenericMethod(type)))
                .Compile();

            return typeObjectFactory();
        }

        public Func<object> GetObjectFactory<T>()
        {
            var factory = BuildSupplier<T>();

            return () => factory();
        }
    }
}
