using System;
using System.Runtime.Serialization;

namespace Organisation.BusinessLayer
{
    [Serializable]
    [DataContract]
    public class Position
    {
        [DataMember]
        public string Uuid { get; set; }
        [DataMember]
        public string ShortKey { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public OUReference OU { get; set; }
        public UserReference User { get; set; }
    }
}
