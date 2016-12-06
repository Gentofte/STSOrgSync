using System;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Security;

namespace Organisation.IntegrationLayer
{
    internal static class ConnectionHelper
    {
        private static OrganisationRegistryProperties stsProperties = OrganisationRegistryProperties.GetInstance();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static string stsAddressSuffix = "runtime/services/kombittrust/14/certificatemixed";

        public static SecurityToken SendRequestSecurityTokenRequest(string appliesTo, X509Certificate2 clientCertificate, string cvr)
        {
            var rst = new RequestSecurityToken
            {
                AppliesTo = new EndpointReference(appliesTo),
                RequestType = RequestTypes.Issue,
                TokenType = "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV2.0",
                KeyType = KeyTypes.Asymmetric,
                Issuer = new EndpointReference(stsProperties.StsBaseAddress),
                UseKey = new UseKey(new X509SecurityToken(clientCertificate))
            };

            if (!string.IsNullOrEmpty(cvr))
            {
                rst.Claims.Dialect = "http://docs.oasis-open.org/wsfed/authorization/200706/authclaims";
                rst.Claims.Add(new RequestClaim("dk:gov:saml:attribute:CvrNumberIdentifier", false, cvr));
            }

            var client = GenerateStsCertificateClientChannel(clientCertificate);

            SecurityToken token;
            try {
               token = client.Issue(rst);
            }
            catch (Exception ex) {
                string message = "Failed to establish connection to the STS";
                log.Error(message, ex);
                throw new STSNotFoundException(message, ex);
            }

            return token;
        }

        private static IWSTrustChannelContract GenerateStsCertificateClientChannel(X509Certificate2 clientCertificate)
        {
            var stsAddress = new EndpointAddress(new Uri(stsProperties.StsBaseAddress + stsAddressSuffix), EndpointIdentity.CreateDnsIdentity(stsProperties.StsCertAlias));
            var binding = new MutualCertificateWithMessageSecurityBinding(null);
            var factory = new WSTrustChannelFactory(binding, stsAddress);
            factory.TrustVersion = TrustVersion.WSTrust13;

            factory.Credentials.ClientCertificate.Certificate = clientCertificate;
            factory.Credentials.ServiceCertificate.ScopedCertificates.Add(stsAddress.Uri, CertificateLoader.LoadCertificateFromTrustedPeopleStore(stsProperties.StsCertThumbprint));
            factory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;
            factory.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
            factory.Endpoint.Contract.ProtectionLevel = ProtectionLevel.Sign;

            if (stsProperties.LogRequestResponse)
            {
                factory.Endpoint.Behaviors.Add(new LoggingBehavior("STS", "RequestSecurityToken"));
            }

            return factory.CreateChannel();
        }
    }
}
