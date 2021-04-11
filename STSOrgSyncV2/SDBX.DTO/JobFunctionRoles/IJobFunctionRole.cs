using System.Collections.Generic;

namespace SDBServices.STS.DTO
{
    /// <summary>
    /// Represents a single JobFunctionRole.
    /// </summary>
    public interface IJobFunctionRole : IItem
    {
        /// <summary>
        /// Contains the CVR number of the municipality. This is used to access data from different municipalities.
        /// 
        /// For now, this contains the CVR number of GK, as configured in the Web.config-file of the main project.
        /// </summary>
        string cvr { get; }

        /// <summary>
        /// Contains the data constraints for this specific JobFunctionRole.
        /// </summary>
        IEnumerable<IDataConstraint> dataConstraints { get; }
    }
}