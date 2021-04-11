using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SDBServices.STS.DTO
{
    [Serializable]
    [DataContract(Namespace = "http://gentofte.dk/sdbservices/sts/dto/2016/v1")]
    public class STSOU : Item, ISTSOU
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string ShortKey { get; set; }
        [DataMember]
        public IEnumerable<STSAddress> Addresses { get; set; }
        [DataMember]
        public List<Guid> ItUsage { get; set; }
        [DataMember]
        public Guid? _parentID { get; set; }
        [DataMember]
        public bool WithinScope { get; set; }
        [DataMember]
        public bool IsRoot { get; set; }
        [DataMember]
        public bool IsPayoutUnit { get; set; }
        [DataMember]
        public Guid? _payoutUnitID { get; set; }
    }
}
