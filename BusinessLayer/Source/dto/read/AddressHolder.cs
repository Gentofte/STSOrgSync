using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Organisation.BusinessLayer
{
    [XmlInclude(typeof(Location))]
    [XmlInclude(typeof(Phone))]
    [XmlInclude(typeof(Email))]
    [XmlInclude(typeof(LOSShortName))]
    [XmlInclude(typeof(PostReturn))]
    [XmlInclude(typeof(Contact))]
    [XmlInclude(typeof(EmailRemarks))]
    [XmlInclude(typeof(PhoneHours))]
    [XmlInclude(typeof(ContactHours))]
    [XmlInclude(typeof(Ean))]
    [XmlInclude(typeof(Post))]
    [XmlRoot(ElementName = "AddressHolder")]
    [Serializable]
    [DataContract]
    [KnownType(typeof(Organisation.BusinessLayer.Location))]
    [KnownType(typeof(Organisation.BusinessLayer.Phone))]
    [KnownType(typeof(Organisation.BusinessLayer.Email))]
    [KnownType(typeof(Organisation.BusinessLayer.LOSShortName))]
    [KnownType(typeof(Organisation.BusinessLayer.PostReturn))]
    [KnownType(typeof(Organisation.BusinessLayer.Contact))]
    [KnownType(typeof(Organisation.BusinessLayer.EmailRemarks))]
    [KnownType(typeof(Organisation.BusinessLayer.PhoneHours))]
    [KnownType(typeof(Organisation.BusinessLayer.ContactHours))]
    [KnownType(typeof(Organisation.BusinessLayer.Ean))]
    [KnownType(typeof(Organisation.BusinessLayer.Post))]
    public abstract class AddressHolder
    {
        [DataMember]
        public string Uuid { get; set; }
        [DataMember]
        public string Value { get; set; }
    }
}
