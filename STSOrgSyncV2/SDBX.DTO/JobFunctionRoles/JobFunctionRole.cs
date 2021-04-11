using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SDBServices.STS.DTO
{
    [Serializable]
    [DataContract(Namespace = "http://gentofte.dk/sdbservices/sts/dto/2016/v1")]
    public class JobFunctionRole : IJobFunctionRole
    {
        [DataMember]
        public string cvr { get; set; }

        [DataMember]
        public IEnumerable<IDataConstraint> dataConstraints { get; set; }

        [DataMember]
        public string _text { get; set; }

        /* Unused fields */
        public int _id { get; set; }
        public Guid _objectID { get; set; }
        public string _type { get; set; }
        public string _ref { get; set; }
    }
}