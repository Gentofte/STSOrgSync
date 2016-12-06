using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;

namespace Organisation.IntegrationLayer
{
    internal class TokenCache
    {
        private static OrganisationRegistryProperties registryProperties = OrganisationRegistryProperties.GetInstance();
        private static TokenMap tokenMap = new TokenMap();
        private static Dictionary<string, string> entityIds = new Dictionary<string, string>();
        private static int renewBefore = 120;

        public static void Init()
        {
            setupEntityIds();
        }

        public static void IssueAllTokens()
        {
            foreach (KeyValuePair<string, string> entry in entityIds)
            {
                IssueToken(entry.Key, renewBefore);
            }
        }

        public static SecurityToken IssueToken(string key)
        {
            return IssueToken(key, 0);
        }

        private static SecurityToken IssueToken(string key, long minutes)
        {
            string entityId = entityIds[key];
            var token = tokenMap.getToken(entityId);

            if (token != null)
            {
                // force expire if argument is given
                if (token.ValidTo.CompareTo(DateTime.Now.AddMinutes(-1 * minutes)) <= 0)
                {
                    token = null;
                }
            }

            if (token == null)
            {
                token = ConnectionHelper.SendRequestSecurityTokenRequest(new Uri(entityId).AbsoluteUri, CertificateLoader.LoadCertificateFromMyStore(registryProperties.ClientCertThumbprint), registryProperties.Municipality);
                tokenMap.addToken(entityId, token);
            }

            return token;
        }

        private static void setupEntityIds()
        {
            string entityTemplate = registryProperties.StsEntityIdBase + "{0}" + "/1";

            entityIds.Add("organisationenhed", String.Format(entityTemplate, "organisationenhed"));
            entityIds.Add("organisationfunktion", String.Format(entityTemplate, "organisationfunktion"));
            entityIds.Add("bruger", String.Format(entityTemplate, "bruger"));
            entityIds.Add("person", String.Format(entityTemplate, "person"));
            entityIds.Add("adresse", String.Format(entityTemplate, "adresse"));
        }
    }
}
