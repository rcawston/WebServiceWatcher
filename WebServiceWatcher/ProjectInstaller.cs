using System.ComponentModel;
using System.ServiceProcess;

namespace WebServiceWatcher
{
    /// <summary>
    /// Installer class for self-installing windows service
    /// </summary>
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            var processInstaller = new ServiceProcessInstaller();
            var serviceInstaller = new ServiceInstaller();

            //set the privileges
            processInstaller.Account = ServiceAccount.LocalSystem;

            serviceInstaller.DisplayName = "WebServiceWatcher";
            serviceInstaller.Description = "Monitors a web server and restarts a windows service if needed.";
            serviceInstaller.StartType = ServiceStartMode.Automatic;

            // must be the same as what was set in the Program's constructor
            serviceInstaller.ServiceName = "WebServiceWatcher";
            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}
