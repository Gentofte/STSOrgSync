
namespace STSOrgSyncV2.OS2sync
{
    public class OS2syncInitializer
    {
        public static void Init()
        {
            // Initialize BusinessLayer
            Organisation.BusinessLayer.Initializer.Init();

            // Initialize SchedulingLayer
            Organisation.SchedulingLayer.SyncJobRunner.InitAsync();
        }
    }
}
