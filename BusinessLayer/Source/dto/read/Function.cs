using System;
using System.Runtime.Serialization;

namespace Organisation.BusinessLayer
{
    [Serializable]
    [DataContract]
    public class Function
    {
        [DataMember]
        public string Uuid { get; set; }
        [DataMember]
        public string ShortKey { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string FunctionType { get; set; }
    }
}
