using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

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
    }
}

