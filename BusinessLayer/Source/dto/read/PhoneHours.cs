using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Organisation.BusinessLayer
{
    [Serializable]
    [DataContract]
    [XmlRoot(ElementName = "AddressHolder")]
    public class PhoneHours : AddressHolder
    {
        [DataMember]
        public string Type { get { return "PhoneHours"; } }
    }
}
