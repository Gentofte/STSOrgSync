using IntegrationLayer.OrganisationEnhed;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.IO;
using System.Net;
using System.ServiceModel;

namespace Organisation.IntegrationLayer
{
    internal class OrganisationEnhedStub
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private OrganisationEnhedStubHelper helper = new OrganisationEnhedStubHelper();
        private OrganisationRegistryProperties registry = OrganisationRegistryProperties.GetInstance();

        public void Importer(OrgUnitData unit)
        {
            log.Debug("Attempting Importer on OrganisationEnhed with uuid " + unit.Uuid);

            // create ShortKey if not supplied
            EnsureKeys(unit);

            // create timestamp object to be used on all registrations, properties and relations
            VirkningType virkning = helper.GetVirkning(unit.Timestamp);

            // setup registration
            RegistreringType1 registration = helper.CreateRegistration(unit, LivscyklusKodeType.Importeret);

            // add properties
            helper.AddProperties(unit.ShortKey, unit.Name, virkning, registration);

            // add relationships
            helper.AddAddressReferences(unit.Addresses, virkning, registration);
            helper.AddOrganisationRelation(StubUtil.GetMunicipalityOrganisationUUID(), virkning, registration);
            helper.AddOverordnetEnhed(unit.ParentOrgUnitUuid, virkning, registration);
            helper.AddTilknyttedeFunktioner(unit.OrgFunctionUuids, virkning, registration);

            // set Tilstand to Active
            helper.SetTilstandToActive(virkning, registration);

            // wire everything together
            OrganisationEnhedType organisationEnhedType = helper.GetOrganisationEnhedType(unit.Uuid, registration);
            ImportInputType importInput = new ImportInputType();
            importInput.OrganisationEnhed = organisationEnhedType;

            // construct request
            importerRequest request = new importerRequest();
            request.OrganisationEnhedImporterRequest = new OrganisationEnhedImporterRequestType();
            request.OrganisationEnhedImporterRequest.ImportInput = importInput;
            request.OrganisationEnhedImporterRequest.AuthorityContext = new AuthorityContextType();
            request.OrganisationEnhedImporterRequest.AuthorityContext.MunicipalityCVR = registry.Municipality;

            // send request
            SecurityToken token = TokenCache.IssueToken(OrganisationEnhedStubHelper.SERVICE);
            OrganisationEnhedPortType channel = StubUtil.CreateChannel<OrganisationEnhedPortType>(OrganisationEnhedStubHelper.SERVICE, "Importer", helper.CreatePort(), token);

            try
            {
                importerResponse result = channel.importer(request);
                int statusCode = Int32.Parse(result.OrganisationEnhedImporterResponse.ImportOutput.StandardRetur.StatusKode);
                if (statusCode != 20)
                {
                    string message = StubUtil.ConstructSoapErrorMessage(statusCode, "Import", OrganisationEnhedStubHelper.SERVICE, result.OrganisationEnhedImporterResponse.ImportOutput.StandardRetur.FejlbeskedTekst);
                    log.Error(message);
                    throw new SoapServiceException(message);
                }

                log.Debug("Importer successful on OrganisationEnhed with uuid " + unit.Uuid);
            }
            catch (Exception ex) when (ex is CommunicationException || ex is IOException || ex is TimeoutException || ex is WebException)
            {
                string message = "Failed to establish connection to the Importer service on OrganisationEnhed";
                log.Error(message, ex);
                throw new ServiceNotFoundException(message, ex);
            }
        }

        public void Ret(OrgUnitData unit)
        {
            log.Debug("Attempting Ret on OrganisationEnhed with uuid " + unit.Uuid);

            RegistreringType1 registration = GetLatestRegistration(unit.Uuid, false);
            if (registration == null)
            {
                log.Debug("Cannot call Ret on OrganisationEnhed with uuid " + unit.Uuid + " because it does not exist in Organisation");
                return;
            }

            VirkningType virkning = helper.GetVirkning(unit.Timestamp);

            SecurityToken token = TokenCache.IssueToken(OrganisationEnhedStubHelper.SERVICE);
            OrganisationEnhedPortType channel = StubUtil.CreateChannel<OrganisationEnhedPortType>(OrganisationEnhedStubHelper.SERVICE, "Ret", helper.CreatePort(), token);

            try
            {
                bool changes = false;

                RetInputType1 input = new RetInputType1();
                input.UUIDIdentifikator = unit.Uuid;
                input.AttributListe = registration.AttributListe;
                input.TilstandListe = registration.TilstandListe;
                input.RelationListe = registration.RelationListe;

                #region Update attributes

                // compare latest property to the local object
                EgenskabType latestProperty = StubUtil.GetLatestProperty(input.AttributListe.Egenskab);
                if (latestProperty == null || !latestProperty.EnhedNavn.Equals(unit.Name) || (unit.ShortKey != null && !latestProperty.BrugervendtNoegleTekst.Equals(unit.ShortKey)))
                {
                    // end the validity of open-ended property
                    if (latestProperty != null)
                    {
                        StubUtil.TerminateVirkning(latestProperty.Virkning, unit.Timestamp);
                    }
                    else
                    {
                        EnsureKeys(unit);
                    }

                    // create a new property
                    EgenskabType newProperty = new EgenskabType();
                    newProperty.Virkning = helper.GetVirkning(unit.Timestamp);
                    newProperty.BrugervendtNoegleTekst = ((unit.ShortKey != null) ? unit.ShortKey : latestProperty.BrugervendtNoegleTekst);
                    newProperty.EnhedNavn = unit.Name;

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
                changes = StubUtil.TerminateObjectsInOrgNoLongerPresentLocally(input.RelationListe.Adresser, unit.Addresses, unit.Timestamp, true) || changes;

                // add references to address objects that are new
                List<string> uuidsToAdd = StubUtil.FindAllObjectsInLocalNotInOrg(input.RelationListe.Adresser, unit.Addresses, true);

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
                        foreach (var addressInLocal in unit.Addresses)
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
                                    case AddressRelationType.LOSSHORTNAME:
                                        roleUuid = UUIDConstants.ADDRESS_ROLE_LOSSHORTNAME;
                                        break;
                                    case AddressRelationType.CONTACT_ADDRESS_OPEN_HOURS:
                                        roleUuid = UUIDConstants.ADDRESS_ROLE_CONTACT_ADDRESS_OPEN_HOURS;
                                        break;
                                    case AddressRelationType.EAN:
                                        roleUuid = UUIDConstants.ADDRESS_ROLE_EAN;
                                        break;
                                    case AddressRelationType.EMAIL_REMARKS:
                                        roleUuid = UUIDConstants.ADDRESS_ROLE_EMAIL_REMARKS;
                                        break;
                                    case AddressRelationType.POST_RETURN:
                                        roleUuid = UUIDConstants.ADDRESS_ROLE_POST_RETURN;
                                        break;
                                    case AddressRelationType.CONTACT_ADDRESS:
                                        roleUuid = UUIDConstants.ADDRESS_ROLE_CONTACT_ADDRESS;
                                        break;
                                    case AddressRelationType.POST:
                                        roleUuid = UUIDConstants.ADDRESS_ROLE_POST;
                                        break;
                                    case AddressRelationType.PHONE_OPEN_HOURS:
                                        roleUuid = UUIDConstants.ADDRESS_ROLE_PHONE_OPEN_HOURS;
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
                        log.Debug("Re-establishing relationship with Organisation for OrgUnit " + unit.Uuid);
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

                #region Update parent relationship
                if (registration.RelationListe.Overordnet != null)
                {
                    // there is an existing Overordnet relationship, so let us see if there is a change
                    if (unit.ParentOrgUnitUuid != null)
                    {
                        bool expired = false;
                        object endDate = registration.RelationListe.Overordnet.Virkning.TilTidspunkt.Item;
                        if (endDate != null && endDate is DateTime)
                        {
                            expired = true;
                        }

                        if (expired || !registration.RelationListe.Overordnet.ReferenceID.Item.Equals(unit.ParentOrgUnitUuid))
                        {
                            // overwrite the existing values (we cannot create multiple references on this, so it is the best we can do with regards to storing full history in the latest registration)
                            registration.RelationListe.Overordnet.ReferenceID = StubUtil.GetReference<UnikIdType>(unit.ParentOrgUnitUuid, ItemChoiceType.UUIDIdentifikator);
                            registration.RelationListe.Overordnet.Virkning = virkning;
                            changes = true;
                        }
                    }
                    else
                    {
                        // attempt to terminate the existing relationship (it might already be terminated)
                        if (StubUtil.TerminateVirkning(registration.RelationListe.Overordnet.Virkning, unit.Timestamp))
                        {
                            changes = true;
                        }
                    }
                }
                else if (unit.ParentOrgUnitUuid != null)
                {
                    // no existing parent, so just create one
                    helper.AddOverordnetEnhed(unit.ParentOrgUnitUuid, virkning, registration);
                    changes = true;
                }
                #endregion

                #region Update payout units
                // terminate the Virkning on all functions (currently there is only one, the payout unit, but this will work for all kinds of functions)
                changes = StubUtil.TerminateObjectsInOrgNoLongerPresentLocally(input.RelationListe.TilknyttedeFunktioner, unit.OrgFunctionUuids, unit.Timestamp, false) || changes;

                // add references to function objects that are new
                List<string> functionUuidsToAdd = StubUtil.FindAllObjectsInLocalNotInOrg(input.RelationListe.TilknyttedeFunktioner, unit.OrgFunctionUuids, false);

                if (functionUuidsToAdd.Count > 0)
                {
                    int size = functionUuidsToAdd.Count + ((input.RelationListe.TilknyttedeFunktioner != null) ? input.RelationListe.TilknyttedeFunktioner.Length : 0);
                    OrganisationFunktionFlerRelationType[] newFunctions = new OrganisationFunktionFlerRelationType[size];

                    int i = 0;
                    if (input.RelationListe.TilknyttedeFunktioner != null)
                    {
                        foreach (var functionsInOrg in input.RelationListe.TilknyttedeFunktioner)
                        {
                            newFunctions[i++] = functionsInOrg;
                        }
                    }

                    foreach (string uuidToAdd in functionUuidsToAdd)
                    {
                        foreach (var functionsInLocal in unit.OrgFunctionUuids)
                        {
                            OrganisationFunktionFlerRelationType newFunction = new OrganisationFunktionFlerRelationType();
                            newFunction.ReferenceID = StubUtil.GetReference<UnikIdType>(functionsInLocal, ItemChoiceType.UUIDIdentifikator);
                            newFunction.Virkning = virkning;

                            newFunctions[i++] = newFunction;
                        }
                    }

                    input.RelationListe.TilknyttedeFunktioner = newFunctions;
                    changes = true;
                }
                #endregion

                // if no changes are made, we do not call the service
                if (!changes)
                {
                    log.Debug("Ret on OrganisationEnhed with uuid " + unit.Uuid + " cancelled because of no changes");
                    return;
                }

                // send Ret request
                retRequest request = new retRequest();
                request.OrganisationEnhedRetRequest = new OrganisationEnhedRetRequestType();
                request.OrganisationEnhedRetRequest.RetInput = input;
                request.OrganisationEnhedRetRequest.AuthorityContext = new AuthorityContextType();
                request.OrganisationEnhedRetRequest.AuthorityContext.MunicipalityCVR = registry.Municipality;

                retResponse response = channel.ret(request);

                int statusCode = Int32.Parse(response.OrganisationEnhedRetResponse.RetOutput.StandardRetur.StatusKode);
                if (statusCode != 20)
                {
                    string message = StubUtil.ConstructSoapErrorMessage(statusCode, "Ret", OrganisationEnhedStubHelper.SERVICE, response.OrganisationEnhedRetResponse.RetOutput.StandardRetur.FejlbeskedTekst);
                    log.Error(message);
                    throw new SoapServiceException(message);
                }

                log.Debug("Ret succesful on OrganisationEnhed with uuid " + unit.Uuid);
            }
            catch (Exception ex) when (ex is CommunicationException || ex is IOException || ex is TimeoutException || ex is WebException)
            {
                string message = "Failed to establish connection to the Ret service on OrganisationEnhed";
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
            request.OrganisationEnhedListeRequest = new OrganisationEnhedListeRequestType();
            request.OrganisationEnhedListeRequest.ListInput = listInput;
            request.OrganisationEnhedListeRequest.AuthorityContext = new AuthorityContextType();
            request.OrganisationEnhedListeRequest.AuthorityContext.MunicipalityCVR = registry.Municipality;

            SecurityToken token = TokenCache.IssueToken(OrganisationEnhedStubHelper.SERVICE);
            OrganisationEnhedPortType channel = StubUtil.CreateChannel<OrganisationEnhedPortType>(OrganisationEnhedStubHelper.SERVICE, "Laes", helper.CreatePort(), token);

            try
            {
                listResponse response = channel.list(request);

                int statusCode = Int32.Parse(response.OrganisationEnhedListeResponse.ListOutput.StandardRetur.StatusKode);
                if (statusCode != 20)
                {
                    // note that statusCode 44 means that the object does not exists, so that is a valid response
                    log.Debug("Lookup on OrgUnit with uuid '" + uuid + "' failed with statuscode " + statusCode);
                    return null;
                }

                if (response.OrganisationEnhedListeResponse.ListOutput.FiltreretOejebliksbillede.Length != 1)
                {
                    log.Warn("Lookup OrgUnit with uuid '" + uuid + "' returned multiple objects");
                    return null;
                }

                RegistreringType1[] resultSet = response.OrganisationEnhedListeResponse.ListOutput.FiltreretOejebliksbillede[0].Registrering;
                if (resultSet.Length == 0)
                {
                    log.Warn("OrgUnit with uuid '" + uuid + "' exists, but has no registration");
                    return null;
                }

                RegistreringType1 result = null;
                if (resultSet.Length > 1)
                {
                    log.Warn("OrgUnit with uuid " + uuid + " has more than one registration when reading latest registration, this should never happen");

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
                string message = "Failed to establish connection to the Laes service on OrganisationEnhed";
                log.Error(message, ex);
                throw new ServiceNotFoundException(message, ex);
            }
        }

        public void Orphan(string uuid, DateTime timestamp)
        {
            log.Debug("Attempting Orphan on OrganisationEnhed with uuid " + uuid);

            RegistreringType1 registration = GetLatestRegistration(uuid, false);
            if (registration == null)
            {
                log.Debug("Cannot Orphan OrganisationEnhed with uuid " + uuid + " because it does not exist in Organisation");
                return;
            }

            SecurityToken token = TokenCache.IssueToken(OrganisationEnhedStubHelper.SERVICE);
            OrganisationEnhedPortType channel = StubUtil.CreateChannel<OrganisationEnhedPortType>(OrganisationEnhedStubHelper.SERVICE, "Laes", helper.CreatePort(), token);

            try
            {
                RetInputType1 input = new RetInputType1();
                input.UUIDIdentifikator = uuid;
                input.AttributListe = registration.AttributListe;
                input.TilstandListe = registration.TilstandListe;
                input.RelationListe = registration.RelationListe;

                // cut relationship to Organisation
                if (input.RelationListe.Tilhoerer != null)
                {
                    StubUtil.TerminateVirkning(input.RelationListe.Tilhoerer.Virkning, timestamp);
                }

                // cut relationship to Parent
                if (input.RelationListe.Overordnet != null)
                {
                    StubUtil.TerminateVirkning(input.RelationListe.Overordnet.Virkning, timestamp);
                }

                // cut relationship to all functions within the Organisation (payout unit references)
                if (input.RelationListe.TilknyttedeFunktioner != null && input.RelationListe.TilknyttedeFunktioner.Length > 0)
                {
                    foreach (OrganisationFunktionFlerRelationType funktion in input.RelationListe.TilknyttedeFunktioner)
                    {
                        StubUtil.TerminateVirkning(funktion.Virkning, timestamp);
                    }
                }

                retRequest request = new retRequest();
                request.OrganisationEnhedRetRequest = new OrganisationEnhedRetRequestType();
                request.OrganisationEnhedRetRequest.RetInput = input;
                request.OrganisationEnhedRetRequest.AuthorityContext = new AuthorityContextType();
                request.OrganisationEnhedRetRequest.AuthorityContext.MunicipalityCVR = registry.Municipality;

                retResponse response = channel.ret(request);

                int statusCode = Int32.Parse(response.OrganisationEnhedRetResponse.RetOutput.StandardRetur.StatusKode);
                if (statusCode != 20)
                {
                    string message = StubUtil.ConstructSoapErrorMessage(statusCode, "Ret", OrganisationEnhedStubHelper.SERVICE, response.OrganisationEnhedRetResponse.RetOutput.StandardRetur.FejlbeskedTekst);
                    log.Error(message);
                    throw new SoapServiceException(message);
                }

                log.Debug("Orphan successful on OrganisationEnhed with uuid " + uuid);
            }
            catch (Exception ex) when (ex is CommunicationException || ex is IOException || ex is TimeoutException || ex is WebException)
            {
                string message = "Failed to establish connection to the Ret service on OrganisationEnhed";
                log.Error(message, ex);
                throw new ServiceNotFoundException(message, ex);
            }
        }

        public List<string> Soeg()
        {
            SecurityToken token = TokenCache.IssueToken(OrganisationEnhedStubHelper.SERVICE);
            OrganisationEnhedPortType channel = StubUtil.CreateChannel<OrganisationEnhedPortType>(OrganisationEnhedStubHelper.SERVICE, "Soeg", helper.CreatePort(), token);

            SoegInputType1 soegInput = new SoegInputType1();
            soegInput.AttributListe = new AttributListeType();
            soegInput.RelationListe = new RelationListeType();
            soegInput.SoegRegistrering = new SoegRegistreringType();
            soegInput.SoegVirkning = new SoegVirkningType();
            soegInput.TilstandListe = new TilstandListeType();
            soegInput.MaksimalAntalKvantitet = "5000"; // the default limit is 500, and for the report tool, we need to extract ALL OUs/Users, which can be a higher number

            // TODO: This is not working - it appears that we get the full list, even those where the Virkning on the reference is not valid
            // we are only interested in relationships that are currently valid
            soegInput.SoegVirkning.FraTidspunkt = new TidspunktType();
            soegInput.SoegVirkning.FraTidspunkt.Item = DateTime.Now;

            // only return objects that have a Tilhører relationship top-level Organisation
            UnikIdType orgReference = StubUtil.GetReference<UnikIdType>(registry.MunicipalityOrganisationUUID, ItemChoiceType.UUIDIdentifikator);
            OrganisationRelationType organisationRelationType = new OrganisationRelationType();
            organisationRelationType.ReferenceID = orgReference;
            soegInput.RelationListe.Tilhoerer = organisationRelationType;

            // search
            soegRequest request = new soegRequest();
            request.OrganisationEnhedSoegRequest = new OrganisationEnhedSoegRequestType();
            request.OrganisationEnhedSoegRequest.SoegInput = soegInput;
            request.OrganisationEnhedSoegRequest.AuthorityContext = new AuthorityContextType();
            request.OrganisationEnhedSoegRequest.AuthorityContext.MunicipalityCVR = registry.Municipality;

            try
            {
                soegResponse response = channel.soeg(request);
                int statusCode = Int32.Parse(response.OrganisationEnhedSoegResponse.SoegOutput.StandardRetur.StatusKode);
                if (statusCode != 20 && statusCode != 44) // 44 is empty search result
                {
                    string message = StubUtil.ConstructSoapErrorMessage(statusCode, "Soeg", OrganisationEnhedStubHelper.SERVICE, response.OrganisationEnhedSoegResponse.SoegOutput.StandardRetur.FejlbeskedTekst);
                    log.Error(message);
                    throw new SoapServiceException(message);
                }

                List<string> functions = new List<string>();
                if (statusCode == 20)
                {
                    foreach (string id in response.OrganisationEnhedSoegResponse.SoegOutput.IdListe)
                    {
                        functions.Add(id);
                    }
                }

                return functions;
            }
            catch (Exception ex) when (ex is CommunicationException || ex is IOException || ex is TimeoutException || ex is WebException)
            {
                string message = "Failed to establish connection to the Soeg service on OrganisationEnhed";
                log.Error(message, ex);
                throw new ServiceNotFoundException(message, ex);
            }
        }

        private void EnsureKeys(OrgUnitData unit)
        {
            unit.ShortKey = (unit.ShortKey != null) ? unit.ShortKey : IdUtil.GenerateShortKey();
        }
    }
}
