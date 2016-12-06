
namespace Organisation.ServiceLayer
{
    public class ServiceMessages
    {
        private const string SERVICE_NAME_OU = "OrgUnit REST service";
        private const string SERVICE_NAME_USER = "User REST service";
        private const string SERVICE_NAME_ITSYSTEM = "ItSystem REST service";

        public string UPDATE_FAILED = "Could not update object";
        public string DELETE_FAILED = "Could not delete object";

        public string GetOuMessage(string message) {
            return string.Format("{0}, {1}",SERVICE_NAME_OU, message);
        }

        public string GetUserMessage(string message)
        {
            return string.Format("{0}, {1}", SERVICE_NAME_USER, message);
        }

        public string GetItSystemMessage(string message)
        {
            return string.Format("{0}, {1}", SERVICE_NAME_ITSYSTEM, message);
        }
    }
}
