using System;
using System.Collections.Generic;

namespace Organisation.IntegrationLayer
{
    internal class UserData
    {
        public string Uuid { get; set; }
        public string ShortKey { get; set; }
        public string UserId { get; set; }

        public List<AddressRelation> Addresses { get; set; } = new List<AddressRelation>();
        public string PersonUuid { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
