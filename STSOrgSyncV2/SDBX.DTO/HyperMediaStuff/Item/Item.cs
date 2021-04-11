using System;
using System.Runtime.Serialization;

namespace SDBServices.STS.DTO
{
    // ================================================================================
    [Serializable]
    [DataContract(Namespace = "http://gentofte.dk/sdbservices/sts/dto/2016/v1")]

    [KnownType(typeof(STSAddress))]
    [KnownType(typeof(STSOU))]
    [KnownType(typeof(STSUser))]
    [KnownType(typeof(JobFunctionRole))]
    [KnownType(typeof(DataConstraint))]
    [KnownType(typeof(StsPosition))]
    [KnownType(typeof(KLE))]

    public class Item : IItem
    {
        // -----------------------------------------------------------------------------
        [DataMember]
        public int _id { get; set; }

        // -----------------------------------------------------------------------------
        [DataMember]
        public Guid _objectID { get; set; }

        // -----------------------------------------------------------------------------
        //[DataMember]
        public string _type { get; set; }

        // -----------------------------------------------------------------------------
        [DataMember]
        public string _ref { get; set; }

        // -----------------------------------------------------------------------------
        [DataMember]
        public string _text { get; set; }
    }
}