using Nieko.Infrastructure.ComponentModel;
using Nieko.Infrastructure.Windows;
using Nieko.Infrastructure.Windows.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Windows;

namespace Nieko.Infrastructure.ViewModel
{
    public abstract class NiekoForm<T> : DataViewModel
        where T : NiekoForm<T>
    {
        private static EndPoint _EndPoint;

        protected static EndPoint Define<TView>(EndPoint parent, Expression<Func<EndPoint>> property, string description, short ordinal)
            where TView : FrameworkElement
        {
            if(_EndPoint != null)
            {
                return _EndPoint;
            }

            if (typeof(T).IsAbstract || typeof(T).IsInterface || typeof(T).IsGenericTypeDefinition)
            {
                throw new ArgumentException(typeof(T).FullName + "is not a valid View Model. View Models must be a concrete type to define EndPoints");
            }

            string propertyName = BindingHelper.Name(property);

            _EndPoint = NiekoFormCatalog.GetEndPoint(parent, property)
                .Description(description)
                .CreateMenuEntry(true)
                .Ordinal(ordinal)
                .Build(); 

            NiekoFormCatalog.RegisterForm(factory =>
                {
                    ViewModelForm viewModelForm = null;

                    var viewFactory = factory.GetViewBuilder<TView>();
                    var viewModelSuppier = factory.GetViewModelBuilder<T>();

                    Func<T> viewModelFactory = () =>
                    {
                        var viewModel = viewModelSuppier();
                        viewModel.Load();

                        return viewModel;
                    };

                    viewModelForm = new ViewModelForm()
                    {
                        EndPoint = _EndPoint,
                        ViewType = typeof(TView),
                        ViewModelType = typeof(T),
                        ViewFactory = () => viewFactory(),
                        ViewModelFactory = () => viewModelFactory(),
                        ViewModelSet = (v, vm) =>
                        {
                            (v as TView).DataContext = (T)vm;
                        },
                        RegionName = CoreRegionNames.MainRegion
                    };

                    return viewModelForm;
                });

            return _EndPoint;
        }

        protected IGraphFactory GraphFactory { get; private set; }

        public NiekoForm(IGraphFactory graphFactory) : 
            base()
        {
            GraphFactory = graphFactory;
        }
    }
}
