using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organisation.IntegrationLayer
{
    internal interface Environment
    {
        string GetServicesCertAlias();
        string GetServicesCertThumbprint();
        string GetServicesBaseUrl();

        string GetSTSBaseUrl();
        string GetSTSCertAlias();
        string GetSTSCertThumbprint();

        string GetEntityIdBase();
    }
}
