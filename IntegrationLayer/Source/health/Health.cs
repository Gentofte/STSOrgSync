using System;

namespace Organisation.IntegrationLayer
{
    internal class Health
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private BrugerStub stub = new BrugerStub();

        public bool AreServicesReachable()
        {
            try
            {
                stub.GetLatestRegistration(Guid.NewGuid().ToString().ToLower());
            }
            catch (Exception ex)
            {
                log.Error("Failed to connect to organisation", ex);
                return false;
            }

            return true;
        }
    }
}
