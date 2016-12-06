using System;
using System.Collections.Generic;

namespace Organisation.BusinessLayer
{
    [Serializable]
    public class OU
    {
        public string Uuid { get; set; }
        public string ShortKey { get; set; }
        public string Name { get; set; }
        public OUReference ParentOU { get; set; }
        public OUReference PayoutOU { get; set; }
        public List<AddressHolder> Addresses { get; set; }
        public List<string> ItSystems { get; set; }
        public List<Position> Positions { get; set; }

        public override string ToString() { return this.Name + " (" + this.Uuid + ")"; }

    }
}
