using IntegrationLayer.Bruger;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.IO;
using System.Net;
using System.ServiceModel;

namespace Organisation.IntegrationLayer
{
    internal class BrugerStub
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private BrugerStubHelper helper = new BrugerStubHelper();
        private OrganisationRegistryProperties registry = OrganisationRegistryProperties.GetInstance();

        public void Importer(UserData user)
        {
            // create ShortKey if not supplied
            EnsureKeys(user);

            log.Debug("Attempting Import on Bruger with uuid " + user.Uuid);

            // create timestamp object to be used on all registrations, properties and relations
            VirkningType virkning = helper.GetVirkning(user.Timestamp);

            // setup registration
            RegistreringType1 registration = helper.CreateRegistration(user.Timestamp, LivscyklusKodeType.Importeret);

            // add properties
            helper.AddProperties(user.ShortKey, user.UserId, virkning, registration);

            // setup relations
            helper.AddAddressReferences(user.Addresses, virkning, registration);
            helper.AddPersonRelationship(user.PersonUuid, virkning, registration);
            helper.AddOrganisationRelation(StubUtil.GetMunicipalityOrganisationUUID(), virkning, registration);

            // set Tilstand to Active
            helper.SetTilstandToActive(virkning, registration, user.Timestamp);

            // wire everything together
            BrugerType brugerType = helper.GetBrugerType(user.Uuid, registration);
            ImportInputType importInput = new ImportInputType();
            importInput.Bruger = brugerType;

            // construct request
            importerRequest request = new importerRequest();
            request.BrugerImporterRequest = new BrugerImporterRequestType();
            request.BrugerImporterRequest.ImportInput = importInput;
            request.BrugerImporterRequest.AuthorityContext = new AuthorityContextType();
            request.BrugerImporterRequest.AuthorityContext.MunicipalityCVR = registry.Municipality;

            // send request
            SecurityToken token = TokenCache.IssueToken(BrugerStubHelper.SERVICE);
            BrugerPortType channel = StubUtil.CreateChannel<BrugerPortType>(BrugerStubHelper.SERVICE, "Importer", helper.CreatePort(), token);

            try
            {
                importerResponse response = channel.importer(request);
                int statusCode = Int32.Parse(response.BrugerImporterResponse.ImportOutput.StandardRetur.StatusKode);
                if (statusCode != 20)
                {
                    string message = StubUtil.ConstructSoapErrorMessage(statusCode, "Import", BrugerStubHelper.SERVICE, response.BrugerImporterResponse.ImportOutput.StandardRetur.FejlbeskedTekst);
                    log.Error(message);

                    throw new SoapServiceException(message);
                }

                log.Debug("Import on Bruger with uuid " + user.Uuid + " succeeded");
            }
            catch (Exception ex) when (ex is CommunicationException || ex is IOException || ex is TimeoutException || ex is WebException)
            {
                string message = "Failed to establish connection to the Importer service on Bruger";
                log.Error(message, ex );

                throw new ServiceNotFoundException(message, ex);
            }
        }

        // Deactivates the user by setting the Gyldighed attribute to Inactive
        public void Deactivate(string uuid, DateTime timestamp)
        {
            log.Debug("Attempting Deactivate on Bruger with uuid " + uuid);

            RegistreringType1 registration = GetLatestRegistration(uuid, false);
            if (registration == null)
            {
                log.Debug("Cannot Deactivate Bruger with uuid " + uuid + " because it does not exist in Organisation");
                return;
            }

            SecurityToken token = TokenCache.IssueToken(BrugerStubHelper.SERVICE);
            BrugerPortType channel = StubUtil.CreateChannel<BrugerPortType>(BrugerStubHelper.SERVICE, "Ret", helper.CreatePort(), token);

            try
            {
                RetInputType1 input = new RetInputType1();
                input.UUIDIdentifikator = uuid;
                input.AttributListe = registration.AttributListe;
                input.TilstandListe = registration.TilstandListe;
                input.RelationListe = registration.RelationListe;

                VirkningType virkning = helper.GetVirkning(timestamp);
                helper.SetTilstandToInactive(virkning, registration, timestamp);

                retRequest request = new retRequest();
                request.BrugerRetRequest = new BrugerRetRequestType();
                request.BrugerRetRequest.RetInput = input;
                request.BrugerRetRequest.AuthorityContext = new AuthorityContextType();
                request.BrugerRetRequest.AuthorityContext.MunicipalityCVR = registry.Municipality;

                retResponse response = channel.ret(request);

                int statusCode = Int32.Parse(response.BrugerRetResponse.RetOutput.StandardRetur.StatusKode);
                if (statusCode != 20)
                {
                    string message = StubUtil.ConstructSoapErrorMessage(statusCode, "Ret", BrugerStubHelper.SERVICE, response.BrugerRetResponse.RetOutput.StandardRetur.FejlbeskedTekst);
                    log.Error(message);
                    throw new SoapServiceException(message);
                }

                log.Debug("Deactivate on Bruger with uuid " +  uuid + " succeeded");
            }
            catch (Exception ex) when (ex is CommunicationException || ex is IOException || ex is TimeoutException || ex is WebException)
            {
                string message = "Failed to establish connection to the Ret service on Bruger";
                log.Error(message, ex);
                throw new ServiceNotFoundException(message, ex);
            }
        }

        public void Ret(UserData user)
        {
            log.Debug("Attempting Ret on Bruger with uuid " + user.Uuid);

            RegistreringType1 registration = GetLatestRegistration(user.Uuid, false);
            if (registration == null)
            {
                log.Debug("Cannot call Ret on Bruger with uuid " + user.Uuid + " because it does not exist in Organisation");
                return;
            }

            VirkningType virkning = helper.GetVirkning(user.Timestamp);

            SecurityToken token = TokenCache.IssueToken(BrugerStubHelper.SERVICE);
            BrugerPortType channel = StubUtil.CreateChannel<BrugerPortType>(BrugerStubHelper.SERVICE, "Ret", helper.CreatePort(), token);

            try
            {
                bool changes = false;

                RetInputType1 input = new RetInputType1();
                input.UUIDIdentifikator = user.Uuid;
                input.AttributListe = registration.AttributListe;
                input.TilstandListe = registration.TilstandListe;
                input.RelationListe = registration.RelationListe;

                // set Tilstand to Active (idempotent change - but it ensures that a previously deactived object is re-activated)
                changes = helper.SetTilstandToActive(virkning, registration, user.Timestamp) | changes;

                #region Update attributes

                // compare latest property to the local object
                EgenskabType latestProperty = StubUtil.GetLatestProperty(input.AttributListe.Egenskab);
                if (latestProperty == null || !latestProperty.BrugerNavn.Equals(user.UserId) || (user.ShortKey != null && !latestProperty.BrugervendtNoegleTekst.Equals(user.ShortKey)))
                {
                    // end the validity of open-ended property
                    if (latestProperty != null)
                    {
                        StubUtil.TerminateVirkning(latestProperty.Virkning, user.Timestamp);
                    }
                    else
                    {
                        // create ShortKey if not supplied
                        EnsureKeys(user);
                    }

                    // create a new property
                    EgenskabType newProperty = new EgenskabType();
                    newProperty.Virkning = helper.GetVirkning(user.Timestamp);
                    newProperty.BrugervendtNoegleTekst = ((user.ShortKey != null) ? user.ShortKey : latestProperty.BrugervendtNoegleTekst);
                    newProperty.BrugerNavn = user.UserId;

                    // create a new set of properties
                    EgenskabType[] oldProperties = input.AttributListe.Egenskab;
                    input.AttributListe.Egenskab = new EgenskabType[oldProperties.Length + 1];
                    for (int i = 0; i < oldProperties.Length; i++)
                    {
                        input.AttributListe.Egenskab[i] = oldProperties[i];
                    }
                    input.AttributListe.Egenskab[oldProperties.Length] = newProperty;

                    changes = true;
                }
                #endregion

                #region Update address relationships
                // terminate the Virkning on all address relationships that no longer exists locally
                changes = StubUtil.TerminateObjectsInOrgNoLongerPresentLocally(input.RelationListe.Adresser, user.Addresses, user.Timestamp, true) || changes;

                // add references to address objects that are new
                List<string> uuidsToAdd = StubUtil.FindAllObjectsInLocalNotInOrg(input.RelationListe.Adresser, user.Addresses, true);

                if (uuidsToAdd.Count > 0)
                {
                    int size = uuidsToAdd.Count + ((input.RelationListe.Adresser != null) ? input.RelationListe.Adresser.Length : 0);
                    AdresseFlerRelationType[] newAdresser = new AdresseFlerRelationType[size];

                    int i = 0;
                    if (input.RelationListe.Adresser != null)
                    {
                        foreach (var addressInOrg in input.RelationListe.Adresser)
                        {
                            newAdresser[i++] = addressInOrg;
                        }
                    }

                    foreach (string uuidToAdd in uuidsToAdd)
                    {
                        foreach (var addressInLocal in user.Addresses)
                        {
                            if (addressInLocal.Uuid.Equals(uuidToAdd))
                            {
                                string roleUuid = null;
                                switch (addressInLocal.Type)
                                {
                                    case AddressRelationType.EMAIL:
                                        roleUuid = UUIDConstants.ADDRESS_ROLE_EMAIL;
                                        break;
                                    case AddressRelationType.PHONE:
                                        roleUuid = UUIDConstants.ADDRESS_ROLE_PHONE;
                                        break;
                                    case AddressRelationType.LOCATION:
                                        roleUuid = UUIDConstants.ADDRESS_ROLE_LOCATION;
                                        break;
                                    default:
                                        log.Warn("Cannot add relationship to address of type " + addressInLocal.Type + " with uuid " + addressInLocal.Uuid + " as the type is unknown");
                                        continue;
                                }

                                AdresseFlerRelationType newAddress = helper.CreateAddressReference(uuidToAdd, (i + 1), roleUuid, virkning);
                                newAdresser[i++] = newAddress;
                            }
                        }
                    }
                    input.RelationListe.Adresser = newAdresser;
                    changes = true;
                }
                #endregion

                #region Update organisation relationship
                if (registration.RelationListe.Tilhoerer != null)
                {
                    // make sure that the pointer is set correctly
                    if (!StubUtil.GetMunicipalityOrganisationUUID().Equals(registration.RelationListe.Tilhoerer.ReferenceID.Item))
                    {
                        registration.RelationListe.Tilhoerer.ReferenceID.Item = StubUtil.GetMunicipalityOrganisationUUID();
                        changes = true;
                    }

                    // update the Virkning on the Tilhører relationship if needed (undelete feature)
                    object endTime = registration.RelationListe.Tilhoerer.Virkning.TilTidspunkt.Item;
                    if (endTime is DateTime) // if it is a DateTime, it means that it has been terminated, otherwise it would be a boolean
                    {
                        log.Debug("Re-establishing relationship with Organisation for Bruger " + user.Uuid);
                        registration.RelationListe.Tilhoerer.Virkning = virkning;
                        changes = true;
                    }
                }
                else
                {
                    // no existing relationship (should actually never happen, but let us just take care of it)
                    helper.AddOrganisationRelation(StubUtil.GetMunicipalityOrganisationUUID(), virkning, registration);
                    changes = true;
                }
                #endregion

                #region Update person relationship
                PersonFlerRelationType existingPerson = BrugerStubHelper.GetLatestPersonFlerRelationType(registration.RelationListe.TilknyttedePersoner);
                if (existingPerson != null)
                {
                    // It really shouldn't happen that often that the Person reference changes on a User, but we support it nonetheless
                    if (!existingPerson.ReferenceID.Item.Equals(user.PersonUuid))
                    {
                        // terminiate existing relationship, and add a new one
                        StubUtil.TerminateVirkning(existingPerson.Virkning, user.Timestamp);

                        // create a new person relation
                        PersonFlerRelationType newPerson = new PersonFlerRelationType();
                        newPerson.Virkning = helper.GetVirkning(user.Timestamp);
                        newPerson.ReferenceID = StubUtil.GetReference<UnikIdType>(user.PersonUuid, ItemChoiceType.UUIDIdentifikator);

                        // create a new set of person references, containing the new user
                        PersonFlerRelationType[] oldPersons = registration.RelationListe.TilknyttedePersoner;
                        input.RelationListe.TilknyttedePersoner = new PersonFlerRelationType[oldPersons.Length + 1];
                        for (int i = 0; i < oldPersons.Length; i++)
                        {
                            input.RelationListe.TilknyttedePersoner[i] = oldPersons[i];
                        }
                        input.RelationListe.TilknyttedePersoner[oldPersons.Length] = newPerson;

                        changes = true;

                    }
                }
                else
                {
                    // This really shouldn't have happened, as it means we have an existing User without a Person attached - but
                    // we will just fix it, and create the reference on this user

                    log.Warn("Ret on Bruge with uuid " + user.Uuid + " encountered a registration with NO relationship to a Person - fixing it!");
                    helper.AddPersonRelationship(user.PersonUuid, virkning, registration);
                    changes = true;
                }
                #endregion

                // if no changes are made, we do not call the service
                if (!changes)
                {
                    log.Debug("Ret on Bruger with uuid " + user.Uuid + " cancelled because of no changes");
                    return;
                }

                // send Ret request
                retRequest request = new retRequest();
                request.BrugerRetRequest = new BrugerRetRequestType();
                request.BrugerRetRequest.RetInput = input;
                request.BrugerRetRequest.AuthorityContext = new AuthorityContextType();
                request.BrugerRetRequest.AuthorityContext.MunicipalityCVR = registry.Municipality;

                retResponse response = channel.ret(request);

                int statusCode = Int32.Parse(response.BrugerRetResponse.RetOutput.StandardRetur.StatusKode);
                if (statusCode != 20)
                {
                    string message = StubUtil.ConstructSoapErrorMessage(statusCode, "Ret", BrugerStubHelper.SERVICE, response.BrugerRetResponse.RetOutput.StandardRetur.FejlbeskedTekst);
                    log.Error(message);
                    throw new SoapServiceException(message);
                }

                log.Debug("Ret succesful on Bruger with uuid " + user.Uuid);
            }
            catch (Exception ex) when (ex is CommunicationException || ex is IOException || ex is TimeoutException || ex is WebException)
            {
                string message = "Failed to establish connection to the Ret service on Bruger";
                log.Error(message, ex);
                throw new ServiceNotFoundException(message, ex);
            }
        }

        public List<string> Soeg()
        {
            SecurityToken token = TokenCache.IssueToken(BrugerStubHelper.SERVICE);
            BrugerPortType channel = StubUtil.CreateChannel<BrugerPortType>(BrugerStubHelper.SERVICE, "Soeg", helper.CreatePort(), token);

            SoegInputType1 soegInput = new SoegInputType1();
            soegInput.AttributListe = new AttributListeType();
            soegInput.RelationListe = new RelationListeType();
            soegInput.SoegRegistrering = new SoegRegistreringType();
            soegInput.SoegVirkning = new SoegVirkningType();
            soegInput.TilstandListe = new TilstandListeType();
            soegInput.MaksimalAntalKvantitet = "5000"; // the default limit is 500, and for the report tool, we need to extract ALL OUs/Users, which can be a higher number

            // TODO: This is not working because KMDs implementation does not take Virkning into consideration when searching. We are waiting for a fix from KMD
            // only search in "actual state"
            soegInput.SoegVirkning.FraTidspunkt = new TidspunktType();
            soegInput.SoegVirkning.FraTidspunkt.Item = DateTime.Now;

            // only search for Active users
            // TODO: It shouldn't be required to add a Virkning to the individual fields in search(), but we get an errorcode 40 (bad request) if we leave it out (another bug @KMD I think)
            // TODO: This code causes the KMD services to throw an SQL Exception - this has been reported to KMD, and we are waiting for a fix. This means that we get inactive users back when searching
            /*
            soegInput.TilstandListe.Gyldighed = new GyldighedType[1];
            soegInput.TilstandListe.Gyldighed[0] = new GyldighedType();
            soegInput.TilstandListe.Gyldighed[0].GyldighedStatusKode = GyldighedStatusKodeType.Aktiv;
            soegInput.TilstandListe.Gyldighed[0].Virkning = new VirkningType();
            soegInput.TilstandListe.Gyldighed[0].Virkning.FraTidspunkt = new TidspunktType();
            soegInput.TilstandListe.Gyldighed[0].Virkning.FraTidspunkt.Item = DateTime.Now;
            */

            // only return objects that have a Tilhører relationship top-level Organisation
            UnikIdType orgReference = StubUtil.GetReference<UnikIdType>(registry.MunicipalityOrganisationUUID, ItemChoiceType.UUIDIdentifikator);
            OrganisationRelationType organisationRelationType = new OrganisationRelationType();
            organisationRelationType.ReferenceID = orgReference;
            soegInput.RelationListe.Tilhoerer = organisationRelationType;

            // search
            soegRequest request = new soegRequest();
            request.BrugerSoegRequest = new BrugerSoegRequestType();
            request.BrugerSoegRequest.SoegInput = soegInput;
            request.BrugerSoegRequest.AuthorityContext = new AuthorityContextType();
            request.BrugerSoegRequest.AuthorityContext.MunicipalityCVR = registry.Municipality;

            try
            {
                soegResponse response = channel.soeg(request);
                int statusCode = Int32.Parse(response.BrugerSoegResponse.SoegOutput.StandardRetur.StatusKode);
                if (statusCode != 20 && statusCode != 44) // 44 is empty search result
                {
                    string message = StubUtil.ConstructSoapErrorMessage(statusCode, "Soeg", OrganisationFunktionStubHelper.SERVICE, response.BrugerSoegResponse.SoegOutput.StandardRetur.FejlbeskedTekst);
                    log.Error(message);
                    throw new SoapServiceException(message);
                }

                List<string> functions = new List<string>();
                if (statusCode == 20)
                {
                    foreach (string id in response.BrugerSoegResponse.SoegOutput.IdListe)
                    {
                        functions.Add(id);
                    }
                }

                return functions;
            }
            catch (Exception ex) when (ex is CommunicationException || ex is IOException || ex is TimeoutException || ex is WebException)
            {
                string message = "Failed to establish connection to the Soeg service on Bruger";
                log.Error(message, ex);
                throw new ServiceNotFoundException(message, ex);
            }
        }

        // TODO: this method contains a work-around.
        //       KMD released version 1.4 of Organisation, containing a series of changes to how reading and searching for data works, unfortunately this new functionality
        //       does not cover the actually required use-cases (e.g. reading actual state), but luckily they forgot to change the List() operation, so we are using List()
        //       instead of Laes() until Laes() is fixed in a later release of Organisation.
        public RegistreringType1 GetLatestRegistration(string uuid, bool actualStateOnly)
        {
            ListInputType listInput = new ListInputType();
            listInput.UUIDIdentifikator = new string[] { uuid };

            // this ensures that we get the full history when reading, and not just what is valid right now
            if (!actualStateOnly)
            {
                listInput.VirkningFraFilter = new TidspunktType();
                listInput.VirkningFraFilter.Item = true;
                listInput.VirkningTilFilter = new TidspunktType();
                listInput.VirkningTilFilter.Item = true;
            }

            listRequest request = new listRequest();
            request.BrugerListeRequest = new BrugerListeRequestType();
            request.BrugerListeRequest.ListInput = listInput;
            request.BrugerListeRequest.AuthorityContext = new AuthorityContextType();
            request.BrugerListeRequest.AuthorityContext.MunicipalityCVR = registry.Municipality;

            SecurityToken token = TokenCache.IssueToken(BrugerStubHelper.SERVICE);
            BrugerPortType channel = StubUtil.CreateChannel<BrugerPortType>(BrugerStubHelper.SERVICE, "Laes", helper.CreatePort(), token);

            try
            {
                listResponse response = channel.list(request);

                int statusCode = Int32.Parse(response.BrugerListeResponse.ListOutput.StandardRetur.StatusKode);
                if (statusCode != 20)
                {
                    // note that statusCode 44 means that the object does not exists, so that is a valid response
                    log.Debug("Lookup on Bruger with uuid '" + uuid + "' failed with statuscode " + statusCode);
                    return null;
                }

                if (response.BrugerListeResponse.ListOutput.FiltreretOejebliksbillede.Length != 1)
                {
                    log.Warn("Lookup Bruger with uuid '" + uuid + "' returned multiple objects");
                    return null;
                }

                RegistreringType1[] resultSet = response.BrugerListeResponse.ListOutput.FiltreretOejebliksbillede[0].Registrering;
                if (resultSet.Length == 0)
                {
                    log.Warn("Bruger with uuid '" + uuid + "' exists, but has no registration");
                    return null;
                }

                RegistreringType1 result = null;
                if (resultSet.Length > 1)
                {
                    log.Warn("Bruger with uuid " + uuid + " has more than one registration when reading latest registration, this should never happen");

                    DateTime winner = DateTime.MinValue;
                    foreach (RegistreringType1 res in resultSet)
                    {
                        // first time through will always result in a True evaluation here
                        if (DateTime.Compare(winner, res.Tidspunkt) < 0)
                        {
                            result = res;
                            winner = res.Tidspunkt;
                        }
                    }
                }
                else
                {
                    result = resultSet[0];
                }

                return result;
            }
            catch (Exception ex) when (ex is CommunicationException || ex is IOException || ex is TimeoutException || ex is WebException)
            {
                string message = "Failed to establish connection to the Laes service on Bruger";
                log.Error(message, ex);
                throw new ServiceNotFoundException(message, ex);
            }
        }

        private void EnsureKeys(UserData bruger)
        {
            bruger.ShortKey = (bruger.ShortKey != null) ? bruger.ShortKey : IdUtil.GenerateShortKey();
        }
    }
}
