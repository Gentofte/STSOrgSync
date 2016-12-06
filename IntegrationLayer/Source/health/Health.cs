using System;

namespace Organisation.IntegrationLayer
{
    internal class Health
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private BrugerStub stub = new BrugerStub();

        public bool IsStsReachable()
        {
            bool isReachable = true;

            try
            {
                // perform a Token request to the STS
                TokenCache.IssueToken("bruger");
            }
            catch (Exception ex)
            {
                isReachable = false;
                log.Error("Failed to connect to sts", ex);
            }

            return isReachable;
        }

        public bool AreServicesReachable()
        {
            try
            {
                stub.GetLatestRegistration(Guid.NewGuid().ToString().ToLower(), true);
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
