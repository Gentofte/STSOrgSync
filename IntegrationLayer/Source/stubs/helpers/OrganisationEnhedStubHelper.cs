using System;
using System.Collections.Generic;
using IntegrationLayer.OrganisationEnhed;
using System.Security.Cryptography.X509Certificates;

namespace Organisation.IntegrationLayer
{
    internal class OrganisationEnhedStubHelper
    {
        internal const string SERVICE = "organisationenhed";
        private static OrganisationRegistryProperties registryProperties = OrganisationRegistryProperties.GetInstance();

        internal void AddTilknyttedeFunktioner(List<string> tilknytteFunktioner, VirkningType virkning, RegistreringType1 registration)
        {
            if (tilknytteFunktioner == null || tilknytteFunktioner.Count == 0)
            {
                return;
            }

            OrganisationFunktionFlerRelationType[] orgFunktionFlerRelationTypes = new OrganisationFunktionFlerRelationType[tilknytteFunktioner.Count];

            for (int i = 0; i < tilknytteFunktioner.Count; i++)
            {
                UnikIdType tilknytteFunktionId = StubUtil.GetReference<UnikIdType>(tilknytteFunktioner[i], ItemChoiceType.UUIDIdentifikator);

                OrganisationFunktionFlerRelationType orgFunktionFlerRelationType = new OrganisationFunktionFlerRelationType();
                orgFunktionFlerRelationType.ReferenceID = tilknytteFunktionId;
                orgFunktionFlerRelationType.Virkning = virkning;

                orgFunktionFlerRelationTypes[i] = orgFunktionFlerRelationType;
            }

            registration.RelationListe.TilknyttedeFunktioner = orgFunktionFlerRelationTypes;
        }

        internal void AddOrganisationRelation(string organisationUUID, VirkningType virkning, RegistreringType1 registration)
        {
            UnikIdType orgReference = StubUtil.GetReference<UnikIdType>(organisationUUID, ItemChoiceType.UUIDIdentifikator);

            OrganisationRelationType organisationRelationType = new OrganisationRelationType();
            organisationRelationType.Virkning = virkning;
            organisationRelationType.ReferenceID = orgReference;

            registration.RelationListe.Tilhoerer = organisationRelationType;
        }

        internal OrganisationEnhedType GetOrganisationEnhedType(string uuid, RegistreringType1 registration)
        {
            OrganisationEnhedType organisationType = new OrganisationEnhedType();
            organisationType.UUIDIdentifikator = uuid;
            RegistreringType1[] registreringTypes = new RegistreringType1[1];
            registreringTypes[0] = registration;
            organisationType.Registrering = registreringTypes;

            return organisationType;
        }

        internal void AddOverordnetEnhed(string overordnetEnhedUUID, VirkningType virkning, RegistreringType1 registration)
        {
            // allowed to be empty for top-level OU's
            if (String.IsNullOrEmpty(overordnetEnhedUUID))
            {
                return;
            }

            UnikIdType orgUnitReference = StubUtil.GetReference<UnikIdType>(overordnetEnhedUUID, ItemChoiceType.UUIDIdentifikator);

            OrganisationEnhedRelationType organisationEnhedRelationType = new OrganisationEnhedRelationType();
            organisationEnhedRelationType.Virkning = virkning;
            organisationEnhedRelationType.ReferenceID = orgUnitReference;

            registration.RelationListe.Overordnet = organisationEnhedRelationType;
        }

        internal void SetTilstandToActive(VirkningType virkning, RegistreringType1 registration)
        {
            GyldighedType gyldighed = GetGyldighedType(GyldighedStatusKodeType.Aktiv, virkning);
            GyldighedType[] gyldigheds = new GyldighedType[1];
            gyldigheds[0] = gyldighed;
            registration.TilstandListe.Gyldighed = gyldigheds;
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

        internal RegistreringType1 CreateRegistration(OrgUnitData ou, LivscyklusKodeType livcyklusKodeType)
        {
            UnikIdType systemReference = GetOrganisationReference();
            RegistreringType1 registration = new RegistreringType1();

            registration.Tidspunkt = ou.Timestamp;
            registration.TidspunktSpecified = true;
            registration.LivscyklusKode = LivscyklusKodeType.Importeret;
            registration.LivscyklusKodeSpecified = true;
            registration.BrugerRef = systemReference;
            registration.NoteTekst = (ou.ParentOrgUnitUuid == null) ? "STSOrgSync" : null; // TODO: update according to AP26 once we know how to identify the root OU

            registration.AttributListe = new AttributListeType();
            registration.RelationListe = new RelationListeType();
            registration.TilstandListe = new TilstandListeType();

            return registration;
        }

        internal void AddProperties(String shortKey, String enhedsNavn, VirkningType virkning, RegistreringType1 registration)
        {
            EgenskabType property = new EgenskabType();
            property.BrugervendtNoegleTekst = shortKey;
            property.EnhedNavn = enhedsNavn;
            property.Virkning = virkning;

            EgenskabType[] egenskab = new EgenskabType[1];
            egenskab[0] = property;

            registration.AttributListe.Egenskab = egenskab;
        }

        internal AdresseFlerRelationType CreateAddressReference(string uuid, int indeks, string roleUuid, VirkningType virkning)
        {
            UnikIdType type = new UnikIdType();
            type.Item = UUIDConstants.ORGUNIT_ADDRESS_TYPE;
            type.ItemElementName = ItemChoiceType.UUIDIdentifikator;

            UnikIdType role = new UnikIdType();
            role.ItemElementName = ItemChoiceType.UUIDIdentifikator;
            role.Item = roleUuid;

            AdresseFlerRelationType address = new AdresseFlerRelationType();
            address.ReferenceID = StubUtil.GetReference<UnikIdType>(uuid, ItemChoiceType.UUIDIdentifikator);
            address.Virkning = virkning;
            address.Indeks = "" + indeks;
            address.Rolle = role;
            address.Type = type;

            return address;
        }

        internal void AddAddressReferences(List<AddressRelation> references, VirkningType virkning, RegistreringType1 registration)
        {
            if (references == null || references.Count == 0)
            {
                return;
            }

            var adresses = new AdresseFlerRelationType[references.Count];

            int referencesCount = references.Count;
            registration.RelationListe.Adresser = new AdresseFlerRelationType[referencesCount];

            for (int i = 0; i < referencesCount; i++)
            {
                AddressRelation addressRelation = references[i];

                switch (addressRelation.Type)
                {
                    case AddressRelationType.EMAIL:
                        AdresseFlerRelationType emailAddress = CreateAddressReference(addressRelation.Uuid, (i + 1), UUIDConstants.ADDRESS_ROLE_EMAIL, virkning);
                        registration.RelationListe.Adresser[i] = emailAddress;
                        break;
                    case AddressRelationType.PHONE:
                        AdresseFlerRelationType phoneAddress = CreateAddressReference(addressRelation.Uuid, (i + 1), UUIDConstants.ADDRESS_ROLE_PHONE, virkning);
                        registration.RelationListe.Adresser[i] = phoneAddress;
                        break;
                    case AddressRelationType.LOCATION:
                        AdresseFlerRelationType locationAddres = CreateAddressReference(addressRelation.Uuid, (i + 1), UUIDConstants.ADDRESS_ROLE_LOCATION, virkning);
                        registration.RelationListe.Adresser[i] = locationAddres;
                        break;
                    case AddressRelationType.LOSSHORTNAME:
                        AdresseFlerRelationType losAddress = CreateAddressReference(addressRelation.Uuid, (i + 1), UUIDConstants.ADDRESS_ROLE_LOSSHORTNAME, virkning);
                        registration.RelationListe.Adresser[i] = losAddress;
                        break;
                    case AddressRelationType.CONTACT_ADDRESS_OPEN_HOURS:
                        AdresseFlerRelationType contactOpenHoursAddress = CreateAddressReference(addressRelation.Uuid, (i + 1), UUIDConstants.ADDRESS_ROLE_CONTACT_ADDRESS_OPEN_HOURS, virkning);
                        registration.RelationListe.Adresser[i] = contactOpenHoursAddress;
                        break;
                    case AddressRelationType.EAN:
                        AdresseFlerRelationType eanAddress = CreateAddressReference(addressRelation.Uuid, (i + 1), UUIDConstants.ADDRESS_ROLE_EAN, virkning);
                        registration.RelationListe.Adresser[i] = eanAddress;
                        break;
                    case AddressRelationType.PHONE_OPEN_HOURS:
                        AdresseFlerRelationType phoneOpenHoursAddress = CreateAddressReference(addressRelation.Uuid, (i + 1), UUIDConstants.ADDRESS_ROLE_PHONE_OPEN_HOURS, virkning);
                        registration.RelationListe.Adresser[i] = phoneOpenHoursAddress;
                        break;
                    case AddressRelationType.POST:
                        AdresseFlerRelationType postAddress = CreateAddressReference(addressRelation.Uuid, (i + 1), UUIDConstants.ADDRESS_ROLE_POST, virkning);
                        registration.RelationListe.Adresser[i] = postAddress;
                        break;
                    case AddressRelationType.CONTACT_ADDRESS:
                        AdresseFlerRelationType contactAddress = CreateAddressReference(addressRelation.Uuid, (i + 1), UUIDConstants.ADDRESS_ROLE_CONTACT_ADDRESS, virkning);
                        registration.RelationListe.Adresser[i] = contactAddress;
                        break;
                    case AddressRelationType.EMAIL_REMARKS:
                        AdresseFlerRelationType emailRemarks = CreateAddressReference(addressRelation.Uuid, (i + 1), UUIDConstants.ADDRESS_ROLE_EMAIL_REMARKS, virkning);
                        registration.RelationListe.Adresser[i] = emailRemarks;
                        break;
                    case AddressRelationType.POST_RETURN:
                        AdresseFlerRelationType postReturn = CreateAddressReference(addressRelation.Uuid, (i + 1), UUIDConstants.ADDRESS_ROLE_POST_RETURN, virkning);
                        registration.RelationListe.Adresser[i] = postReturn;
                        break;
                    default:
                        throw new Exception("Cannot import OrganisationEnhed with addressRelationType = " + addressRelation.Type);
                }
            }
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

        internal OrganisationEnhedPortTypeClient CreatePort()
        {
            CustomLibBasBinding binding = new CustomLibBasBinding();

            // TODO: old STS tcode
            // OrganisationEnhedPortTypeClient port = new OrganisationEnhedPortTypeClient(binding, StubUtil.GetEndPointAddress(SERVICE));

            // TODO: new SP code (pre-token)
            OrganisationEnhedPortTypeClient port = new OrganisationEnhedPortTypeClient(binding, StubUtil.GetEndPointAddress("OrganisationEnhed/1"));
            port.ClientCredentials.ClientCertificate.SetCertificate(StoreLocation.LocalMachine, StoreName.My, X509FindType.FindByThumbprint, registryProperties.ClientCertThumbprint);

            // Disable revocation checking
            if (registryProperties.DisableRevocationCheck)
            {
                port.ClientCredentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
            }

            return port;
        }
    }
}
