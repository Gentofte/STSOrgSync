using System;
using System.IO;
using System.ServiceProcess;

namespace WindowsService
{
    public partial class Service1 : ServiceBase
    {
        private static log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IDisposable webServer = null;

        public Service1()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory); // needed to override default windows services directory
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Log.Info("Service started");

            // Initialize BusinessLayer
            Organisation.BusinessLayer.Initializer.Init();

            // Initialize SchedulingLayer
            Organisation.SchedulingLayer.SyncJobRunner.Init();

            // Initialize ServiceLayer
            webServer = Organisation.ServiceLayer.ServiceInitializer.Init();
        }

        protected override void OnStop()
        {
            Log.Info("Service stopped");

            if (webServer != null) {
                webServer.Dispose();
            }

            base.OnStop();
        }
    }
}
