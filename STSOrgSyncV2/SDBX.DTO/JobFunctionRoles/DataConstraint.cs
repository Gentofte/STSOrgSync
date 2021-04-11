using System;
using System.Runtime.Serialization;

namespace SDBServices.STS.DTO
{
    [Serializable]
    [DataContract(Namespace = "http://gentofte.dk/sdbservices/sts/dto/2016/v1")]
    public class DataConstraint : IDataConstraint
    {
        [DataMember]
        public string id { get; set; }

        [DataMember]
        public string value { get; set; }
    }
}