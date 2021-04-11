namespace SDBServices.STS.DTO
{
    /// <summary>
    /// A Data-constraint as used in the JobFunctionRoles endpoint.
    /// </summary>
    public interface IDataConstraint
    {
        /// <summary>
        /// Points to the type of data-constraint that this object represents.
        /// 
        /// Examples: Organisational Unit, KLE, jobfunction.
        /// </summary>
        string id { get; }

        /// <summary>
        /// The value of the data-constraint.
        /// 
        /// In case of KLE-numers, this contains the reduced tree of KLE-numbers that
        /// this user is allowed to access cases from.
        /// </summary>
        string value { get; }
    }
}