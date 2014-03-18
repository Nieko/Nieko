using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Collections;
using Nieko.Infrastructure.Navigation;
using System.ComponentModel;
using Nieko.Infrastructure.EventAggregation;
using Nieko.Infrastructure.ComponentModel;
using System.Windows.Threading;
using Nieko.Infrastructure.Threading;
using Nieko.Infrastructure;
using Nieko.Infrastructure.Windows;
using System.Windows.Controls;
using Nieko.Infrastructure.Windows.Data;
using System.IO;
using System.Reflection;
using Nieko.Infrastructure.Logging;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml;
using Nieko.Infrastructure.Export;
using p = Microsoft.Practices.Prism.Regions;
using Nieko.Prism.Events;
using System.Windows;

namespace Nieko.Modules.Navigation
{
    public class ViewNavigator : IViewNavigator
    {
        private object _LockObject = new object();
        private bool _WaitDialogVisible = false;
        private bool _RunningBackgroundTask = false;
        private p.IRegionManager _RegionManager;
        private Dictionary<string, HashSet<INavigationAware>> _RegionActiveViewModels;
        private Dictionary<object, IDisposable> _DisposableViewStates;
        private IDialogs _Dialogs;
        private IFormsManager _FormsManager;
        private IInfrastructureEventAggregator _EventAggregator;
        private Queue<Action> _UIWorkQueue = new Queue<Action>();
        private Queue<Action> _PostLayoutWorkQueue = new Queue<Action>();
        private Dictionary<object, string> _ModalActionsRunning = new Dictionary<object, string>();
        private IWaitDialog _CurrentWaitDialog;
        private bool _IsProcessingNotifications;

        public event EventHandler<NavigationEventArgs> Navigating = delegate { };

        public ISet<string> KeptAliveRegions { get; private set;  }

        public IDialogs Dialogs
        {
            get
            {
                return _Dialogs;
            }
        }

        public ViewNavigator(p.IRegionManager regionManager, IDialogs dialogs, IFormsManager formsManager, IInfrastructureEventAggregator eventAggregator)
        {
            _RegionManager = regionManager;
            _Dialogs = dialogs;
            _FormsManager = formsManager;
            _EventAggregator = eventAggregator;

            _RegionActiveViewModels = new Dictionary<string, HashSet<INavigationAware>>();
            _DisposableViewStates = new Dictionary<object, IDisposable>();
            KeptAliveRegions = new HashSet<string>();

            _EventAggregator.Subscribe<IApplicationExitRequestEvent>(ApplicationExitRequested);
            _EventAggregator.Subscribe<IStartupNotificationsRequestEvent>(args => _IsProcessingNotifications = true);
            _EventAggregator.Subscribe<IInitialActionRequestEvent>(args => _IsProcessingNotifications = false);
        }

        public bool ClearRegion(string regionName)
        {
            if (!IsRequestGranted(null, regionName))
            {
                return false;
            }

            PrepareRegionForChange(regionName, null, null);

            return true;
        }

        public bool NavigateTo(EndPoint endPoint)
        {
            Logger.Instance.Log("Navigation to " + GetLog(endPoint) + " requested");

            if (endPoint == null)
            {
                throw new ArgumentException("endPoint");
            }

            DoLocked(() =>
                {
                    if (_IsProcessingNotifications)
                    {
                        throw new InvalidOperationException("Invalid navigation to " + GetLog(endPoint) + " : navigation to EndPoints is not allowed while start-up notifications are being processed");
                    }
                });

            ViewModelForm form = null;

            if (!_FormsManager.FormsByEndPoint.TryGetValue(endPoint, out form))
            {
                throw new ArgumentException(GetLog(endPoint) + " does not have an associated form");
            }

#if!DEBUG
            try
            {
#endif
                if (!IsRequestGranted(endPoint, form.RegionName))
                {
                    Logger.Instance.Log("Request to Navigate to " + GetLog(endPoint) + " denied");  
                    return false;
                }

                EnqueueUIWork(() =>
                    {
                        CompleteNavigation(form);
                    });

                return true;
#if!DEBUG
            }
            catch (Exception e)
            {
                Dialogs.ShowModalMessage("Navigation to " + endPoint.Description + " failed");  
                Logger.Instance.LogException("Navigation failed :", e);
                return false;
            }
#endif
        }

        public void ExecuteModal(Action<object, DoWorkEventArgs> work, string message)
        {
            ExecuteModal(work, null, message);
        }

        public void ExecuteModal(Action<object, DoWorkEventArgs> work, Action<object, RunWorkerCompletedEventArgs> finish, string message)
        {
            try
            {
                _RunningBackgroundTask = true;
                BackgroundWorker worker = new BackgroundWorker();
                DoWorkEventHandler handler = (sender, args) => work(sender, args);

                worker.DoWork += handler;
                worker.RunWorkerCompleted += (sender, args) =>
                {
                    RemoveModalAction(handler);
                    if (finish != null)
                    {
                        finish(sender, args);
                    }

                    worker.Dispose();
                };

                AddModalAction(handler, message);

                worker.RunWorkerAsync();
            }
            catch (Exception e)
            {
                Logger.Instance.LogException("Modal execution failed", e);
                throw (e);
            }
        }

        public void EnqueueUIWork(Action work)
        {
            if (!_RunningBackgroundTask)
            {
                Dispatcher.CurrentDispatcher.BeginInvoke(work, null);
                return;
            }

            DoLocked(() =>
            {
                _UIWorkQueue.Enqueue(() => 
                    {
                        Dispatcher.CurrentDispatcher.BeginInvoke(work, null);
                    });
            });
        }

        public void EnqueuePostLayoutWork(Action work)
        {
            if (!_RunningBackgroundTask)
            {
                Dispatcher.CurrentDispatcher.BeginInvoke(work, DispatcherPriority.ContextIdle);
                return;
            }

            DoLocked(() =>
            {
                _PostLayoutWorkQueue.Enqueue(work);
            });
        }

        public void WaitModal(Func<bool> endCondition, string message)
        {
            BackgroundWorker worker = new BackgroundWorker();
            DoWorkEventHandler handler = null;

            worker.WorkerSupportsCancellation = true;
            Action endWorker = () =>
            {
                RemoveModalAction(handler);
                worker.CancelAsync();
            };

            var task = new PolledTask(endCondition, 1000, endWorker);
            handler = (sender, args) =>
            {
                task.Start();
            };
            worker.DoWork += handler;

            AddModalAction(handler, message);

            worker.RunWorkerAsync();
        }

        public void SaveMainRegionDataToXml()
        {
            var views = _RegionManager.Regions[CoreRegionNames.MainRegion].Views.ToList();
            string data = string.Empty;

            while (true)
            {
                if (!views.Any())
                {
                    break;
                }

                var view = views.First() as UserControl;

                if (view == null)
                {
                    break;
                }

                if (view.DataContext == null)
                {
                    break;
                }

                if (view.DataContext == null)
                {
                    break;
                }

                var viewModel = view.DataContext;

                if (viewModel == null)
                {
                    break;
                }

                var stream = new MemoryStream();
                var exportWriter = new StreamWriter(stream);
                var exporter = new XmlExporter();

                exporter.Export(exportWriter, viewModel);

                var exportReader = new StreamReader(stream);

                stream.Position = 0;
                data = exportReader.ReadToEnd();

                break;
            }

            if (data == string.Empty)
            {
                Dialogs.ShowModalMessage("Cannot save data to Xml");
                return;
            }

            var fileDialog = new Microsoft.Win32.SaveFileDialog();
            fileDialog.DefaultExt = "xml";

            if (!fileDialog.ShowDialog().Value)
            {
                return;
            }

            data = data.Replace(@"<?xml version=""1.0"" encoding=""utf-16""?>", "");

            var fileStream = new StreamWriter(fileDialog.FileName);
            fileStream.Write(data);

            fileStream.Close();
        }

        private void DoLocked(Action action)
        {
            lock (_LockObject)
            {
                action();
            }
        }

        private HashSet<INavigationAware> GetActiveViewModels(string regionName)
        {
            HashSet<INavigationAware> viewModels;

            if (!_RegionActiveViewModels.TryGetValue(regionName, out viewModels))
            {
                viewModels = new HashSet<INavigationAware>();
                _RegionActiveViewModels.Add(regionName, viewModels);
            }

            return viewModels;
        }

        private bool IsRequestGranted(EndPoint destination, string regionName)
        {
            Logger.Instance.Log("Checking for objections for navigation to " + GetLog(destination));

            var request = new NavigationRequest(destination, regionName, this);

            foreach (var subscriber in GetActiveViewModels(regionName))
            {
                subscriber.NavigationRequested(request);
                if (request.Cancel)
                {
                    return false;
                }
            }

            return true;
        }

        private void CompleteNavigation(ViewModelForm form)
        {
            Logger.Instance.Log("Navigation details collected for " + GetLog(form.EndPoint) + ": completing... ");
            Logger.Instance.Log("Creating " + GetLog(form.EndPoint) + " View");

            var instance = form.ViewFactory();
            object state = null;

            ExecuteModal((sender, args) => 
                {
                    Logger.Instance.Log("Creating " + GetLog(form.EndPoint) + " ViewModel");
                    state = form.ViewModelFactory();
                },
                (sender, args) =>
                {
                    PrepareRegionForChange(form.RegionName, form.EndPoint, instance);

                    Logger.Instance.Log("Setting " + form.RegionName + " Region content to " + GetLog(form.EndPoint));
                    EnqueueUIWork(() =>
                    {
                        form.ViewModelSet(instance, state);
                        _RegionManager.Regions[form.RegionName].Add(instance);
                        _RegionManager.Regions[form.RegionName].Activate(instance);
                    });

                    if (state is INavigationAware)
                    {
                        GetActiveViewModels(form.RegionName).Add(state as INavigationAware);
                    }

                    if (state != null && state is IDisposable)
                    {
                        _DisposableViewStates.Add(instance, (IDisposable)state);
                    }
                },
                    "Loading...");
        }

        private void PrepareRegionForChange(string regionName, EndPoint endPoint, object content)
        {
            var activeViewModels = GetActiveViewModels(regionName);
            FrameworkElement dataContextHolder = null;

            var args = new NavigationEventArgs(activeViewModels.ToList(), endPoint, content);
            Navigating(this, args);

            Logger.Instance.Log("Removing " + regionName + " Region content not for " + GetLog(endPoint));
            foreach (var view in _RegionManager.Regions[regionName].Views.ToList())
            {
                if (view != content)
                {
                    if (_DisposableViewStates.ContainsKey(view))
                    {
                        _DisposableViewStates[view].Dispose();
                        _DisposableViewStates.Remove(view);
                    }

                    dataContextHolder = view as FrameworkElement;
                    if (dataContextHolder != null)
                    {
                        dataContextHolder.DataContext = null;
                    }

                    _RegionManager.Regions[regionName].Remove(view);
                }
            }

            activeViewModels.Clear();
        }

        private void AddModalAction(object action, string message)
        {
            _ModalActionsRunning.Add(action, message);
            if (_CurrentWaitDialog == null)
            {
                _CurrentWaitDialog = _Dialogs.CreateWaitDialog();
                ShowWaitingDialog(_CurrentWaitDialog, message);
            }
                    
        }

        private void RemoveModalAction(object action)
        {
            bool changeMessage = _CurrentWaitDialog.Title == _ModalActionsRunning[action];

            _ModalActionsRunning.Remove(action);
            if (_ModalActionsRunning.Count == 0)
            {
                Action closeDialog = () => CloseWaitingDialog(_CurrentWaitDialog);
                Dispatcher.CurrentDispatcher.Invoke(closeDialog);
                _CurrentWaitDialog = null;
                _RunningBackgroundTask = false;
                ProcessUIWorkQueue();
            }
            else if (changeMessage)
            {
                Action changeAction = () => _CurrentWaitDialog.Title = _ModalActionsRunning.Values.FirstOrDefault();
                Dispatcher.CurrentDispatcher.Invoke(changeAction);
            }       
        }

        private void ShowWaitingDialog(IWaitDialog waitDialog, string message)
        {
            waitDialog.Title = message;
            _RegionManager.Regions[CoreRegionNames.PopupRegion].Add(waitDialog);
            _RegionManager.Regions[CoreRegionNames.PopupRegion].Activate(waitDialog);

            var eventArgs = _EventAggregator.CreateEvent<IPopupChangedEvent>();
            eventArgs.SetShow(true);
            _EventAggregator.Publish(eventArgs);
            _WaitDialogVisible = true;
        }

        private void CloseWaitingDialog(IWaitDialog waitDialog)
        {
            if (!_WaitDialogVisible)
            {
                return;
            }

            _RegionManager.Regions[CoreRegionNames.PopupRegion].Remove(waitDialog);

            var eventArgs = _EventAggregator.CreateEvent<IPopupChangedEvent>();
            _EventAggregator.Publish(eventArgs);
            _WaitDialogVisible = false;
        }

        private void ProcessUIWorkQueue()
        {
            DoLocked(() =>
            {
                while (_UIWorkQueue.Count > 0)
                {
                    _UIWorkQueue.Dequeue()();
                }

                while (_PostLayoutWorkQueue.Count > 0)
                {
                    Dispatcher.CurrentDispatcher.BeginInvoke(_PostLayoutWorkQueue.Dequeue(), DispatcherPriority.ContextIdle);
                }
            });
        }

        private void ApplicationExitRequested(IApplicationExitRequestEvent eventArgs)
        {
            var request = new ExitRequest(this);

            foreach (var subscriber in GetActiveViewModels(CoreRegionNames.MainRegion))
            {
                subscriber.CloseRequested(request); 
                if (request.Cancel)
                {
                    eventArgs.Cancel = true;
                    return;
                }
            }
        }

        public void Shutdown()
        {
            Logger.Instance.Log("Application shutdown requested");
            var result = _EventAggregator.Publish<IApplicationExitRequestEvent>();

            if (!result.Cancel)
            {
                Application.Current.Shutdown(); 
            }
        }

        private string GetLog(EndPoint endPoint)
        {
            if (endPoint == null || endPoint == EndPoint.Root)
            {
                return "desktop";
            }

            return endPoint.GetFullPath(); 
        }
    }
}
