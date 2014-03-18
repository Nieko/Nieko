using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Navigation.RecordNavigation;
using Nieko.Infrastructure;
using Nieko.Infrastructure.Collections;

namespace Nieko.Modules.Navigation.RecordNavigator
{
    public class DataNavigatorFactory : IDataNavigatorFactory
    {
        private Func<IDataNavigator> _DataNavigatorWithoutViewFactory;
        private Func<IDataNavigatorViewModel> _DataNavigatorWithViewFactory;
        private Dictionary<INotifyDisposing, HashSet<IDataNavigatorViewModel>> _ViewModelNavigators;

        public DataNavigatorFactory(Func<IDataNavigator> dataNavigatorWithoutViewFactory, Func<IDataNavigatorViewModel> dataNavigatorWithViewFactory)
        {
            _DataNavigatorWithoutViewFactory = dataNavigatorWithoutViewFactory;
            _DataNavigatorWithViewFactory = dataNavigatorWithViewFactory;
            _ViewModelNavigators = new Dictionary<INotifyDisposing, HashSet<IDataNavigatorViewModel>>();
        }

        public IDataNavigator CreateInstance(INotifyDisposing viewModel, bool hasView)
        {
            if (hasView)
            {
                return CreateInstanceWithView(viewModel);
            }
            else
            {
                return CreateInstanceWithoutView(viewModel);
            }
        }

        private IDataNavigator CreateInstanceWithView(INotifyDisposing viewModel)
        {
            HashSet<IDataNavigatorViewModel> dataNavigators;

            IDataNavigatorViewModel instance = _DataNavigatorWithViewFactory();

            if (!_ViewModelNavigators.TryGetValue(viewModel, out dataNavigators))
            {
                dataNavigators = new HashSet<IDataNavigatorViewModel>();
                _ViewModelNavigators.Add(viewModel, dataNavigators);
                viewModel.Disposing += ViewModelDisposing;
            }

            dataNavigators.Add(instance);

            return instance;
        }

        private IDataNavigator CreateInstanceWithoutView(INotifyDisposing viewModel)
        {
            var instance = _DataNavigatorWithoutViewFactory();

            EventHandler disposingHandler = null;

            disposingHandler = (sender, args) =>
                {
                    viewModel.Disposing -= disposingHandler;
                    instance.Dispose();
                };

            viewModel.Disposing += disposingHandler;

            return instance;
        }

        private void ViewModelDisposing(object sender, EventArgs args)
        {
            if ((sender as INotifyDisposing) == null || !_ViewModelNavigators.ContainsKey((INotifyDisposing)sender))
            {
                return;
            }

            var dataNavigators = _ViewModelNavigators[(INotifyDisposing)sender];

            foreach (var dataNavigator in dataNavigators)
            {
                dataNavigator.Dispose();
            }

            _ViewModelNavigators.Remove((INotifyDisposing)sender); 
        }
    }
}
