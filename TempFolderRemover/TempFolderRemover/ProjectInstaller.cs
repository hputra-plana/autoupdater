using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.IO;

namespace TempFolderRemover
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        private ServiceProcessInstaller serviceProcessInstaller;
        private ServiceInstaller serviceInstaller;

        public ProjectInstaller()
        {
            serviceProcessInstaller = new ServiceProcessInstaller();
            serviceInstaller = new ServiceInstaller();

            // Service will run under the Local System account
            serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
            serviceProcessInstaller.Username = null;
            serviceProcessInstaller.Password = null;

            // Service configuration
            serviceInstaller.ServiceName = "TempFolderRemover";
            serviceInstaller.DisplayName = "Temp Folder Remover Service";
            serviceInstaller.Description = "Automatically removes files from a specified temporary folder at regular intervals.";
            serviceInstaller.StartType = ServiceStartMode.Automatic;

            // Add installers to the collection
            Installers.Add(serviceProcessInstaller);
            Installers.Add(serviceInstaller);
        }

        protected override void OnBeforeInstall(IDictionary savedState)
        {
            base.OnBeforeInstall(savedState);

            // Allow overriding install target via InstallUtil parameter: /installPath="C:\\autoupdaterpoc"
            // Default to C:\autoupdaterpoc if not provided
            string targetPath = Context?.Parameters?["installPath"];
            if (string.IsNullOrWhiteSpace(targetPath))
            {
                targetPath = @"C:\\autoupdaterpoc";
            }

            // Force the service's binary path (binPath) to the desired install location
            // Quote the path to support spaces
            string exePath = Path.Combine(targetPath, "TempFolderRemover.exe");
            Context.Parameters["assemblypath"] = "\"" + exePath + "\"";
        }
    }
}

