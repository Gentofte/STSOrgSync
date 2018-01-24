using log4net.Config;
using Organisation.IntegrationLayer;
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
                XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("Log.config"));

                initialized = true;
            }
        }
    }
}
