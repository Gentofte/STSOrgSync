using System;
using System.Reflection;
using System.ServiceProcess;
using System.Configuration.Install;

namespace WindowsService
{
    static class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting installer...");

            if (Environment.UserInteractive)
            {
                Console.WriteLine("Entering interactive mode...");

                string parameter = string.Concat(args);
                if (args.Length != 1)
                {
                    Console.WriteLine("Syntax: WindowsService.exe [install/uninstall]");
                    return;
                }
                if (parameter.Contains("uninstall"))
                {
                    Console.WriteLine("Performing uninstall");
                    ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
                }
                else if (parameter.Contains("install"))
                {
                    Console.WriteLine("Performing install");                 
                    try
                    {
                        ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed to install service: " + ex.Message);
                        Console.WriteLine(ex.StackTrace);
                    }
                }
            }
            else
            {
                Console.WriteLine("Entering non-interactive mode...");
                ServiceBase.Run(new Service1());
            }
        }
    }
}
