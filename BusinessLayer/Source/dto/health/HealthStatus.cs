using System;

namespace Organisation.BusinessLayer
{
    [Serializable]
    public class HealthStatus
    {
        public bool ServiceStatus { get; set; } = true;
        public bool DBStatus { get; set; } = true;

        public bool Up()
        {
            return ServiceStatus && DBStatus;
        }
    }
}
