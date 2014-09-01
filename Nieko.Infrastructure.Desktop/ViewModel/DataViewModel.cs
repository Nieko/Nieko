using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Navigation;
using Nieko.Infrastructure.Navigation.RecordNavigation;
using Nieko.Infrastructure.Windows.Data;

namespace Nieko.Infrastructure.ViewModel
{
    /// <summary>
    /// Base class for a ViewModel with that confirms closing of its screen
    /// if their are any unsaved changes
    /// </summary>
    public abstract class DataViewModel : ViewModelBase, INavigationAware
    {
        protected virtual IDataNavigator PrimaryDataNavigator 
        { 
            get
            {
                return Root != null
                    && Root.Owner != null
                    && Root.Owner.Hierarchy.FirstOrDefault() != null
                    && Root.Owner.Hierarchy.FirstOrDefault().PersistedView != null
                    && Root.Owner.Hierarchy.FirstOrDefault().PersistedView.Owner != null ?
                    Root.Owner.Hierarchy.FirstOrDefault().PersistedView.Owner.DataNavigator :
                    null;
            }
        }

        protected IPersistedViewRoot Root { get; set; }

        public override void Load()
        {
            if (Root == null)
            {
                Root = BuildRoot();
            }

            Root.Load();
        }

        protected abstract IPersistedViewRoot BuildRoot();

        public void NavigationRequested(NavigationRequest request)
        {
            CheckSave(request);
        }

        public void CloseRequested(ExitRequest request)
        {
            CheckSave(request);
        }

        private void CheckSave(INavigateRequest request)
        {
            if ((EditState.New | EditState.Edit).HasFlag(PrimaryDataNavigator.EditState))
            {
                var result = request.RegionNavigator.Dialogs.ShowModalMessage("Save Changes?", "You have unsaved changes. Save now?", ModalMessageButton.Yes | ModalMessageButton.No | ModalMessageButton.Cancel);

                if (result == ModalMessageButton.Cancel)
                {
                    request.Cancel = true;
                    return;
                }

                if (result == ModalMessageButton.Yes)
                {
                    PrimaryDataNavigator.EnterState(EditState.Save); 
                }

                return;
            }
        }
    }
}
