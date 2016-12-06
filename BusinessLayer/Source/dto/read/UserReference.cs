using System;
using System.Runtime.Serialization;

namespace Organisation.BusinessLayer
{
    [Serializable]
    [DataContract]
    public class UserReference
    {
        [DataMember]
        public string Uuid { get; set; }
    }
}
