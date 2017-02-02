using System;
using System.Collections.Generic;

namespace Organisation.BusinessLayer.DTO.V1_0
{
    [Obsolete]
    [Serializable]
    public class OrgUnitRegistration
    {
        public string Uuid { get; set; }
        public string ShortKey { get; set; }
        public string Name { get; set; }
        public string ParentOrgUnitUuid { get; set; }
        public string PayoutUnitUuid { get; set; }
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

        public V1_1.OrgUnitRegistration ConvertToV1_1()
        {
            V1_1.OrgUnitRegistration orgUnit = new V1_1.OrgUnitRegistration()
            {
                Contact = this.Contact.ConvertToV1_1(),
                ContactOpenHours = this.ContactOpenHours.ConvertToV1_1(),
                Ean = this.Ean.ConvertToV1_1(),
                Email = this.Email.ConvertToV1_1(),
                EmailRemarks = this.EmailRemarks.ConvertToV1_1(),
                ItSystemUuids = this.ItSystemUuids,
                Location = this.Location.ConvertToV1_1(),
                LOSShortName = this.LOSShortName.ConvertToV1_1(),
                Name = this.Name,
                ParentOrgUnitUuid = this.ParentOrgUnitUuid,
                PayoutUnitUuid = this.PayoutUnitUuid,
                Phone = this.Phone.ConvertToV1_1(),
                PhoneOpenHours = this.PhoneOpenHours.ConvertToV1_1(),
                Post = this.Post.ConvertToV1_1(),
                PostReturn = this.PostReturn.ConvertToV1_1(),
                ShortKey = this.ShortKey,
                Timestamp = this.Timestamp,
                Uuid = this.Uuid
            };

            return orgUnit;
        }
    }
}
