using System;
using System.Collections.Generic;

namespace SDBServices.STS.DTO
{
    /// <summary>
    /// An item containing the information that should be available about an OU for the STS plugin.
    /// </summary>
    public interface ISTSOU : IItem
    {
        /// <summary>
        /// The name of the Organisational Unit.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The shortkey of the Organisational Unit.
        /// </summary>
        string ShortKey { get; }

        /// <summary>
        /// A collection of Addresses.
        /// 
        /// See ISTSAddress for examples of addresses.
        /// </summary>
        IEnumerable<STSAddress> Addresses { get; }

        /// <summary>
        /// A list of UUIDs representing IT Systems that are used by this Organisational Unit.
        /// </summary>
        List<Guid> ItUsage { get; }

        /// <summary>
        /// The ID of the parent organisational unit.
        /// 
        /// If there is no parent, this property is null.
        /// </summary>
        Guid? _parentID { get; }

        /// <summary>
        /// States whether or not a given organisational unit lives inside the actual organisation
        /// (In Gentofte this is true if the OU has GK in list of parents)
        /// </summary>
        bool WithinScope { get; }

        /// <summary>
        /// States whether or not the particular OU is a root.
        /// </summary>
        bool IsRoot { get; }

        /// <summary>
        /// States whether or not this OU should be considered a Payout Unit.
        /// </summary>
        bool IsPayoutUnit { get; set; }
        
        /// <summary>
        /// A reference to the actual OU that is the payout unit.
        /// 
        /// If we use virtual payout units, this should refer to the object ID of the virtual OU.
        /// Otherwise it should point to this OU itself.
        /// 
        /// If IsPayoutUnit is false, then this field should be null.
        /// </summary>
        Guid? _payoutUnitID { get; set; }
    }
}
