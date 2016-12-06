using System;
using Microsoft.Owin.Hosting;
using System.Runtime.CompilerServices;
using Organisation.IntegrationLayer;

namespace Organisation.ServiceLayer
{
    public static class ServiceInitializer
    {
        private static bool initialized = false;

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IDisposable Init()
        {
            if (!initialized)
            {
                initialized = true;

                if (OrganisationRegistryProperties.GetInstance().UseSSL)
                {
                    // If this fails with an error code 5 (Access Denied), then run powershell as admin, start netsh and run this command
                    // http add urlacl url=https://+:9010/ user=Everyone
                    return WebApp.Start<Startup>(url: "https://+:9010/");
                }

                // If this fails with an error code 5 (Access Denied), then run powershell as admin, start netsh and run this command
                // http add urlacl url=http://+:9010/ user=Everyone
                return WebApp.Start<Startup>(url: "http://+:9010/");
            }

            return null;
        }
    }
}
