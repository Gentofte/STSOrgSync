using System;
using System.Collections.Generic;

namespace Organisation.BusinessLayer.DTO.V1_1
{
    [Serializable]
    public class ContactPlace
    {
        public string OrgUnitUuid { get; set; }
        public List<string> Tasks { get; set; } = new List<string>();
    }
}
