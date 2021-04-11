using System;

namespace SDBServices.STS.DTO
{
    /// <summary>
    /// A position.
    /// 
    /// The _text-property contains the textual representation of the position.
    /// 
    /// Since positions are only retrieved with relation to a user-object, it is not 
    /// necessary to know the User ID.
    /// </summary>
    public interface IStsPosition
    {
        /// <summary>
        /// A shortkey for the position.
        /// </summary>
        string ShortKey { get; }

        /// <summary>
        /// The ID of the organisational unit in which the user has this position.
        /// </summary>
        Guid OUID { get; }

        string _text { get; }
        Guid? _objectID { get; }
    }
}
