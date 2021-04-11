using System;
using System.Runtime.Serialization;

namespace SDBServices.STS.DTO
{
    public class StsPosition : IStsPosition
    {
        [DataMember]
        public string ShortKey { get; set; }
        [DataMember]
        public Guid OUID { get; set; }

        public int _id { get; set; }

        [DataMember]
        public Guid? _objectID { get; set; }
        public string _type { get; set; }
        public string _ref { get; set; }

        [DataMember]
        public string _text { get; set; }
    }
}