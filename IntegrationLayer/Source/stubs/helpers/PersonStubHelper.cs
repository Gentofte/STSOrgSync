﻿using System;
using IntegrationLayer.Person;
using System.Security.Cryptography.X509Certificates;

namespace Organisation.IntegrationLayer
{
    internal class PersonStubHelper
    {
        internal const string SERVICE = "person";
        private static OrganisationRegistryProperties registryProperties = OrganisationRegistryProperties.GetInstance();

        internal void AddProperties(string navn, string shortKey, string cprnummer, VirkningType virkning, RegistreringType1 registration)
        {
            EgenskabType property = new EgenskabType();
            property.CPRNummerTekst = cprnummer;
            property.Virkning = virkning;
            property.NavnTekst = navn;
            property.BrugervendtNoegleTekst = shortKey;

            EgenskabType[] egenskab = new EgenskabType[1];
            egenskab[0] = property;

            registration.AttributListe = egenskab;
        }

        internal PersonPortTypeClient CreatePort()
        {
            CustomLibBasBinding binding = new CustomLibBasBinding();

            // TODO: old STS tcode
            // PersonPortTypeClient port = new PersonPortTypeClient(binding, StubUtil.GetEndPointAddress(SERVICE));

            // TODO: new SP code (pre-token)
            PersonPortTypeClient port = new PersonPortTypeClient(binding, StubUtil.GetEndPointAddress("Person/1"));
            port.ClientCredentials.ClientCertificate.SetCertificate(StoreLocation.LocalMachine, StoreName.My, X509FindType.FindByThumbprint, registryProperties.ClientCertThumbprint);

            // Disable revocation checking
            if (registryProperties.DisableRevocationCheck)
            {
                port.ClientCredentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
            }

            return port;
        }

        internal RegistreringType1 CreateRegistration(DateTime timestamp, LivscyklusKodeType livCyklusKodeType)
        {
            UnikIdType systemReference = GetOrganisationReference();
            RegistreringType1 registration = new RegistreringType1();

            registration.Tidspunkt = timestamp;
            registration.TidspunktSpecified = true;
            registration.LivscyklusKode = livCyklusKodeType;
            registration.LivscyklusKodeSpecified = true;
            registration.BrugerRef = systemReference;

            registration.AttributListe = null;
            registration.RelationListe = new RelationListeType();
            registration.TilstandListe = new TilstandListeType();

            return registration;
        }

        internal UnikIdType GetOrganisationReference()
        {
            return StubUtil.GetReference<UnikIdType>(StubUtil.GetMunicipalityOrganisationUUID(), ItemChoiceType.UUIDIdentifikator);
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

        internal PersonType GetPersonType(string uuid, RegistreringType1 registration)
        {
            PersonType organisationType = new PersonType();
            organisationType.UUIDIdentifikator = uuid;

            RegistreringType1[] registreringTypes = new RegistreringType1[1];
            registreringTypes[0] = registration;
            organisationType.Registrering = registreringTypes;

            return organisationType;
        }
    }
}
