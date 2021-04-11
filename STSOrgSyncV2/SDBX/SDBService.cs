using RestSharp;
using RestSharp.Authenticators;
using System.Net;
using SDBServices.STS.DTO;
using System;

namespace SDB
{
    public class SDBService
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string sdbEndpoint;

        public SDBService(string sdbEndpoint)
        {
            this.sdbEndpoint = sdbEndpoint;
        }

        public STSUser InspectUser(string objectGuid, string dn)
        {
            string url = "users/" + objectGuid;
            var request = new RestRequest(url, Method.GET);
            request.RequestFormat = DataFormat.Json;

            IRestResponse<STSUser> response = null;
            try
            {
                response = getRestClient().Execute<STSUser>(request);
            }
            catch (WebException ex)
            {
                throw new SDBException("Failed to connect to SDB (" + url + ")", ex);
            }

            if (response.StatusCode.Equals(HttpStatusCode.OK) && response.Data != null)
            {
                Log.Debug("SDB returned status code 200 0K");

                return response.Data;
            }

            throw new Exception("Could not inspect user " + dn + ". Attempt to connect to " + url + " failed with HTTP " + response.StatusCode.ToString() + " and message: " + response.Content);
        }

        public STSOU InspectOrgUnit(string objectGuid, string dn)
        {
            string url = "ous/" + objectGuid;
            var request = new RestRequest(url, Method.GET);
            request.RequestFormat = DataFormat.Json;

            IRestResponse<STSOU> response = null;
            try
            {
                response = getRestClient().Execute<STSOU>(request);
            }
            catch (WebException ex)
            {
                throw new SDBException("Failed to connect to SDB (" + url + ")", ex);
            }

            if (response.StatusCode.Equals(HttpStatusCode.OK) && response.Data != null)
            {
                Log.Debug("SDB returned status code 200 0K");

                return response.Data;
            }

            throw new Exception("Could not inspect OU " + dn + ". Attempt to connect to " + url + " failed with HTTP " + response.StatusCode.ToString() + " and message: " + response.Content);
        }

        private RestClient getRestClient()
        {
            RestClient client = new RestClient(sdbEndpoint);
            client.Authenticator = new NtlmAuthenticator();

            return client;
        }
    }
}
