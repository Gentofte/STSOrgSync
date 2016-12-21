using IntegrationLayer.OrganisationFunktion;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.IO;
using System.Net;
using System.ServiceModel;

namespace Organisation.IntegrationLayer
{
    internal class OrganisationFunktionStub
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private OrganisationFunktionStubHelper helper = new OrganisationFunktionStubHelper();
        private OrganisationRegistryProperties registry = OrganisationRegistryProperties.GetInstance();

        public void Importer(OrgFunctionData orgFunction)
        {
            // create ShortKey and Uuid if not supplied
            EnsureKeys(orgFunction);

            log.Debug("Attempting Import on OrgFunction with uuid " + orgFunction.Uuid);

            // create timestamp object to be used on all registrations, properties and relations
            VirkningType virkning = helper.GetVirkning(orgFunction.Timestamp);

            // setup registration
            RegistreringType1 registration = helper.CreateRegistration(orgFunction.Timestamp, LivscyklusKodeType.Importeret);

            // add properties
            helper.AddProperties(orgFunction.ShortKey, orgFunction.Name, virkning, registration);

            // add relationships on registration
            helper.AddTilknyttedeBrugere(orgFunction.Users, virkning, registration);
            helper.AddTilknyttedeEnheder(orgFunction.OrgUnits, virkning, registration);
            helper.AddTilknyttedeItSystemer(orgFunction.ItSystems, virkning, registration);
            helper.AddOrganisationRelation(StubUtil.GetMunicipalityOrganisationUUID(), virkning, registration);
            helper.AddAddressReferences(orgFunction.Addresses, virkning, registration);
            helper.SetFunktionsType(orgFunction.FunctionTypeUuid, virkning, registration);

            // set Tilstand to Active
            helper.SetTilstandToActive(virkning, registration);

            // wire everything together
            OrganisationFunktionType organisationFunktionType = helper.GetOrganisationFunktionType(orgFunction.Uuid, registration);
            ImportInputType importInput = new ImportInputType();
            importInput.OrganisationFunktion = organisationFunktionType;

            // construct request
            importerRequest request = new importerRequest();
            request.OrganisationFunktionImporterRequest = new OrganisationFunktionImporterRequestType();
            request.OrganisationFunktionImporterRequest.ImportInput = importInput;
            request.OrganisationFunktionImporterRequest.AuthorityContext = new AuthorityContextType();
            request.OrganisationFunktionImporterRequest.AuthorityContext.MunicipalityCVR = registry.Municipality;

            // send request
            SecurityToken token = TokenCache.IssueToken(OrganisationFunktionStubHelper.SERVICE);
            OrganisationFunktionPortType channel = StubUtil.CreateChannel<OrganisationFunktionPortType>(OrganisationFunktionStubHelper.SERVICE, "Import", helper.CreatePort(), token);

            try
            {
                importerResponse result = channel.importer(request);

                int statusCode = Int32.Parse(result.OrganisationFunktionImporterResponse.ImportOutput.StandardRetur.StatusKode);
                if (statusCode != 20)
                {
                    string message = StubUtil.ConstructSoapErrorMessage(statusCode, "Import", OrganisationFunktionStubHelper.SERVICE, result.OrganisationFunktionImporterResponse.ImportOutput.StandardRetur.FejlbeskedTekst);
                    log.Error(message);
                    throw new SoapServiceException(message);
                }

                log.Debug("Import successful on OrgFunction with uuid " + orgFunction.Uuid);
            }
            catch (Exception ex) when (ex is CommunicationException || ex is IOException || ex is TimeoutException || ex is WebException)
            {
                string message = "Failed to establish connection to the Importer service on OrganisationFunktion";
                log.Error(message, ex);
                throw new ServiceNotFoundException(message, ex);
            }
        }

        public void Ret(OrgFunctionData orgFunction, UpdateIndicator userIndicator, UpdateIndicator unitIndicator)
        {
            log.Debug("Attempting Ret on OrganisationFunction with uuid " + orgFunction.Uuid);

            RegistreringType1 registration = GetLatestRegistration(orgFunction.Uuid, false);
            if (registration == null)
            {
                log.Debug("Cannot call Ret on OrganisationFunktion with uuid " + orgFunction.Uuid + " because it does not exist in Organisation");
                return;
            }

            VirkningType virkning = helper.GetVirkning(orgFunction.Timestamp);

            SecurityToken token = TokenCache.IssueToken(OrganisationFunktionStubHelper.SERVICE);
            OrganisationFunktionPortType channel = StubUtil.CreateChannel<OrganisationFunktionPortType>(OrganisationFunktionStubHelper.SERVICE, "Ret", helper.CreatePort(), token);

            try
            {
                bool changes = false;

                RetInputType1 input = new RetInputType1();
                input.UUIDIdentifikator = orgFunction.Uuid;
                input.AttributListe = registration.AttributListe;
                input.TilstandListe = registration.TilstandListe;
                input.RelationListe = registration.RelationListe;

                #region Update attributes if needed
                EgenskabType latestProperty = StubUtil.GetLatestProperty(input.AttributListe.Egenskab);
                if (latestProperty == null || (orgFunction.Name != null && !latestProperty.FunktionNavn.Equals(orgFunction.Name)) || (orgFunction.ShortKey != null && !latestProperty.BrugervendtNoegleTekst.Equals(orgFunction.ShortKey)))
                {
                    // end the validity of open-ended property
                    if (latestProperty != null)
                    {
                        StubUtil.TerminateVirkning(latestProperty.Virkning, orgFunction.Timestamp);
                    }
                    else
                    {
                        orgFunction.ShortKey = (orgFunction.ShortKey != null) ? orgFunction.ShortKey : IdUtil.GenerateShortKey();
                        orgFunction.Name = (orgFunction.Name != null) ? orgFunction.Name : "Unknown Function"; // special case where editing a function that has been orphaned, without supplying a name - should never really happen, but the API allows it
                    }

                    // create a new property
                    EgenskabType newProperty = new EgenskabType();
                    newProperty.Virkning = helper.GetVirkning(orgFunction.Timestamp);
                    newProperty.BrugervendtNoegleTekst = ((orgFunction.ShortKey != null) ? orgFunction.ShortKey : latestProperty.BrugervendtNoegleTekst);
                    newProperty.FunktionNavn = ((orgFunction.Name != null) ? orgFunction.Name : latestProperty.FunktionNavn);

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

                #region Update TilknyttedeBrugere relationships
                // terminate references
                if (userIndicator.Equals(UpdateIndicator.COMPARE))
                {
                    // terminate the references in Org that no longer exist locally
                    changes = StubUtil.TerminateObjectsInOrgNoLongerPresentLocally(input.RelationListe.TilknyttedeBrugere, orgFunction.Users, orgFunction.Timestamp, false) || changes;
                }
                else if (userIndicator.Equals(UpdateIndicator.REMOVE))
                {
                    changes = TerminateObjectsInOrgThatAreInLocal(input.RelationListe.TilknyttedeBrugere, orgFunction.Users, orgFunction.Timestamp) || changes;
                }

                if (userIndicator.Equals(UpdateIndicator.COMPARE) || userIndicator.Equals(UpdateIndicator.ADD))
                {
                    // get the set of new local objects only
                    List<string> uuidsToAdd = StubUtil.FindAllObjectsInLocalNotInOrg(input.RelationListe.TilknyttedeBrugere, orgFunction.Users, false);

                    // add all the new references
                    if (uuidsToAdd.Count > 0)
                    {
                        int size = uuidsToAdd.Count + ((input.RelationListe.TilknyttedeBrugere != null) ? input.RelationListe.TilknyttedeBrugere.Length : 0);
                        BrugerFlerRelationType[] newUsers = new BrugerFlerRelationType[size];

                        int i = 0;
                        if (input.RelationListe.TilknyttedeBrugere != null)
                        {
                            foreach (var usersInOrg in input.RelationListe.TilknyttedeBrugere)
                            {
                                newUsers[i++] = usersInOrg;
                            }
                        }

                        foreach (string uuidToAdd in uuidsToAdd)
                        {
                            newUsers[i++] = helper.CreateBrugerRelation(uuidToAdd, virkning);
                        }

                        input.RelationListe.TilknyttedeBrugere = newUsers;
                        changes = true;
                    }
                }
                #endregion

                #region Update TilknyttedeEnheder relationships
                // terminate references
                if (unitIndicator.Equals(UpdateIndicator.COMPARE))
                {
                    // terminate the references in Org that no longer exist locally
                    changes = StubUtil.TerminateObjectsInOrgNoLongerPresentLocally(input.RelationListe.TilknyttedeEnheder, orgFunction.OrgUnits, orgFunction.Timestamp, false) || changes;
                }
                else if (unitIndicator.Equals(UpdateIndicator.REMOVE))
                {
                    changes = TerminateObjectsInOrgThatAreInLocal(input.RelationListe.TilknyttedeEnheder, orgFunction.OrgUnits, orgFunction.Timestamp) || changes;
                }

                if (unitIndicator.Equals(UpdateIndicator.COMPARE) || unitIndicator.Equals(UpdateIndicator.ADD))
                {
                    // get the set of new local objects
                    List<string> uuidsToAdd = StubUtil.FindAllObjectsInLocalNotInOrg(input.RelationListe.TilknyttedeEnheder, orgFunction.OrgUnits, false);

                    // add all the new references
                    if (uuidsToAdd.Count > 0)
                    {
                        int size = uuidsToAdd.Count + ((input.RelationListe.TilknyttedeEnheder != null) ? input.RelationListe.TilknyttedeEnheder.Length : 0);
                        OrganisationEnhedFlerRelationType[] newUnits = new OrganisationEnhedFlerRelationType[size];

                        int i = 0;
                        if (input.RelationListe.TilknyttedeEnheder != null)
                        {
                            foreach (var unit in input.RelationListe.TilknyttedeEnheder)
                            {
                                newUnits[i++] = unit;
                            }
                        }

                        foreach (string uuidToAdd in uuidsToAdd)
                        {
                            newUnits[i++] = helper.CreateOrgEnhedRelation(uuidToAdd, virkning);
                        }

                        input.RelationListe.TilknyttedeEnheder = newUnits;
                        changes = true;
                    }
                }
                #endregion

                #region Update organisation relationship
                bool foundExistingValidOrganisationRelation = false;
                if (registration.RelationListe.TilknyttedeOrganisationer != null && registration.RelationListe.TilknyttedeOrganisationer.Length > 0)
                {
                    foreach (OrganisationFlerRelationType orgRelation in registration.RelationListe.TilknyttedeOrganisationer)
                    {
                        // make sure that the pointer is set correctly
                        if (!StubUtil.GetMunicipalityOrganisationUUID().Equals(orgRelation.ReferenceID.Item))
                        {
                            orgRelation.ReferenceID.Item = StubUtil.GetMunicipalityOrganisationUUID();
                            changes = true;
                        }

                        // update the Virkning on the TilknyttedeOrganisationer relationship if needed (undelete feature)
                        object endTime = orgRelation.Virkning.TilTidspunkt.Item;
                        if (!(endTime is DateTime)) // if it is not a DateTime, the relationship is still valid (as we never teminate into the future)
                        {
                            foundExistingValidOrganisationRelation = true;
                        }
                    }
                }

                if (!foundExistingValidOrganisationRelation)
                {
                    helper.AddOrganisationRelation(StubUtil.GetMunicipalityOrganisationUUID(), virkning, registration);
                    changes = true;
                }
                #endregion

                #region Update Address relationships
                // terminate the Virkning on all address relationships that no longer exists locally
                changes = StubUtil.TerminateObjectsInOrgNoLongerPresentLocally(input.RelationListe.Adresser, orgFunction.Addresses, orgFunction.Timestamp, true) || changes;

                // add references to address objects that are new
                List<string> addressUuidsToAdd = StubUtil.FindAllObjectsInLocalNotInOrg(input.RelationListe.Adresser, orgFunction.Addresses, true);

                if (addressUuidsToAdd.Count > 0)
                {
                    int size = addressUuidsToAdd.Count + ((input.RelationListe.Adresser != null) ? input.RelationListe.Adresser.Length : 0);
                    AdresseFlerRelationType[] newAdresser = new AdresseFlerRelationType[size];

                    int i = 0;
                    if (input.RelationListe.Adresser != null)
                    {
                        foreach (var addressInOrg in input.RelationListe.Adresser)
                        {
                            newAdresser[i++] = addressInOrg;
                        }
                    }

                    foreach (string uuidToAdd in addressUuidsToAdd)
                    {
                        foreach (var addressInLocal in orgFunction.Addresses)
                        {
                            if (addressInLocal.Uuid.Equals(uuidToAdd))
                            {
                                string roleUuid = null;
                                switch (addressInLocal.Type)
                                {
                                    case AddressRelationType.URL:
                                        roleUuid = UUIDConstants.ADDRESS_ROLE_URL;
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

                // if no changes are made, we do not call the service
                if (!changes)
                {
                    log.Debug("Ret on OrganisationFunktion with uuid " + orgFunction.Uuid + " cancelled because of no changes");
                    return;
                }

                // send Ret request
                retRequest request = new retRequest();
                request.OrganisationFunktionRetRequest = new OrganisationFunktionRetRequestType();
                request.OrganisationFunktionRetRequest.RetInput = input;
                request.OrganisationFunktionRetRequest.AuthorityContext = new AuthorityContextType();
                request.OrganisationFunktionRetRequest.AuthorityContext.MunicipalityCVR = registry.Municipality;

                retResponse response = channel.ret(request);

                int statusCode = Int32.Parse(response.OrganisationFunktionRetResponse.RetOutput.StandardRetur.StatusKode);
                if (statusCode != 20)
                {
                    string message = StubUtil.ConstructSoapErrorMessage(statusCode, "Ret", OrganisationFunktionStubHelper.SERVICE, response.OrganisationFunktionRetResponse.RetOutput.StandardRetur.FejlbeskedTekst);
                    log.Error(message);
                    throw new SoapServiceException(message);
                }

                log.Debug("Ret succesful on OrganisationFunktion with uuid " + orgFunction.Uuid);
            }
            catch (Exception ex) when (ex is CommunicationException || ex is IOException || ex is TimeoutException || ex is WebException)
            {
                string message = "Failed to establish connection to the Ret service on OrganisationFunktion";
                log.Error(message, ex);
                throw new ServiceNotFoundException(message, ex);
            }
        }

        // TODO: This is a work-around that "fixes" the bug in the KMD implementation, by first searching, and then reading each
        //       individual object to check if they matches the criteria, filtering away those that do not
        //
        //       This method returns the full object (might as well, since we just read them, could be useful for saving a call later
        public List<FiltreretOejebliksbilledeType> SoegAndGetLatestRegistration(string functionsTypeUuid, string userUuid, string unitUuid, string itSystemUuid)
        {
            List<FiltreretOejebliksbilledeType> result = new List<FiltreretOejebliksbilledeType>();

            // perform a search and then retrieve all the objects that matches the search criteria
            List<string> uuidCandidates = Soeg(functionsTypeUuid, userUuid, unitUuid, itSystemUuid);
            if (uuidCandidates == null || uuidCandidates.Count == 0)
            {
                return result;
            }

            FiltreretOejebliksbilledeType[] resultCandidates = GetLatestRegistrations(uuidCandidates.ToArray(), true); // searching is always done on ActualState
            if (resultCandidates == null || resultCandidates.Length == 0)
            {
                return result;
            }

            // check that each result from the search actually matches the search criteria (because KMDs implementation doesn't do it corectly :( )
            foreach (FiltreretOejebliksbilledeType resultCandidate in resultCandidates)
            {
                if (resultCandidate.Registrering == null || resultCandidate.Registrering.Length == 0)
                {
                    log.Warn("Result candidate with uuid " + resultCandidate.ObjektType.UUIDIdentifikator + " does not have a registration - it is skipped in the result");
                    continue;
                }

                if (resultCandidate.Registrering.Length > 1)
                {
                    log.Warn("Result candidate with uuid " + resultCandidate.ObjektType.UUIDIdentifikator + " has more than one registration - it is skipped in the result");
                    continue;
                }

                RegistreringType1 registration = resultCandidate.Registrering[0];

                if (userUuid != null)
                {
                    bool found = false;

                    if (registration.RelationListe.TilknyttedeBrugere != null)
                    {
                        foreach (var userRelation in registration.RelationListe.TilknyttedeBrugere)
                        {
                            if (userUuid.Equals(userRelation.ReferenceID?.Item))
                            {
                                found = true;
                            }

                        }
                    }

                    if (!found)
                    {
                        log.Debug("Filtering OrgFunction with uuid " + resultCandidate.ObjektType.UUIDIdentifikator + " because it does not have a correct user relation");
                        continue;
                    }
                }

                if (unitUuid != null)
                {
                    bool found = false;

                    if (registration.RelationListe.TilknyttedeEnheder != null)
                    {
                        foreach (var unitRelation in registration.RelationListe.TilknyttedeEnheder)
                        {
                            if (unitUuid.Equals(unitRelation.ReferenceID?.Item))
                            {
                                found = true;
                            }

                        }
                    }

                    if (!found)
                    {
                        log.Debug("Filtering OrgFunction with uuid " + resultCandidate.ObjektType.UUIDIdentifikator + " because it does not have a correct unit relation");
                        continue;
                    }
                }

                if (itSystemUuid != null)
                {
                    bool found = false;

                    if (registration.RelationListe.TilknyttedeItSystemer != null)
                    {
                        foreach (var itSystemRelation in registration.RelationListe.TilknyttedeItSystemer)
                        {
                            if (itSystemUuid.Equals(itSystemRelation.ReferenceID?.Item))
                            {
                                found = true;
                            }

                        }
                    }

                    if (!found)
                    {
                        log.Debug("Filtering OrgFunction with uuid " + resultCandidate.ObjektType.UUIDIdentifikator + " because it does not have a correct itSystem relation");
                        continue;
                    }
                }

                result.Add(resultCandidate);
            }

            return result;
        }

        // TODO: This is a work-around that "fixes" the bug in the KMD implementation, by first searching, and then reading each
        //       individual object to check if they matches the criteria, filtering away those that do not
        public List<string> SoegAndGetUuids(string functionsTypeUuid, string userUuid, string unitUuid, string itSystemUuid)
        {
            List<string> uuidResult = new List<string>();

            List<FiltreretOejebliksbilledeType> result = SoegAndGetLatestRegistration(functionsTypeUuid, userUuid, unitUuid, itSystemUuid);
            foreach (FiltreretOejebliksbilledeType reg in result)
            {
                uuidResult.Add(reg.ObjektType.UUIDIdentifikator);
            }

            return uuidResult;
        }

        // TODO: KMDs implementation of Soeg() is broken, so we will get false positives in the result from time to time, which we should handle carefully
        // TODO: I've made the method private, so it cannot be called outside this class - instead use the filtered versions above (they remove the wrong results from Soeg())
        private List<string> Soeg(string functionsTypeUuid, string userUuid, string unitUuid, string itSystemUuid)
        {
            SecurityToken token = TokenCache.IssueToken(OrganisationFunktionStubHelper.SERVICE);
            OrganisationFunktionPortType channel = StubUtil.CreateChannel<OrganisationFunktionPortType>(OrganisationFunktionStubHelper.SERVICE, "Soeg", helper.CreatePort(), token);

            SoegInputType1 soegInput = new SoegInputType1();
            soegInput.AttributListe = new AttributListeType();
            soegInput.RelationListe = new RelationListeType();
            soegInput.SoegRegistrering = new SoegRegistreringType();
            soegInput.SoegVirkning = new SoegVirkningType();
            soegInput.TilstandListe = new TilstandListeType();

            // TODO: This is not working - it appears that we get the full list, even those where the Virkning on the reference is not valid
            // we are only interested in relationships that are currently valid
            soegInput.SoegVirkning.FraTidspunkt = new TidspunktType();
            soegInput.SoegVirkning.FraTidspunkt.Item = DateTime.Now;

            // only return objects that have a Tilhører relationship top-level Organisation
            UnikIdType orgReference = StubUtil.GetReference<UnikIdType>(registry.MunicipalityOrganisationUUID, ItemChoiceType.UUIDIdentifikator);
            soegInput.RelationListe.TilknyttedeOrganisationer = new OrganisationFlerRelationType[1];
            soegInput.RelationListe.TilknyttedeOrganisationer[0] = new OrganisationFlerRelationType();
            soegInput.RelationListe.TilknyttedeOrganisationer[0].ReferenceID = orgReference;

            if (!String.IsNullOrEmpty(functionsTypeUuid))
            {
                UnikIdType reference = new UnikIdType();
                reference.Item = functionsTypeUuid;
                reference.ItemElementName = ItemChoiceType.UUIDIdentifikator;

                KlasseRelationType funktionsType = new KlasseRelationType();
                funktionsType.ReferenceID = reference;
                soegInput.RelationListe.Funktionstype = funktionsType;
            }

            if (!String.IsNullOrEmpty(userUuid))
            {
                UnikIdType reference = new UnikIdType();
                reference.Item = userUuid;
                reference.ItemElementName = ItemChoiceType.UUIDIdentifikator;

                soegInput.RelationListe.TilknyttedeBrugere = new BrugerFlerRelationType[1];
                soegInput.RelationListe.TilknyttedeBrugere[0] = new BrugerFlerRelationType();
                soegInput.RelationListe.TilknyttedeBrugere[0].ReferenceID = reference;
            }

            if (!String.IsNullOrEmpty(unitUuid))
            {
                UnikIdType reference = new UnikIdType();
                reference.Item = unitUuid;
                reference.ItemElementName = ItemChoiceType.UUIDIdentifikator;

                soegInput.RelationListe.TilknyttedeEnheder = new OrganisationEnhedFlerRelationType[1];
                soegInput.RelationListe.TilknyttedeEnheder[0] = new OrganisationEnhedFlerRelationType();
                soegInput.RelationListe.TilknyttedeEnheder[0].ReferenceID = reference;

                /*
                // TODO: this should not be here - it is just for testing
                soegInput.RelationListe.TilknyttedeEnheder[0].Virkning = new VirkningType();
                soegInput.RelationListe.TilknyttedeEnheder[0].Virkning.FraTidspunkt = new TidspunktType();
                soegInput.RelationListe.TilknyttedeEnheder[0].Virkning.FraTidspunkt.Item = DateTime.Now;
                */
            }

            if (!String.IsNullOrEmpty(itSystemUuid))
            {
                UnikIdType reference = new UnikIdType();
                reference.Item = itSystemUuid;
                reference.ItemElementName = ItemChoiceType.UUIDIdentifikator;

                soegInput.RelationListe.TilknyttedeItSystemer = new ItSystemFlerRelationType[1];
                soegInput.RelationListe.TilknyttedeItSystemer[0] = new ItSystemFlerRelationType();
                soegInput.RelationListe.TilknyttedeItSystemer[0].ReferenceID = reference;
            }

            // search
            soegRequest request = new soegRequest();
            request.OrganisationFunktionSoegRequest = new OrganisationFunktionSoegRequestType();
            request.OrganisationFunktionSoegRequest.SoegInput = soegInput;
            request.OrganisationFunktionSoegRequest.AuthorityContext = new AuthorityContextType();
            request.OrganisationFunktionSoegRequest.AuthorityContext.MunicipalityCVR = registry.Municipality;

            try
            {
                soegResponse response = channel.soeg(request);
                int statusCode = Int32.Parse(response.OrganisationFunktionSoegResponse.SoegOutput.StandardRetur.StatusKode);
                if (statusCode != 20 && statusCode != 44) // 44 is empty search result
                {
                    string message = StubUtil.ConstructSoapErrorMessage(statusCode, "Soeg", OrganisationFunktionStubHelper.SERVICE, response.OrganisationFunktionSoegResponse.SoegOutput.StandardRetur.FejlbeskedTekst);
                    log.Error(message);
                    throw new SoapServiceException(message);
                }

                List<string> functions = new List<string>();
                if (statusCode == 20)
                {
                    foreach (string id in response.OrganisationFunktionSoegResponse.SoegOutput.IdListe)
                    {
                        functions.Add(id);
                    }
                }

                return functions;
            }
            catch (Exception ex) when (ex is CommunicationException || ex is IOException || ex is TimeoutException || ex is WebException)
            {
                string message = "Failed to establish connection to the Soeg service on OrganisationFunktion";
                log.Error(message, ex);
                throw new ServiceNotFoundException(message, ex);
            }
        }

        public void Orphan(string uuid, DateTime timestamp)
        {
            log.Debug("Attempting Orphan on OrganisationFunktion with uuid " + uuid);

            RegistreringType1 registration = GetLatestRegistration(uuid, false);
            if (registration == null)
            {
                log.Debug("Cannot call Orphan on OrganisationFunktion with uuid " + uuid + " because it does not exist in Organisation");
                return;
            }

            SecurityToken token = TokenCache.IssueToken(OrganisationFunktionStubHelper.SERVICE);
            OrganisationFunktionPortType channel = StubUtil.CreateChannel<OrganisationFunktionPortType>(OrganisationFunktionStubHelper.SERVICE, "Ret", helper.CreatePort(), token);

            try
            {
                RetInputType1 input = new RetInputType1();
                input.UUIDIdentifikator = uuid;
                input.AttributListe = registration.AttributListe;
                input.TilstandListe = registration.TilstandListe;
                input.RelationListe = registration.RelationListe;

                // cut relationship to all users
                if (input.RelationListe.TilknyttedeBrugere != null && input.RelationListe.TilknyttedeBrugere.Length > 0)
                {
                    foreach (var bruger in input.RelationListe.TilknyttedeBrugere)
                    {
                        StubUtil.TerminateVirkning(bruger.Virkning, timestamp);
                    }
                }

                // cut relationship to all orgUnits
                if (input.RelationListe.TilknyttedeEnheder != null && input.RelationListe.TilknyttedeEnheder.Length > 0)
                {
                    foreach (var enhed in input.RelationListe.TilknyttedeEnheder)
                    {
                        StubUtil.TerminateVirkning(enhed.Virkning, timestamp);
                    }
                }

                // cut relationship to all ItSystems
                if (input.RelationListe.TilknyttedeItSystemer != null && input.RelationListe.TilknyttedeItSystemer.Length > 0)
                {
                    foreach (var itSystem in input.RelationListe.TilknyttedeItSystemer)
                    {
                        StubUtil.TerminateVirkning(itSystem.Virkning, timestamp);
                    }
                }

                // cut relationship to Organisation
                if (input.RelationListe.TilknyttedeOrganisationer != null && input.RelationListe.TilknyttedeOrganisationer.Length > 0)
                {
                    foreach (var organisation in input.RelationListe.TilknyttedeOrganisationer)
                    {
                        StubUtil.TerminateVirkning(organisation.Virkning, timestamp);
                    }
                }

                retRequest request = new retRequest();
                request.OrganisationFunktionRetRequest = new OrganisationFunktionRetRequestType();
                request.OrganisationFunktionRetRequest.RetInput = input;
                request.OrganisationFunktionRetRequest.AuthorityContext = new AuthorityContextType();
                request.OrganisationFunktionRetRequest.AuthorityContext.MunicipalityCVR = registry.Municipality;

                retResponse response = channel.ret(request);

                int statusCode = Int32.Parse(response.OrganisationFunktionRetResponse.RetOutput.StandardRetur.StatusKode);
                if (statusCode != 20)
                {
                    string message = StubUtil.ConstructSoapErrorMessage(statusCode, "Ret", OrganisationFunktionStubHelper.SERVICE, response.OrganisationFunktionRetResponse.RetOutput.StandardRetur.FejlbeskedTekst);
                    log.Error(message);
                    throw new SoapServiceException(message);
                }

                log.Debug("Orphan on OrganisationFunktion with uuid " + uuid + " succeded");
            }
            catch (Exception ex) when (ex is CommunicationException || ex is IOException || ex is TimeoutException || ex is WebException)
            {
                string message = "Failed to establish connection to the Ret service on OrganisationFunktion";
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
            FiltreretOejebliksbilledeType[] registrations = GetLatestRegistrations(new string[] { uuid }, actualStateOnly);
            if (registrations == null || registrations.Length == 0)
            {
                return null;
            }

            RegistreringType1[] resultSet = registrations[0].Registrering;
            if (resultSet.Length == 0)
            {
                log.Warn("OrgFunction with uuid '" + uuid + "' exists, but has no registration");
                return null;
            }

            RegistreringType1 result = null;
            if (resultSet.Length > 1)
            {
                log.Warn("OrgFunction with uuid " + uuid + " has more than one registration when reading latest registration, this should never happen");

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

        // TODO: this method contains a work-around.
        //       KMD released version 1.4 of Organisation, containing a series of changes to how reading and searching for data works, unfortunately this new functionality
        //       does not cover the actually required use-cases (e.g. reading actual state), but luckily they forgot to change the List() operation, so we are using List()
        //       instead of Laes() until Laes() is fixed in a later release of Organisation.
        public FiltreretOejebliksbilledeType[] GetLatestRegistrations(string[] uuids, bool actualStateOnly)
        {
            ListInputType listInput = new ListInputType();
            listInput.UUIDIdentifikator = uuids;

            // this ensures that we get the full history when reading, and not just what is valid right now
            if (!actualStateOnly)
            {
                listInput.VirkningFraFilter = new TidspunktType();
                listInput.VirkningFraFilter.Item = true;
                listInput.VirkningTilFilter = new TidspunktType();
                listInput.VirkningTilFilter.Item = true;
            }

            listRequest request = new listRequest();
            request.OrganisationFunktionListeRequest = new OrganisationFunktionListeRequestType();
            request.OrganisationFunktionListeRequest.ListInput = listInput;
            request.OrganisationFunktionListeRequest.AuthorityContext = new AuthorityContextType();
            request.OrganisationFunktionListeRequest.AuthorityContext.MunicipalityCVR = registry.Municipality;

            SecurityToken token = TokenCache.IssueToken(OrganisationFunktionStubHelper.SERVICE);
            OrganisationFunktionPortType channel = StubUtil.CreateChannel<OrganisationFunktionPortType>(OrganisationFunktionStubHelper.SERVICE, "Laes", helper.CreatePort(), token);

            try
            {
                listResponse response = channel.list(request);

                int statusCode = Int32.Parse(response.OrganisationFunktionListeResponse.ListOutput.StandardRetur.StatusKode);
                if (statusCode != 20)
                {
                    // note that statusCode 44 means that the objects does not exists, so that is a valid response
                    if (statusCode != 44)
                    {
                        log.Warn("Lookup on OrgFunction with uuids '" + string.Join(",", uuids) + "' failed with statuscode " + statusCode);
                    }
                    else
                    {
                        log.Debug("Lookup on OrgFunction with uuids '" + string.Join(",", uuids) + "' failed with statuscode " + statusCode);
                    }

                    return null;
                }

                if (response.OrganisationFunktionListeResponse.ListOutput.FiltreretOejebliksbillede == null || response.OrganisationFunktionListeResponse.ListOutput.FiltreretOejebliksbillede.Length == 0)
                {
                    log.Debug("Lookup on OrgFunction with uuids '" + string.Join(",", uuids) + "' returned an empty resultset");
                    return null;
                }

                return response.OrganisationFunktionListeResponse.ListOutput.FiltreretOejebliksbillede;
            }
            catch (Exception ex) when (ex is CommunicationException || ex is IOException || ex is TimeoutException || ex is WebException)
            {
                string message = "Failed to establish connection to the Laes service on OrganisationFunktion";
                log.Error(message, ex);
                throw new ServiceNotFoundException(message, ex);
            }
        }

        // this is a special version of the one found in StubUtil - it handles removing objects that are present in the input (so the reverse method basically ;))
        private bool TerminateObjectsInOrgThatAreInLocal(dynamic objectsInOrg, List<string> objectsInLocal, DateTime timestamp)
        {
            bool changes = false;

            if (objectsInOrg != null)
            {
                foreach (var objectInOrg in objectsInOrg)
                {
                    if (objectsInLocal != null)
                    {
                        foreach (var objectInLocal in objectsInLocal)
                        {
                            if (objectInLocal.Equals(objectInOrg.ReferenceID.Item))
                            {
                                // the objectsInOrg collection contains references that are already terminaited, so TerminateVirkning
                                // only returns true if it actually terminiated the reference - and we do not want to flag the object
                                // as modified unless it actually is
                                if (StubUtil.TerminateVirkning(objectInOrg.Virkning, timestamp))
                                {
                                    changes = true;
                                }
                            }
                        }
                    }
                }
            }

            return changes;
        }

        private void EnsureKeys(OrgFunctionData orgFunction)
        {
            orgFunction.Uuid = (orgFunction.Uuid != null) ? orgFunction.Uuid : IdUtil.GenerateUuid();
            orgFunction.ShortKey = (orgFunction.ShortKey != null) ? orgFunction.ShortKey : IdUtil.GenerateShortKey();
        }
    }
}
