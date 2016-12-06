using System;

namespace OrganisationInspector
{
    /// <summary>
    /// Assign Validator instance depending on what type of object is passed
    /// </summary>
    public class ValidatorBroker
    {
        public Validator GetValidator(string objectType, string uuid)
        {
            switch (objectType)
            {
                case "OrgUnit":
                    return new OrgUnitValidator(uuid);
                case "User":
                    return new UserValidator(uuid);
                case "Address":
                    return new AddressValidator(uuid);
                case "Person":
                    return new PersonValidator(uuid);
                case "OrgFunction":
                    return new FunctionValidator(uuid);
                default:
                    throw new Exception("Non recognized type");
            }
        }
    }
}
