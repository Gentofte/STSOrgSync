using System;
using System.Collections.Generic;

namespace Organisation.BusinessLayer.DTO.V1_1
{
    [Serializable]
    public class OrgUnitRegistration
    {
        public string Uuid { get; set; }
        public string ShortKey { get; set; }
        public string Name { get; set; }
        public string ParentOrgUnitUuid { get; set; }
        public string PayoutUnitUuid { get; set; }
        public List<ContactPlace> ContactPlaces { get; set; } = new List<ContactPlace>();
        public DateTime Timestamp { get; set; } = DateTime.Now.AddMinutes(-5);
        public Address Phone { get; set; } = new Address();
        public Address Email { get; set; } = new Address();
        public Address Location { get; set; } = new Address();
        public Address LOSShortName { get; set; } = new Address();
        public Address ContactOpenHours { get; set; } = new Address();
        public Address EmailRemarks { get; set; } = new Address();
        public Address Contact { get; set; } = new Address();
        public Address PostReturn { get; set; } = new Address();
        public Address PhoneOpenHours { get; set; } = new Address();
        public Address Ean { get; set; } = new Address();
        public Address Post { get; set; } = new Address();
        public List<string> ItSystemUuids { get; set; } = new List<string>();
    }
}
