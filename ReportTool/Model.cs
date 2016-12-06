using System.Collections.Generic;

namespace Organisation.ReportTool
{
    public class Model
    {
        public string AsciiTreeRepresentation { get; set; }
        public List<OUModel> OUs { get; set; }
        public List<UserModel> Users { get; set; }
    }

    public class OUModel
    {
        public string Name { get; set; }
        public string Uuid { get; set; }
        public string AddressDetails { get; set; }
        public string EmployeesDetails { get; set; }
        public string BrugervendtNoegle { get; set; }
    }

    public class UserModel
    {
        public string Name { get; set; }
        public string Uuid { get; set; }
        public string UserId { get; set; }
        public string Cpr { get; set; }
        public string ShortKey { get; set; }
        public string AddressDetails { get; set; }
    }
}
