using Organisation.IntegrationLayer;

namespace Organisation.BusinessLayer
{
    public class HealthService
    {
        private Health health = new Health();

        public HealthStatus Status() {
            HealthStatus status = new HealthStatus();
            status.STSstatus = IsStsReachable();
            status.ServiceStatus = AreServicesReachable();

            return status;
        }

        private bool IsStsReachable() {
            return health.IsStsReachable();
        }

        private bool AreServicesReachable() {
            return health.AreServicesReachable();
        }
    }
}
