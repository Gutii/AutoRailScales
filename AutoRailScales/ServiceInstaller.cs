using System.ComponentModel;
using System.ServiceProcess;
using System.Configuration.Install;

namespace ASHK.AutoRailScales
{
    [RunInstaller(true)]
    public class ServiceInstaller : Installer
    {        
        public ServiceInstaller()
        {
            ASHK.AutoRailScales.AssemblyInfo assemblyInfo = new AssemblyInfo();
            var serviceProcessInstaller = new ServiceProcessInstaller()
            {
                Account = ServiceAccount.LocalSystem,
                Username = null,
                Password = null
            };
            var serviceInstaller__1 = new System.ServiceProcess.ServiceInstaller()
            {
                
                DisplayName = assemblyInfo.Title,
                Description = assemblyInfo.Description,
                StartType = ServiceStartMode.Automatic,
                ServiceName = assemblyInfo.Product
            };
            Installers.Add(serviceProcessInstaller);
            Installers.Add(serviceInstaller__1);
        }
    }
}
