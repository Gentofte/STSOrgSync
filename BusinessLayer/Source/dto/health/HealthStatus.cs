using System;

namespace Organisation.BusinessLayer
{
    [Serializable]
    public class HealthStatus
    {
        public bool STSstatus { get; set; } = true;
        public bool ServiceStatus { get; set; } = true;
        public bool DBStatus { get; set; } = true;

        public bool Up()
        {
            return ServiceStatus && STSstatus && DBStatus;
        }
    }
}
