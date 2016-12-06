using Organisation.BusinessLayer;
using Organisation.IntegrationLayer;
using System;

namespace Organisation.ReportTool
{
    public class DataGenerator
    {
        private static Random random = new Random();
        public const string ROOT_UUID = "663B2891-6544-491C-8CE1-F839270D391B";

        public void Generate()
        {
            string it = TestCreateOrgUnit(null, "ITDepartment", "Klamsagervej 20", "itd", true);

            string operations = TestCreateOrgUnit(it, "Operations", "Klamsagervej 21", "op", false);
            string projects = TestCreateOrgUnit(it, "Projects", "Klamsagervej 22", "proj", false);

            string printers = TestCreateOrgUnit(operations, "Printers", "Klamsagervej 23", "print", false);
            string esdh = TestCreateOrgUnit(projects, "ESDH", "Klamsagervej 24", "ESDH", false);
            string kombitsts = TestCreateOrgUnit(projects, "KOMBIT-STS", "Klamsagervej 25", "ksts", false);

            CreateUser("Daniel Torres", "1122334455", "DTO", "Developer", "DEV", operations, "dto@digital-identity.dk", "dto - mail");
            CreateUser("Brian Graversen", "1222331425", "BSG", "Developer", "DEV", projects, "bsg@digital-identity.dk", "bsg - mail");
        }

        private string TestCreateOrgUnit(string parentUUID, string name, string address, string shortkey, bool isRoot)
        {
            string uuid = null;
            if (isRoot)
            {
                uuid = ROOT_UUID;
            }
            else
            {
                uuid = Guid.NewGuid().ToString().ToLower();
            }

            DateTime timestamp = DateTime.Now.AddMinutes(-20);
            OrgUnitService service = new OrgUnitService();

            OrgUnitRegistration registration = new OrgUnitRegistration();
            registration.Uuid = uuid;
            registration.Name = name;
            registration.ShortKey = shortkey;
            registration.Timestamp = timestamp;

            if (parentUUID != null)
            {
                registration.ParentOrgUnitUuid = parentUUID;
            }

            Address locationAddress = new Address();
            locationAddress.Uuid = IdUtil.GenerateUuid();
            locationAddress.Value = address;
            locationAddress.ShortKey = IdUtil.GenerateShortKey();
            registration.Location = locationAddress;

            service.Create(registration);

            return uuid;
        }

        public void CreateUser(string name, string cpr, string shortkey, string position, string positionShortkey, string positionUUID, string email, string emailShortkey)
        {
            string uuid = Guid.NewGuid().ToString().ToLower();
            DateTime timestamp = DateTime.Now.AddMinutes(-20);
            UserService service = new UserService();

            UserRegistration registration = new UserRegistration();
            registration.PersonCpr = cpr;
            registration.PersonName = name;
            registration.PersonShortKey = shortkey;
            registration.PersonUuid = Guid.NewGuid().ToString().ToLower();
            registration.PositionName = position;
            registration.PositionUuid = Guid.NewGuid().ToString().ToLower();
            registration.PositionShortKey = positionShortkey;
            registration.PositionOrgUnitUuid = positionUUID;
            registration.Timestamp = timestamp;
            registration.UserId = shortkey;
            registration.UserShortKey = shortkey;
            registration.UserUuid = uuid;

            Address emailAddress = new Address();
            emailAddress.Uuid = Guid.NewGuid().ToString().ToLower();
            emailAddress.Value = email;
            emailAddress.ShortKey = emailShortkey;
            registration.Email = emailAddress;

            service.Update(registration);
        }
    }
}
