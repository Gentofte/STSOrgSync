using System;

namespace SDBServices.STS.DTO
{
    /// <summary>
    /// Represents an StsAddress, that is, an address with a text, object-id and a type.
    /// </summary>
    public interface ISTSAddress
    {
        /// <summary>
        /// The type of this address. The type is used to distinguish between different 
        /// types of addresses in the STS plugin.
        /// 
        /// Examples are: Phone, Email and Postal address.
        /// </summary>
        AddressType Type { get; }

        int _id { get; }
        Guid? _objectID { get; }
        string _ref { get; }
        string _text { get; }
    }
}
