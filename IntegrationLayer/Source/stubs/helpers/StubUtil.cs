using System;
using System.ServiceModel;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;

namespace Organisation.IntegrationLayer
{
    internal static class StubUtil
    {
        private static OrganisationRegistryProperties registryProperties = OrganisationRegistryProperties.GetInstance();

        public static string GetMunicipalityOrganisationUUID()
        {
            return registryProperties.MunicipalityOrganisationUUID;
        }

        public static EndpointAddress GetEndPointAddress(string suffix)
        {
            EndpointIdentity identity = EndpointIdentity.CreateDnsIdentity(registryProperties.ServiceCertAlias);
            EndpointAddress endPointAddress = new EndpointAddress(new Uri(registryProperties.ServicesBaseUrl + suffix), identity);

            return endPointAddress;
        }

        public static string ConstructSoapErrorMessage(int statusCode, string operation, string service, string FejlbeskedTekst)
        {
            return "Service '" + service + "." + operation + "' returned (" + statusCode + ") with message: " + FejlbeskedTekst;
        }

        public static Uri GetUri(string suffix)
        {
            return new Uri(registryProperties.ServicesBaseUrl + suffix);
        }

        public static bool TerminateVirkning(dynamic virkning, DateTime timestamp)
        {
            object current = virkning.TilTidspunkt.Item;
            if (current == null || !(current is DateTime))
            {
                virkning.TilTidspunkt.Item = timestamp.Date + new TimeSpan(0, 0, 0);
                return true;
            }

            return false;
        }

        public static T GetReference<T>(string uuid, dynamic type) where T : new()
        {
            dynamic reference = new T();
            reference.Item = uuid;
            reference.ItemElementName = type;

            return reference;
        }

        public static AdressePortType CreateChannel<AdressePortType>(string service, string operation, dynamic port, SecurityToken token)
        {
            X509Certificate2 cert = CertificateLoader.LoadCertificateFromTrustedPeopleStore(registryProperties.ServiceCertThumbprint);
            Uri uri = GetUri(service);

            port.ChannelFactory.Credentials.ServiceCertificate.ScopedCertificates.Add(uri, cert);
            port.ChannelFactory.Endpoint.Binding = new CustomLibBasBinding();
            port.ChannelFactory.Endpoint.EndpointBehaviors.Add(new CustomLibBasClientBehavior());

            if (registryProperties.LogRequestResponse)
            {
                port.ChannelFactory.Endpoint.EndpointBehaviors.Add(new LoggingBehavior(service, operation));
            }

            return port.ChannelFactory.CreateChannelWithIssuedToken(token);
        }

        public static EgenskabType GetLatestProperty<EgenskabType>(EgenskabType[] properties)
        {
            foreach (dynamic property in properties)
            {
                // find the first open-ended EgenskabType - objects created by this library does not have end-times associated with them as a rule
                object endTime = property.Virkning.TilTidspunkt.Item;
                if (!(endTime is DateTime))
                {
                    return property;
                }
            }

            return default(EgenskabType);
        }

        public static GyldighedType GetLatestGyldighed<GyldighedType>(GyldighedType[] states)
        {
            foreach (dynamic state in states)
            {
                // find the first open-ended GyldighedType - objects created by this library does not have end-times associated with them as a rule
                object endTime = state.Virkning.TilTidspunkt.Item;
                if (!(endTime is DateTime))
                {
                    return state;
                }
            }

            return default(GyldighedType);
        }

        public static bool TerminateObjectsInOrgNoLongerPresentLocally(dynamic orgArray, dynamic localArray, DateTime timestamp, bool uuidSubReference)
        {
            bool changes = false;

            if (orgArray != null)
            {
                foreach (var objectInOrg in orgArray)
                {
                    bool found = false;

                    if (localArray != null)
                    {
                        foreach (var objectInLocal in localArray)
                        {
                            // we need the UuidSubReference check on both sides of the || to ensure that the second part of the && clause is not evaluated unless needed
                            if ((uuidSubReference && objectInLocal.Uuid.Equals(objectInOrg.ReferenceID.Item)) || (!uuidSubReference && objectInLocal.Equals(objectInOrg.ReferenceID.Item)))
                            {
                                found = true;
                            }
                        }
                    }

                    if (!found)
                    {
                        // as this is a FlerRelation, we also get all the old references, so only mark the object as
                        // changed if we actually terminate a valid Virkning
                        if (StubUtil.TerminateVirkning(objectInOrg.Virkning, timestamp))
                        {
                            changes = true;
                        }
                    }
                }
            }

            return changes;
        }

        public static List<string> FindAllObjectsInLocalNotInOrg(dynamic orgArray, dynamic localArray, bool uuidSubReference)
        {
            List<string> uuidsToAdd = new List<string>();

            if (localArray != null)
            {
                foreach (var objectInLocal in localArray)
                {
                    bool found = false;

                    if (orgArray != null)
                    {
                        foreach (var objectInOrg in orgArray)
                        {
                            if ((uuidSubReference && objectInLocal.Uuid.Equals(objectInOrg.ReferenceID.Item)) || (!uuidSubReference && objectInLocal.Equals(objectInOrg.ReferenceID.Item)))
                            {
                                // DateTime means this relationship has been terminated, and we should not count it as being present
                                if (!(objectInOrg.Virkning.TilTidspunkt.Item is DateTime))
                                {
                                    found = true;
                                }
                            }
                        }
                    }

                    if (!found)
                    {
                        uuidsToAdd.Add((uuidSubReference) ? objectInLocal.Uuid : objectInLocal);
                    }
                }
            }

            return uuidsToAdd;
        }
    }
}
