using Organisation.BusinessLayer;
using Organisation.BusinessLayer.DTO.V1_1;
using Organisation.SchedulingLayer;
using RestSharp;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Organisation.TestDriver
{
    public class Program
    {
        public static int Main(string[] args)
        {
            // Initialize BusinessLayer
            Initializer.Init();

            // Initialize SchedulingLayer
            SchedulingLayer.SyncJobRunner.Init();

            // Initialize WebService
            ServiceLayer.ServiceInitializer.Init();

            Console.WriteLine("Service started...");
            RestClient client = getRestClient();

            // does not really have anything to do with ServiceLayer, but it is a convinient place to test the SchedulingLayer
            testWriteReadOUThroughDB();

            string userUUID = GetFreshUuid();
            testReadUser(client, userUUID);

            string ouUUID = GetFreshUuid();
            testReadOU(client, ouUUID);

            userUUID = GetFreshUuid();
            IRestResponse userRegistration = testCreateUser(client, userUUID);
            IRestResponse userDelete = testDelete(client, "user", userUUID);

            ouUUID = GetFreshUuid();
            IRestResponse<OrgUnitRegistration> OURegistration = testCreateOrgUnit(client, ouUUID);
            IRestResponse orgUnitDelete = testDelete(client, "orgunit", ouUUID);

            Environment.Exit(0);

            return 0;
        }

        private static void testWriteReadOUThroughDB()
        {
            OrgUnitDao dao = new OrgUnitDao();

            OrgUnitRegistration registration = new OrgUnitRegistration();
            registration.Name = "Navn";
            registration.Uuid = GetFreshUuid();
            registration.ContactPlaces.Add(new BusinessLayer.DTO.V1_1.ContactPlace()
            {
                OrgUnitUuid = GetFreshUuid(),
                Tasks = new List<string>() { GetFreshUuid(), GetFreshUuid(), GetFreshUuid() }
            });
            registration.ContactPlaces.Add(new BusinessLayer.DTO.V1_1.ContactPlace()
            {
                OrgUnitUuid = GetFreshUuid(),
                Tasks = new List<string>() { GetFreshUuid(), GetFreshUuid() }
            });

            // save OU with 2 ContactPlaces with 2 and 3 Tasks associated
            dao.Save(registration, OperationType.UPDATE);

            // read and see that we get the expected back
            OrgUnitRegistrationExtended fromDaoRegistration = dao.GetOldestEntry();
        }

        private static IRestResponse<UserRegistration> testCreateUser(RestClient client, string uuid)
        {
            UserRegistration user = new UserRegistration();
            user.Email = new Address();
            user.Email.Value = "dto@digital-identity.dk";
            user.Location = new Address();
            user.Location.Value = "Kontor 15";
            user.Person.Name = "Daniel";
            user.Timestamp = DateTime.Now;
            user.UserId = "dto";
            user.Uuid = uuid;

            BusinessLayer.DTO.V1_1.Position position = new BusinessLayer.DTO.V1_1.Position();
            position.Name = "Udvikler";
            position.OrgUnitUuid = Guid.NewGuid().ToString().ToLower();
            user.Positions.Add(position);

            var request = new RestRequest("/api/v1_1/user/");
            request.Method = Method.POST;
            request.AddJsonBody(user);
            request.RequestFormat = DataFormat.Json;
            return client.Execute<UserRegistration>(request);
        }

        private static void testReadUser(RestClient client, string uuid)
        {
            UserRegistration user = new UserRegistration();
            user.Email.Value = "dto@digital-identity.dk";
            user.Location.Value = "Kontor 15";
            user.Person.Name = "Daniel";
            user.Timestamp = DateTime.Now;
            user.UserId = "dto";
            user.Uuid = uuid;

            BusinessLayer.DTO.V1_1.Position position = new BusinessLayer.DTO.V1_1.Position();
            position.Name = "Udvikler";
            position.OrgUnitUuid = Guid.NewGuid().ToString().ToLower();
            user.Positions.Add(position);

            // store directly, then use service to read
            UserService userService = new UserService();
            userService.Update(user);

            var request = new RestRequest("/api/v1_1/user?uuid=" + uuid);
            request.Method = Method.GET;
            var response = client.Execute<UserRegistration>(request);

            UserRegistration responseRegistration = response.Data;
            if (!user.Email.Value.Equals(responseRegistration.Email.Value))
            {
                throw new Exception("Email mismatch");
            }

            if (!user.Location.Value.Equals(responseRegistration.Location.Value))
            {
                throw new Exception("Location mismatch");
            }

            if (!user.Person.Name.Equals(responseRegistration.Person.Name))
            {
                throw new Exception("Person.Name mismatch");
            }

            if (!user.UserId.Equals(responseRegistration.UserId))
            {
                throw new Exception("UserId mismatch");
            }
        }

        private static void testReadOU(RestClient client, string uuid)
        {
            OrgUnitRegistration ou = new OrgUnitRegistration();
            ou.Uuid = uuid;
            ou.Name = "testOU";
            ou.ParentOrgUnitUuid = GetFreshUuid();
            ou.Email.Value = "email@email.dk";

            // store directly, then use service to read
            OrgUnitService orgUnitService = new OrgUnitService();
            orgUnitService.Update(ou);

            var request = new RestRequest("/api/v1_1/orgunit?uuid=" + uuid);
            request.Method = Method.GET;
            var response = client.Execute<OrgUnitRegistration>(request);

            OrgUnitRegistration responseRegistration = response.Data;
            if (!ou.Email.Value.Equals(responseRegistration.Email.Value))
            {
                throw new Exception("Email mismatch");
            }

            if (!ou.Name.Equals(responseRegistration.Name))
            {
                throw new Exception("Name mismatch");
            }

            if (!ou.ParentOrgUnitUuid.Equals(responseRegistration.ParentOrgUnitUuid))
            {
                throw new Exception("Parent.UUID mismatch");
            }
        }

        private static IRestResponse<OrgUnitRegistration> testCreateOrgUnit(RestClient client, string orgUnitUUID)
        {
            OrgUnitRegistration registration = new OrgUnitRegistration();
            registration.Uuid = orgUnitUUID;
            registration.ShortKey = "abc";
            registration.ParentOrgUnitUuid = Guid.NewGuid().ToString().ToLower();
            registration.Name = "OrgUnit";
            registration.Timestamp = DateTime.Now;

            Address address = new Address();
            address.Uuid = Guid.NewGuid().ToString().ToLower();
            address.Value = "person@mail.dk";
            address.ShortKey = "ShortKey1";
            registration.Email = address;

            address = new Address();
            address.Uuid = Guid.NewGuid().ToString().ToLower();
            address.Value = "12345678";
            address.ShortKey = "ShortKey2";
            registration.Phone = address;

            address = new Address();
            address.Uuid = Guid.NewGuid().ToString().ToLower();
            address.Value = "eanField";
            address.ShortKey = "eanField";
            registration.Ean = address;

            address = new Address();
            address.Uuid = Guid.NewGuid().ToString().ToLower();
            address.Value = "12-16";
            address.ShortKey = "emailRM";
            registration.EmailRemarks = address;

            address = new Address();
            address.Uuid = Guid.NewGuid().ToString().ToLower();
            address.Value = "Bobvej 1, kontor 14";
            address.ShortKey = "contact";
            registration.Contact = address;

            address = new Address();
            address.Uuid = Guid.NewGuid().ToString().ToLower();
            address.Value = "Hans Vej 14, 7800 Skive";
            address.ShortKey = "retur";
            registration.PostReturn = address;

            address = new Address();
            address.Uuid = Guid.NewGuid().ToString().ToLower();
            address.Value = "12-16";
            address.ShortKey = "contactOH";
            registration.ContactOpenHours = address;

            address = new Address();
            address.Uuid = Guid.NewGuid().ToString().ToLower();
            address.Value = "12-16";
            address.ShortKey = "phoneOH";
            registration.PhoneOpenHours = address;

            var request = new RestRequest("/api/v1_1/orgunit/");
            request.Method = Method.POST;
            request.AddJsonBody(registration);
            request.RequestFormat = DataFormat.Json;
            return client.Execute<OrgUnitRegistration>(request);
        }

        private static IRestResponse testDelete(RestClient client, string objectType, string uuid)
        {
            var request = new RestRequest("/api/v1_1/" + objectType + "?uuid=" + uuid);
            request.Method = Method.DELETE;
            return client.Execute(request);
        }

        private static RestClient getRestClient()
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("http://localhost:9010/");
            return client;
        }

        private static object deserialize<T>(IRestResponse<T> response)
        {
            return JsonConvert.DeserializeObject<T>(response.Content, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
        }

        private static string GetFreshUuid()
        {
            return Guid.NewGuid().ToString().ToLower();
        }
    }
}
