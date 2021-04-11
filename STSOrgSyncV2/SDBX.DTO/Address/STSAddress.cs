using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SDBServices.STS.DTO
{
    public class STSAddress : ISTSAddress
    {
        [DataMember]
        [JsonConverter(typeof(StringEnumConverter))]
        public AddressType Type { get; set; }

        [DataMember]
        public int _id { get; set; }
        
        [DataMember]
        public Guid? _objectID { get; set; }
        
        [DataMember]
        public string _ref { get; set; }

        [DataMember]
        public string _text { get; set; }
    }
}
