using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Modularity;

namespace Nieko.Prism.Modularity
{
    public class MemoryModuleTypeLoader : IModuleTypeLoader
    {
        private const string RefMemoryPrefix = "memory://";
        private HashSet<string> loadedModules = new HashSet<string>();

        /// <summary>
        /// Raised repeatedly to provide progress as modules are loaded in the background.
        /// </summary>
        public event EventHandler<ModuleDownloadProgressChangedEventArgs> ModuleDownloadProgressChanged;

        /// <summary>
        /// Raised when a module is loaded or fails to load.
        /// </summary>
        public event EventHandler<LoadModuleCompletedEventArgs> LoadModuleCompleted;

        public bool CanLoadModuleType(ModuleInfo moduleInfo)
        {
            if(moduleInfo == null)
            {
                throw new System.ArgumentNullException("moduleInfo");
            }

            return moduleInfo.Ref != null && moduleInfo.Ref.StartsWith(RefMemoryPrefix, StringComparison.Ordinal);
        }

        public void LoadModuleType(ModuleInfo moduleInfo)
        {
            if (moduleInfo == null)
            {
                throw new System.ArgumentNullException("moduleInfo");
            }

            try
            {
                // If this module has already been downloaded, I fire the completed event.
                if (this.IsSuccessfullyDownloaded(moduleInfo.ModuleType))
                {
                    this.RaiseLoadModuleCompleted(moduleInfo, null);
                }
                else
                {
                    // Although this isn't asynchronous, nor expected to take very long, I raise progress changed for consistency.
                    this.RaiseModuleDownloadProgressChanged(moduleInfo, 0, 100);

                    //

                    // Although this isn't asynchronous, nor expected to take very long, I raise progress changed for consistency.
                    this.RaiseModuleDownloadProgressChanged(moduleInfo, 100, 100);

                    // I remember the downloaded URI.
                    this.RecordDownloadSuccess(moduleInfo.Ref);

                    this.RaiseLoadModuleCompleted(moduleInfo, null);
                }
            }
            catch (Exception ex)
            {
                this.RaiseLoadModuleCompleted(moduleInfo, ex);
            }
        }

        private bool IsSuccessfullyDownloaded(string moduleType)
        {
            lock (this.loadedModules)
            {
                return this.loadedModules.Contains(moduleType);
            }
        }

        private void RecordDownloadSuccess(string moduleType)
        {
            lock (this.loadedModules)
            {
                this.loadedModules.Add(moduleType);
            }
        }

        private void RaiseModuleDownloadProgressChanged(ModuleInfo moduleInfo, long bytesReceived, long totalBytesToReceive)
        {
            this.RaiseModuleDownloadProgressChanged(new ModuleDownloadProgressChangedEventArgs(moduleInfo, bytesReceived, totalBytesToReceive));
        }

        private void RaiseModuleDownloadProgressChanged(ModuleDownloadProgressChangedEventArgs e)
        {
            if (this.ModuleDownloadProgressChanged != null)
            {
                this.ModuleDownloadProgressChanged(this, e);
            }
        }

        private void RaiseLoadModuleCompleted(ModuleInfo moduleInfo, Exception error)
        {
            this.RaiseLoadModuleCompleted(new LoadModuleCompletedEventArgs(moduleInfo, error));
        }

        private void RaiseLoadModuleCompleted(LoadModuleCompletedEventArgs e)
        {
            if (this.LoadModuleCompleted != null)
            {
                this.LoadModuleCompleted(this, e);
            }
        }
    }
}
