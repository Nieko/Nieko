﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.ComponentModel;
using Nieko.Infrastructure.ViewModel;
using Nieko.Infrastructure.Composition;

namespace Nieko.Infrastructure.Windows
{
    /// <summary>
    /// Base class for providing Forms for EndPoints
    /// </summary>
    public abstract class FormsProvider
    {
        private Dictionary<EndPoint, ViewModelForm> _ViewModelForms = new Dictionary<EndPoint, ViewModelForm>();
        private List<ViewModelForm> _AllFormProperties = null;
        private IGenericSupplierBuilder _SupplierBuilder;

        protected internal ViewModelForm Get<TView, TViewModelInterface, TViewModel>(EndPoint endPoint, string regionName)
            where TView : class, IViewBase<TViewModelInterface>, new()
            where TViewModelInterface : IViewModelBase
            where TViewModel : class, TViewModelInterface
        {
            if (endPoint == null)
            {
                throw new ArgumentNullException("endPoint");
            }

            if (!endPoint.Type.CanHaveForm())
            {
                throw new ArgumentException("endPoint can not have forms");
            }

            if (typeof(TViewModel).IsAbstract || typeof(TViewModel).IsInterface || typeof(TViewModel).IsGenericTypeDefinition)
            {
                throw new ArgumentException(typeof(TViewModel).FullName + "is not a valid TViewModel argument. TViewModel must be a concrete type");
            }

            ViewModelForm viewModelForm = null;

            if (!_ViewModelForms.TryGetValue(endPoint, out viewModelForm))
            {
                var viewFactory = _SupplierBuilder.BuildSupplier<TView>();
                var viewModelSuppier = _SupplierBuilder.BuildSupplier<TViewModel>();

                Func<TViewModel> viewModelFactory = () =>
                {
                    var viewModel = viewModelSuppier();
                    viewModel.Load();

                    return viewModel;
                };

                viewModelForm = new ViewModelForm()
                {
                    EndPoint = endPoint,
                    ViewType = typeof(TView),
                    ViewModelType = typeof(TViewModel),
                    ViewFactory = () => viewFactory(),
                    ViewModelFactory = () => viewModelFactory(),
                    ViewModelSet = (v, vm) => 
                        {
                            (v as TView).ViewModel = (TViewModel)vm;
                        },
                    RegionName = regionName
                };

                _ViewModelForms.Add(endPoint, viewModelForm);
            }

            return viewModelForm;
        }

        protected internal ViewModelForm Get<TView, TViewModelInterface, TViewModel>(EndPoint endPoint)
            where TView : class, IViewBase<TViewModelInterface>, new()
            where TViewModelInterface : IViewModelBase
            where TViewModel : class, TViewModelInterface
        {
            return Get<TView, TViewModelInterface, TViewModel>(endPoint, CoreRegionNames.MainRegion); 
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="supplierBuilder">Generic Lazy instantiation factory</param>
        public FormsProvider(IGenericSupplierBuilder supplierBuilder)
        {
            _SupplierBuilder = supplierBuilder;
        }

        /// <summary>
        /// Returns all Forms declared by this class
        /// </summary>
        /// <returns>EndPoint Forms</returns>
        public IEnumerable<ViewModelForm> GetAllForms()
        {
            if (_AllFormProperties == null)
            {
                _AllFormProperties = GetType().GetProperties()
                    .Where(p => p.PropertyType == typeof(ViewModelForm))
                    .Select(p => (ViewModelForm)p.GetValue(this, null))
                    .ToList();
            }

            return _AllFormProperties;
        }
    }
}
