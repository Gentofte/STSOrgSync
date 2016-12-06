using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Organisation.BusinessLayer
{
    [Serializable]
    [KnownType(typeof(Organisation.BusinessLayer.Email))]
    [KnownType(typeof(Organisation.BusinessLayer.Location))]
    [KnownType(typeof(Organisation.BusinessLayer.Address))]
    [KnownType(typeof(Organisation.BusinessLayer.Phone))]
    [KnownType(typeof(Organisation.BusinessLayer.LOSShortName))]
    [DataContract]
    public class User
    {
        [DataMember]
        public string Uuid { get; set; }
        [DataMember]
        public string ShortKey { get; set; }
        [DataMember]
        public string UserId { get; set; }
        [DataMember]
        public Person Person { get; set; }
        [DataMember]
        public Position Position { get; set; }
        [DataMember]
        public List<AddressHolder> Addresses { get; set; }
    }
}
