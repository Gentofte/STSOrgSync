﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace Organisation.IntegrationLayer
{
    internal class OrganisationRegistryProperties
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string REGISTRY_LOCATION = @"SOFTWARE\Wow6432node\Digital Identity\STSOrgSync";

        private const string ORGANISATION_UUID_KEY = "OrganisationUUID";
        private const string LOG_REQUEST_RESPONSE = "LogRequestResponse";
        private const string REVOCATION_CHECK_KEY = "DisableRevocationCheck";
        private const string MUNICIPALITY_KEY = "Municipality";
        private const string DB_CONNECTION_STRING_KEY = "DBConnectionString";
        private const string CLIENTCERT_THUMBPRINT_KEY = "ClientCertThumbprint";
        private const string ENVIRONMENT_KEY = "Environment";
        private const string USE_SSL_KEY = "UseSSL";

        private static OrganisationRegistryProperties instance;

        public string ClientCertThumbprint { get; set; }
        public string ServicesBaseUrl { get; set; }
        public bool LogRequestResponse { get; set; }
        public bool DisableRevocationCheck { get; set; }
        public string ServiceCertAlias { get; set; }
        public Dictionary<string, string> MunicipalityOrganisationUUID { get; set; }
        public string DBConnectionString { get; set; }
        public bool UseSSL { get; set; }
        public string DefaultMunicipality { get; set; }

        [ThreadStatic]
        public static string Municipality;

        private OrganisationRegistryProperties()
        {
            try
            {
                Init();
                log.Info("Loaded Registry Properties");
            }
            catch (Exception ex)
            {
                string message = "Could not open registry properties: " + ex.Message;
                log.Error(message);
                throw new RegistryException(message, ex);
            }
        }

        public static OrganisationRegistryProperties GetInstance()
        {
            if (instance != null)
            {
                return instance;
            }

            return (instance = new OrganisationRegistryProperties());
        }

        public static string GetMunicipality()
        {
            if (string.IsNullOrEmpty(Municipality))
            {
                return GetInstance().DefaultMunicipality;
            }

            return Municipality;
        }

        private void Init()
        {
            var key = Registry.LocalMachine.OpenSubKey(REGISTRY_LOCATION);

            ClientCertThumbprint = (string)key.GetValue(CLIENTCERT_THUMBPRINT_KEY);
            DefaultMunicipality = (string)key.GetValue(MUNICIPALITY_KEY);
            LogRequestResponse = "true".Equals((string)key.GetValue(LOG_REQUEST_RESPONSE));
            DBConnectionString = (string)key.GetValue(DB_CONNECTION_STRING_KEY);
            DisableRevocationCheck = "true".Equals((string)key.GetValue(REVOCATION_CHECK_KEY));
            UseSSL = "true".Equals((string)key.GetValue(USE_SSL_KEY));

            // read all cvr:uuid mappings
            MunicipalityOrganisationUUID = new Dictionary<string, string>();
            string municipalityMap = (string)key.GetValue(ORGANISATION_UUID_KEY);
            string[] tokens = municipalityMap.Split(';');
            foreach (var token in tokens)
            {
                string[] pair = token.Split(':');

                MunicipalityOrganisationUUID[pair[0]] = pair[1];
            }

            // strip out nasty characters that sometimes are added when copy/pasting thumbprints from Windows UI's
            string tmp = "";
            foreach (char c in ClientCertThumbprint.ToCharArray())
            {
                if (char.IsLetterOrDigit(c) || c == ' ')
                {
                    tmp += c;
                }
            }
            ClientCertThumbprint = tmp;

            Environment environment = null;
            string environmentKey = (string)key.GetValue(ENVIRONMENT_KEY);
            if (environmentKey.Equals("TEST"))
            {
                environment = new TestEnvironment();
            }
            else if (environmentKey.Equals("PROD"))
            {
                environment = new ProdEnvironment();
            }
            else
            {
                throw new Exception("Invalid registry setting for Environment (" + environmentKey + ") - TEST or PROD allowed");
            }

            ServicesBaseUrl = environment.GetServicesBaseUrl();
        }
    }
}
