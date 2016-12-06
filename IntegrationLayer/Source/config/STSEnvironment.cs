
using System;

namespace Organisation.IntegrationLayer
{
    internal class STSEnvironment : Environment
    {
        public string GetServicesCertAlias()
        {
            return "Organisation_T (funktionscertifikat)";
        }

        public string GetServicesCertThumbprint()
        {
            return "e0 2f 7d 39 a6 6b d9 bc 85 e7 24 57 e2 49 87 6d f7 8c 83 24";
        }

        public string GetServicesBaseUrl()
        {
            return "https://organisation.eksterntest-stoettesystemerne.dk/sts-soap-organisation/";
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
            return "http://organisation.eksterntest-stoettesystemerne.dk/service/";
        }
    }
}
