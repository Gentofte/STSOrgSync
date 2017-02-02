using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Organisation.BusinessLayer
{
    [Serializable]
    public class ContactPlace
    {
        public OUReference OrgUnit { get; set; }

        public List<string> Tasks { get; set; } = new List<string>();
    }
}
