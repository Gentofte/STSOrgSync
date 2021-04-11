using System;
using System.Collections.Generic;
namespace SDBServices.STS.DTO
{
    /// <summary>
    /// Contains the information about a user that is needed by the STS plugin.
    /// </summary>
    public interface ISTSUser : IItem
    {
        /// <summary>
        /// The UserID (login name) of the user.
        /// </summary>
        string UserId { get; }

        /// <summary>
        /// The name of the user.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The users CPR number, if available. Otherwise the string containing 10 zeroes.
        /// </summary>
        string CPR { get; }

        /// <summary>
        /// A collection of addresses that is known for the user.
        /// </summary>
        IEnumerable<STSAddress> Addresses { get; }

        /// <summary>
        /// A shortkey for the user.
        /// </summary>
        string UserShortKey { get; }

        /// <summary>
        /// A shortkey for the person that is connected to this user account.
        /// </summary>
        string PersonShortKey { get; }

        /// <summary>
        /// An object id for the person object connected to the user.
        /// </summary>
        Guid? PersonObjectId { get; }

        /// <summary>
        /// A representation of the position of this user.
        /// </summary>
        StsPosition Position { get; }

        /// <summary>
        /// States whether or not the user lives within the scope of the organisation.
        /// </summary>
        bool WithinScope { get; }
    }
}
