using Nieko.Infrastructure.ComponentModel;
using Nieko.Infrastructure.Composition;
using Nieko.Infrastructure.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Modules.Navigation
{
    public class ViewModelFormFactory : IViewModelFormFactory
    {
        private IGenericSupplierBuilder _SupplierBuilder;

        public Func<T> GetViewBuilder<T>()
        {
            return _SupplierBuilder.BuildSupplier<T>(); 
        }

        public Func<T> GetViewModelBuilder<T>()
        {
            return _SupplierBuilder.BuildSupplier<T>();
        }

        public ViewModelFormFactory(IGenericSupplierBuilder supplierBuilder)
        {
            _SupplierBuilder = supplierBuilder;
        }
    }
}
