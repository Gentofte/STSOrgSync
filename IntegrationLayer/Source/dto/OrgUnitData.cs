using System;
using System.Collections.Generic;

namespace Organisation.IntegrationLayer
{
    internal class OrgUnitData
    {
        public string Uuid { get; set; }
        public string ShortKey { get; set; }
        public string Name { get; set; }
        public string ParentOrgUnitUuid { get; set; }
        public List<string> OrgFunctionUuids { get; set; } = new List<string>();
        public List<AddressRelation> Addresses { get; set; } = new List<AddressRelation>();
        public DateTime Timestamp { get; set; }
    }
}
