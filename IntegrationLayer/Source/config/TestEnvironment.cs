
using System;

namespace Organisation.IntegrationLayer
{
    internal class TestEnvironment : Environment
    {
        public string GetServicesCertAlias()
        {
            return "*.serviceplatformen.dk";
        }

        public string GetServicesCertThumbprint()
        {
            return "d1 f8 54 d5 41 cf 4a ac dc 4d 1f ad 8c 7c 61 b9 8b 0a 4d 60";
        }

        public string GetServicesBaseUrl()
        {
            return "https://exttest.serviceplatformen.dk/service/Organisation/";
        }

        public string GetSTSBaseUrl()
        {
            return "https://adgangsstyring.eksterntest-stoettesystemerne.dk/";
        }

        public string GetSTSCertAlias()
        {
            return "KOMBIT Støttesystemer-T (funktionscertifikat)";
        }

        public string GetSTSCertThumbprint()
        {
            return "3a 10 3c f6 0a 9d 97 7b a5 a4 99 3e 06 d8 c9 ba 36 f2 58 3e";
        }

        public string GetEntityIdBase()
        {
            // TODO: this points to STS directly, but we do not use tokens for SP, so that is fine
            return "http://organisation.eksterntest-stoettesystemerne.dk/service/";
        }
    }
}
