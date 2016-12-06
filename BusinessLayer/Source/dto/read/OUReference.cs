using System;
using System.Runtime.Serialization;

namespace Organisation.BusinessLayer
{
    [Serializable]
    [DataContract]
    public class OUReference
    {
        [DataMember]
        public string Uuid { get; set; }
        [DataMember]
        public string Name { get; set; }
    }
}
