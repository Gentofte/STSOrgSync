﻿using System;
using IntegrationLayer.Organisation;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;

namespace Organisation.IntegrationLayer
{
    internal class OrganisationStubHelper
    {
        internal const string SERVICE = "organisation";
        private static OrganisationRegistryProperties registryProperties = OrganisationRegistryProperties.GetInstance();

        internal void AddOverordnetEnhed(string overordnetEnhedUUID, VirkningType virkning, RegistreringType1 registration)
        {
            UnikIdType orgUnitReference = StubUtil.GetReference<UnikIdType>(overordnetEnhedUUID, ItemChoiceType.UUIDIdentifikator);

            OrganisationEnhedRelationType organisationEnhedRelationType = new OrganisationEnhedRelationType();
            organisationEnhedRelationType.Virkning = virkning;
            organisationEnhedRelationType.ReferenceID = orgUnitReference;

            registration.RelationListe.Overordnet = organisationEnhedRelationType;
        }

        internal VirkningType GetVirkning(DateTime timestamp)
        {
            TidspunktType beginTime = new TidspunktType();
            beginTime.Item = timestamp.Date + new TimeSpan(0, 0, 0);

            VirkningType virkning = new VirkningType();
            virkning.AktoerRef = GetOrganisationReference();
            virkning.AktoerTypeKode = AktoerTypeKodeType.Organisation;
            virkning.AktoerTypeKodeSpecified = true;
            virkning.FraTidspunkt = beginTime;

            return virkning;
        }

        internal OrganisationPortTypeClient CreatePort()
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.Security.Mode = BasicHttpSecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;

            OrganisationPortTypeClient port = new OrganisationPortTypeClient(binding, StubUtil.GetEndPointAddress("Organisation/2"));
            port.ClientCredentials.ClientCertificate.SetCertificate(StoreLocation.LocalMachine, StoreName.My, X509FindType.FindByThumbprint, registryProperties.ClientCertThumbprint);

            // Disable revocation checking
            if (registryProperties.DisableRevocationCheck)
            {
                port.ClientCredentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
            }

            return port;
        }

        private GyldighedType GetGyldighedType(GyldighedStatusKodeType statusCode, VirkningType virkning)
        {
            GyldighedType gyldighed = new GyldighedType();
            gyldighed.GyldighedStatusKode = statusCode;
            gyldighed.Virkning = virkning;

            return gyldighed;
        }

        private UnikIdType GetOrganisationReference()
        {
            return StubUtil.GetReference<UnikIdType>(StubUtil.GetMunicipalityOrganisationUUID(), ItemChoiceType.UUIDIdentifikator);
        }
    }
}
