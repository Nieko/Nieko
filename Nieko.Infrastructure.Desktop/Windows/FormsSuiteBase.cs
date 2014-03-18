using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Windows;
using Nieko.Infrastructure.Navigation;
using Nieko.Infrastructure.ComponentModel;
using Nieko.Infrastructure.EventAggregation;
using System.Xml.Serialization;
using System.Windows.Input;
using System.IO;
using Nieko.Infrastructure.Export;

namespace Nieko.Infrastructure.Windows
{
    /// <summary>
    /// Base class for implementing a Forms Suite (see also <seealso cref="IFormsSuite"/>
    /// </summary>
    /// <typeparam name="T">Self reference to implementing type</typeparam>
    public abstract class FormsSuiteBase<T> : IFormsSuite 
        where T : FormsSuiteBase<T>
    {
        private IViewNavigator _RegionNavigator;
        private IList<ISuiteForm> _Forms;
        private int _SelectedTabIndex = -1;
        private bool _IsDisposing;
        private Action<IFormSuiteRequestEvent> _FormRequestHandler;
        
        protected IInfrastructureEventAggregator EventAggregator { get; private set; }

        protected IViewNavigator RegionNavigator
        {
            get
            {
                return _RegionNavigator;
            }
            private set
            {
                _RegionNavigator = value;
            }
        }

        protected virtual IDataExporter Exporter
        {
            get
            {
                return DataExporter.None;
            }
        }

        public event EventHandler Disposing = delegate { };
        private IFormsManager _FormsManager;

        public virtual int SelectedTabIndex
        {
            get
            {
                return _SelectedTabIndex;
            }
            set
            {
                if (_SelectedTabIndex == value)
                {
                    return;
                }

                _SelectedTabIndex = value;

                if (value >= 0 && value < Forms.Count)
                {
                    Forms[SelectedTabIndex].ShowAction();
                }
            }
        }

        public virtual bool ShowExportData
        {
            get
            {
                return Exporter != null && Exporter.IsEnabled;
            }
        }

        public ICommand ExportData { get; private set; }

        [XmlIgnore]
        public IList<ISuiteForm> Forms
        {
            get
            {
                if (_Forms == null)
                {
                    BuildForms();
                }
                return _Forms;
            }
        }

        public abstract string FormRegion { get; }
        public abstract IEndPointProvider SubForms { get; }

        public FormsSuiteBase(IViewNavigator regionNavigator, IInfrastructureEventAggregator eventAggregator, IFormsManager formsManager)
        {
            RegionNavigator = regionNavigator;
            _FormRequestHandler = FormSuiteRequested;
            EventAggregator = eventAggregator;
            EventAggregator.Subscribe<IFormSuiteRequestEvent>(_FormRequestHandler, false);
            _FormsManager = formsManager;
            ExportData = new RelayCommand(DoExportData);
        }

        internal void Add(EndPoint endPoint)
        {
            var report = new SuiteForm()
            {
                Caption = endPoint.Description
            };
            
            if(Forms.Count == 0)
            {
                report.Ordinal = 0;
            }
            else
            {
                report.Ordinal = Forms.Max(sr => sr.Ordinal) + 1; 
            }

            report.ShowAction = () => RegionNavigator.NavigateTo(endPoint);

            Forms.Add(report);
        }

        private void BuildForms()
        {
            _Forms = new List<ISuiteForm>();

            foreach (var subForm in SubForms.GetEndPoints())
            {
                Add(subForm); 
            }

            var byOrdinal = _Forms
                .OrderBy(f => f.Ordinal)
                .ToList();

            _Forms = byOrdinal;
        }

        private void FormSuiteRequested(IFormSuiteRequestEvent request)
        {
            if (request.TypeRequested == GetType())
            {
                request.Result = this;
            }
        }

        private void DoExportData()
        {
            Exporter.Export(GetExportData());
        }

        protected virtual object GetExportData()
        {
            var data = new CompositeExport();
            var currentTabIndex = SelectedTabIndex;

            SelectedTabIndex = 0;

            foreach (var subForm in SubForms.GetEndPoints()
                .OrderBy(ep => ep.Ordinal))
            {
                var viewModel = _FormsManager.FormsByEndPoint[subForm].ViewModelFactory() as IExportableSubForm;
                var exportData = viewModel == null ?
                    null :
                    viewModel.GetExportData();

                data.AddItem(subForm.Description, exportData);
                SelectedTabIndex++;
            }

            SelectedTabIndex = currentTabIndex;

            return data;
        }

        public virtual void Dispose()
        {
            if (_IsDisposing)
            {
                return;
            }

            _IsDisposing = true;
            EventAggregator.Unsubscribe<IFormSuiteRequestEvent>(_FormRequestHandler);
            _FormRequestHandler = null;
            RegionNavigator.ClearRegion(FormRegion);  
            Disposing(this, EventArgs.Empty);
        }
    }
}
