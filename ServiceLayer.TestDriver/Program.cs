using Microsoft.Owin.Hosting;
using Organisation.BusinessLayer;
using RestSharp;
using System;
using Newtonsoft.Json;
using Organisation.SchedulingLayer;
using System.Collections.Generic;

namespace Organisation.TestDriver
{
    public class Program
    {
        public static int Main(string[] args)
        {
            ServiceLayer.ServiceInitializer.Init();
            Console.WriteLine("Service started...");
            RestClient client = getRestClient();
            UserDao UserDao = new UserDao();
            OrgUnitDao OrgUnitDao = new OrgUnitDao();
            ItSystemDao ItSystemDao = new ItSystemDao();

            string userUUID = GetFreshUuid();
            IRestResponse userRegistration = testCreateUser(client, userUUID);
           
            string ouUUID = GetFreshUuid();
            IRestResponse<OrgUnitRegistration> OURegistration = testCreateOrgUnit(client, ouUUID);

            string itSystemUUID = GetFreshUuid();
            IRestResponse<ItSystemRegistration> ITSystemRegistration = testCreateItSystem(client,itSystemUUID);

            // UserRegistration userReg = UserDao.GetOldestEntry();
           // ItSystemRegistration itSystem = ItSystemDao.GetOldestEntry();

            return 0;
        }
       
        private static IRestResponse<UserRegistration> testCreateUser(RestClient client, string uuid)
        {

            UserRegistration user = new UserRegistration();
            user.Email = new Address();
            user.Email.Value = "dto@digital-identity.dk";
            user.Location = new Address();
            user.Location.Value = "Kontor 15";
            user.PersonName = "Daniel";
            user.PositionName = "Udvikler";
            user.Timestamp = DateTime.Now;
            user.UserId = "dto";
            user.UserUuid = uuid;
            user.PositionOrgUnitUuid = Guid.NewGuid().ToString().ToLower();

            var request = new RestRequest("/api/user/");
            request.Method = Method.POST;
            request.AddJsonBody(user);
            request.RequestFormat = DataFormat.Json;
            return client.Execute<UserRegistration>(request);
        }

        private static IRestResponse<OrgUnitRegistration> testCreateOrgUnit(RestClient client, string orgUnitUUID)
        {
            OrgUnitService service = new OrgUnitService();
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

            var request = new RestRequest("/api/orgunit/");
            request.Method = Method.POST;
            request.AddJsonBody(registration);
            request.RequestFormat = DataFormat.Json;
            return client.Execute<OrgUnitRegistration>(request);
        }

        private static IRestResponse<ItSystemRegistration> testCreateItSystem(RestClient client, string uuid)
        {

            ItSystemRegistration itSystem = new ItSystemRegistration();
            itSystem.Uuid = uuid;
            itSystem.SystemShortKey = "systemshortkey";
            itSystem.Timestamp = DateTime.Now;
            itSystem.JumpUrl = "http://sapa.dk/";

            var request = new RestRequest("/api/itsystem/");
            request.Method = Method.POST;
            request.AddJsonBody(itSystem);
            request.RequestFormat = DataFormat.Json;
            return client.Execute<ItSystemRegistration>(request);
        }

        private static IRestResponse testDelete(RestClient client, string objectType, string uuid) {
            var request = new RestRequest("/api/"+ objectType +"?uuid=" + uuid);
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

        private static string GetFreshUuid() {
            return Guid.NewGuid().ToString().ToLower();
        }
    }
}
