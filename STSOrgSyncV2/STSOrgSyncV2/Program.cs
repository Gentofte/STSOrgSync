using GK.AppCore.Boot;

namespace STSOrgSyncV2
{
    // ================================================================================
    class Program
    {
        // -----------------------------------------------------------------------------
        public static void Main(string[] args)
        {
            StubHttpSys.Main<Startup>(args);
        }
    }
}
