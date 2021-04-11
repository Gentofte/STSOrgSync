using System;

namespace SDBServices.STS.DTO
{
    /// <summary>
    /// A collection of JobFunctionRoles.
    /// </summary>
    public interface IJobFunctionRoles : ICollection
    {
        /// <summary>
        /// The UserID of the user who has these jobfunctionroles.
        /// </summary>
        string userId { get; }

        /// <summary>
        /// The name of the user.
        /// </summary>
        string commonName { get; }

        /// <summary>
        /// The objectID of the user account.
        /// </summary>
        Guid serial { get; }
    }
}