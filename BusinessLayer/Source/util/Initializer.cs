using log4net.Config;
using Organisation.IntegrationLayer;
using System.Net;
using System.Runtime.CompilerServices;

namespace Organisation.BusinessLayer
{
    public static class Initializer
    {
        private static bool initialized = false;

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Init()
        {
            if (!initialized)
            {
                // enable TLS 1.2 support for .NET 4.5
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("Log.config"));

                initialized = true;
            }
        }
    }
}
