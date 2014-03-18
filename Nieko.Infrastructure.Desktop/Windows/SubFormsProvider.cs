using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.ComponentModel;
using Nieko.Infrastructure.ViewModel;
using Nieko.Infrastructure.Composition;

namespace Nieko.Infrastructure.Windows
{
    /// <summary>
    /// FormsProvider facade for declaring a Suite's
    /// EndPoints' Forms
    /// </summary>
    /// <typeparam name="T">Form Suite</typeparam>
    public abstract class SubFormsProvider<T> : FormsProvider
        where T : FormsSuiteBase<T>, IViewModelBase
    {
        public ViewModelForm GetSubForm<TView, TViewModel>(EndPoint endPoint, string regionName)
            where TView : class, IViewBase<ISuiteFormViewModel<T>>, new()
            where TViewModel : class, ISuiteFormViewModel<T>
        {
            return Get<TView, ISuiteFormViewModel<T>, TViewModel>(endPoint, regionName);
        }

        public SubFormsProvider(IGenericSupplierBuilder supplierBuilder) : base(supplierBuilder) { }
    }
}
