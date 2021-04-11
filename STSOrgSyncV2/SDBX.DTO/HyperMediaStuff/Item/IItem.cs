using System;

namespace SDBServices.STS.DTO
{
    // ================================================================================
    public interface IItem
    {
        // -----------------------------------------------------------------------------
        /// <summary>
        /// Item ID. Alternative key. Provided for backward compatibility.
        /// New services MUST use 'ObjectID' attribute.
        /// </summary>
        int _id { get; }

        // -----------------------------------------------------------------------------
        /// <summary>
        /// Item ID (UUID). Primary key. This key may be persisted client side.
        /// </summary>
        Guid _objectID { get; }

        // -----------------------------------------------------------------------------
        /// <summary>
        /// Type of this item. Format to be defined later.
        /// </summary>
        string _type { get; }

        // -----------------------------------------------------------------------------
        /// <summary>
        /// Absolute ressource URL to THIS item - possibly empty string or NULL.
        /// </summary>
        string _ref { get; }

        // -----------------------------------------------------------------------------
        /// <summary>
        /// Item text - possibly empty string or NULL.
        /// </summary>
        string _text { get; }
    }
}