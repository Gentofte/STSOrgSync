using System.Collections.Generic;

namespace Organisation.ReportTool
{
    public class Model
    {
        public string AsciiTreeRepresentation { get; set; }
        public List<OUModel> OUs { get; set; }
        public List<UserModel> Users { get; set; }
        public List<PayoutUnitModel> PayoutUnits { get; set; }
        public List<ContactPlacesModel> ContactPlaces { get; set; }
        public List<OUItSystem> ItSystems { get; set; }
    }

    public class OUModel
    {
        public string Name { get; set; }
        public string Uuid { get; set; }
        public string AddressDetails { get; set; }
        public string EmployeesDetails { get; set; }
        public string BrugervendtNoegle { get; set; }
        public string Status { get; set; }
        public List<string> Errors { get; set; }
    }

    public class UserModel
    {
        public string Name { get; set; }
        public string Uuid { get; set; }
        public string UserId { get; set; }
        public string Cpr { get; set; }
        public string ShortKey { get; set; }
        public string AddressDetails { get; set; }
        public string Status { get; set; }
        public List<string> Errors { get; set; }
    }

    public class ContactPlacesModel
    {
        public string ContactUnitName { get; set; }
        public string ContactUnitUuid { get; set; }
        public List<string> Opgaver { get; set; }
        public string UnitName { get; set; }
        public string UnitUuid { get; set; }
    }

    public class PayoutUnitModel
    {
        public string PayoutUnitName { get; set; }
        public string PayoutUnitUuid { get; set; }
        public string PayoutUnitLOSShortKey { get; set; }
        public string UnitName { get; set; }
        public string UnitUuid { get; set; }
    }

    public class OUItSystem
    {
        public string Uuid { get; set; }
        public List<string> Enheder { get; set; } = new List<string>();
    }
}
