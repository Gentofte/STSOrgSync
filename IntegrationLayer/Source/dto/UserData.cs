using System;
using System.Collections.Generic;

namespace Organisation.IntegrationLayer
{
    internal class UserData
    {
        public string ShortKey { get; set; }
        public string UserId { get; set; }
        public List<AddressRelation> Addresses { get; set; } = new List<AddressRelation>();
        public DateTime Timestamp { get; set; }

        private string uuid;
        public string Uuid
        {
            get
            {
                return uuid;
            }
            set
            {
                uuid = value?.ToLower();
            }
        }

        private string personUuid;
        public string PersonUuid
        {
            get
            {
                return personUuid;
            }
            set
            {
                personUuid = value?.ToLower();
            }
        }
    }
}
