using Organisation.IntegrationLayer;

namespace Organisation.BusinessLayer
{
    public class HealthService
    {
        private Health health = new Health();

        public HealthStatus Status()
        {
            HealthStatus status = new HealthStatus();
            status.ServiceStatus = AreServicesReachable();

            return status;
        }

        private bool AreServicesReachable()
        {
            return health.AreServicesReachable();
        }
    }
}
