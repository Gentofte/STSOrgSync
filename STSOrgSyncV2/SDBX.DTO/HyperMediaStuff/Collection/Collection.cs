using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SDBServices.STS.DTO
{
    // ================================================================================
    [Serializable]
    [DataContract(Namespace = "http://gentofte.dk/sdbservices/sts/dto/2016/v1")]

    [KnownType(typeof(JobFunctionRoles))]
    [KnownType(typeof(KLECollection))]

    public class Collection : ICollection
    {
        // -----------------------------------------------------------------------------
        [DataMember]
        public string _ref { get; set; }

        // -----------------------------------------------------------------------------
        [DataMember]
        public int _count { get; set; }

        // -----------------------------------------------------------------------------
        [DataMember]
        public IEnumerable<IItem> _items { get; set; }

        // -----------------------------------------------------------------------------
        //[DataMember]
        public string _text { get; set; }
    }
}