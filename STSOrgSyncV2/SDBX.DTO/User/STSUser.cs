using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SDBServices.STS.DTO
{
    [Serializable]
    [DataContract(Namespace = "http://gentofte.dk/sdbservices/sts/dto/2016/v1")]
    public class STSUser : Item, ISTSUser
    {
        [DataMember]
        public string UserId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string CPR { get; set; }
        [DataMember]
        public IEnumerable<STSAddress> Addresses { get; set; }

        [DataMember]
        public string UserShortKey { get;  set; }
        
        [DataMember]
        public string PersonShortKey { get;  set; }

        [DataMember]
        public Guid? PersonObjectId { get; set; }

        [DataMember]
        public StsPosition Position { get; set; }

        [DataMember]
        public bool WithinScope { get; set; }
    }
}
