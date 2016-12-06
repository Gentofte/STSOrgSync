using System;
using System.IdentityModel.Tokens;
using IntegrationLayer.Person;
using System.ServiceModel;
using System.IO;
using System.Net;

namespace Organisation.IntegrationLayer
{
    internal class PersonStub
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private PersonStubHelper helper = new PersonStubHelper();
        private OrganisationRegistryProperties registry = OrganisationRegistryProperties.GetInstance();

        public void Importer(PersonData person)
        {
            // create ShortKey and Uuid if not supplied
            EnsureKeys(person);

            log.Debug("Attempting Import on Person with uuid " + person.Uuid);

            // create timestamp object to be used on all registrations, properties and relations
            VirkningType virkning = helper.GetVirkning(person.Timestamp);

            // setup registration
            RegistreringType1 registration = helper.CreateRegistration(person.Timestamp, LivscyklusKodeType.Importeret);

            // add properties
            helper.AddProperties(person.Name, person.ShortKey, person.Cpr, virkning, registration);

            // wire everything together
            PersonType personType = helper.GetPersonType(person.Uuid, registration);
            ImportInputType importInput = new ImportInputType();
            importInput.Person = personType;

            // construct request
            importerRequest request = new importerRequest();
            request.PersonImporterRequest = new PersonImporterRequestType();
            request.PersonImporterRequest.ImportInput = importInput;
            request.PersonImporterRequest.AuthorityContext = new AuthorityContextType();
            request.PersonImporterRequest.AuthorityContext.MunicipalityCVR = registry.Municipality;

            // send request
            SecurityToken token = TokenCache.IssueToken(PersonStubHelper.SERVICE);
            PersonPortType channel = StubUtil.CreateChannel<PersonPortType>(PersonStubHelper.SERVICE,  "Import", helper.CreatePort(), token);

            try
            {
                importerResponse response = channel.importer(request);
                int statusCode = Int32.Parse(response.PersonImporterResponse.ImportOutput.StandardRetur.StatusKode);
                if (statusCode != 20)
                {
                    string message = StubUtil.ConstructSoapErrorMessage(statusCode, "Import", PersonStubHelper.SERVICE, response.PersonImporterResponse.ImportOutput.StandardRetur.FejlbeskedTekst);
                    log.Error(message);
                    throw new SoapServiceException(message);
                }

                log.Debug("Import successful on Person with uuid " + person.Uuid);
            }
            catch (Exception ex) when (ex is CommunicationException || ex is IOException || ex is TimeoutException || ex is WebException)
            {
                string message = "Failed to establish connection to the Importer service on Person";
                log.Error(message, ex);
                throw new ServiceNotFoundException(message, ex);
            }
        }

        // this method only performs a call to Ret on Organisation if there are actual changes
        public void Ret(string uuid, string newName, string newShortKey, string newCpr, DateTime timestamp)
        {
            log.Debug("Attempting Ret on Person with uuid " + uuid);

            // this should never fail - we always import the Person before the User, and we have a User with a reference to this object
            RegistreringType1 registration = GetLatestRegistration(uuid, false);
            if (registration == null)
            {
                log.Debug("Cannot call Ret on Person with uuid " + uuid + " because it does not exist in Organisation");
                return;
            }

            SecurityToken token = TokenCache.IssueToken(PersonStubHelper.SERVICE);
            PersonPortType channel = StubUtil.CreateChannel<PersonPortType>(PersonStubHelper.SERVICE, "Ret", helper.CreatePort(), token);

            try
            {
                RetInputType1 input = new RetInputType1();
                input.UUIDIdentifikator = uuid;
                input.AttributListe = registration.AttributListe;
                input.TilstandListe = registration.TilstandListe;
                input.RelationListe = registration.RelationListe;

                // compare latest property to the local object
                EgenskabType latestProperty = StubUtil.GetLatestProperty(input.AttributListe);
                if (latestProperty == null || !latestProperty.NavnTekst.Equals(newName) || (latestProperty.CPRNummerTekst == null && newCpr != null) || (latestProperty.CPRNummerTekst != null && !latestProperty.CPRNummerTekst.Equals(newCpr)) || (newShortKey != null && !latestProperty.BrugervendtNoegleTekst.Equals(newShortKey)))
                {
                    // end the validity of open-ended property
                    if (latestProperty != null)
                    {
                        StubUtil.TerminateVirkning(latestProperty.Virkning, timestamp);
                    }

                    // create a new property
                    EgenskabType newProperty = new EgenskabType();
                    newProperty.Virkning = helper.GetVirkning(timestamp);
                    newProperty.BrugervendtNoegleTekst = ((newShortKey != null) ? newShortKey : latestProperty.BrugervendtNoegleTekst);
                    newProperty.NavnTekst = newName;
                    newProperty.CPRNummerTekst = newCpr;

                    // create a new set of properties
                    EgenskabType[] oldProperties = input.AttributListe;
                    input.AttributListe = new EgenskabType[oldProperties.Length + 1];
                    for (int i = 0; i < oldProperties.Length; i++)
                    {
                        input.AttributListe[i] = oldProperties[i];
                    }
                    input.AttributListe[oldProperties.Length] = newProperty;
                }
                else
                {
                    log.Debug("No changes on Person, so returning without calling Organisation");

                    // if there are no changes to the attributes, we do not call the Organisation service
                    return;
                }

                // send Ret request
                retRequest request = new retRequest();
                request.PersonRetRequest = new PersonRetRequestType();
                request.PersonRetRequest.RetInput = input;
                request.PersonRetRequest.AuthorityContext = new AuthorityContextType();
                request.PersonRetRequest.AuthorityContext.MunicipalityCVR = registry.Municipality;

                retResponse response = channel.ret(request);

                int statusCode = Int32.Parse(response.PersonRetResponse.RetOutput.StandardRetur.StatusKode);
                if (statusCode != 20)
                {
                    string message = StubUtil.ConstructSoapErrorMessage(statusCode, "Ret", PersonStubHelper.SERVICE, response.PersonRetResponse.RetOutput.StandardRetur.FejlbeskedTekst);
                    log.Error(message);
                    throw new SoapServiceException(message);
                }

                log.Debug("Ret successful on Person with uuid " + uuid);
            }
            catch (Exception ex) when (ex is CommunicationException || ex is IOException || ex is TimeoutException || ex is WebException)
            {
                string message = "Failed to establish connection to the Ret service on Person";
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
            request.PersonListeRequest = new PersonListeRequestType();
            request.PersonListeRequest.ListInput = listInput;
            request.PersonListeRequest.AuthorityContext = new AuthorityContextType();
            request.PersonListeRequest.AuthorityContext.MunicipalityCVR = registry.Municipality;

            SecurityToken token = TokenCache.IssueToken(PersonStubHelper.SERVICE);
            PersonPortType channel = StubUtil.CreateChannel<PersonPortType>(PersonStubHelper.SERVICE, "Laes", helper.CreatePort(), token);

            try
            {
                listResponse response = channel.list(request);

                int statusCode = Int32.Parse(response.PersonListeResponse.ListOutput.StandardRetur.StatusKode);
                if (statusCode != 20)
                {
                    // note that statusCode 44 means that the object does not exists, so that is a valid response
                    log.Debug("Lookup on Person with uuid '" + uuid + "' failed with statuscode " + statusCode);
                    return null; 
                }

                if (response.PersonListeResponse.ListOutput.FiltreretOejebliksbillede.Length != 1)
                {
                    log.Warn("Lookup Person with uuid '" + uuid + "' returned multiple objects");
                    return null;
                }

                RegistreringType1[] resultSet = response.PersonListeResponse.ListOutput.FiltreretOejebliksbillede[0].Registrering;
                if (resultSet.Length == 0)
                {
                    log.Warn("Person with uuid '" + uuid + "' exists, but has no registration");
                    return null;
                }

                RegistreringType1 result = null;
                if (resultSet.Length > 1)
                {
                    log.Warn("Person with uuid " + uuid + " has more than one registration when reading latest registration, this should never happen");

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
                string message = "Failed to establish connection to the Laes service on Person";
                log.Error(message, ex);
                throw new ServiceNotFoundException(message, ex);
            }
        }

        private void EnsureKeys(PersonData person)
        {
            person.Uuid = (person.Uuid != null) ? person.Uuid : IdUtil.GenerateUuid();
            person.ShortKey = (person.ShortKey != null) ? person.ShortKey : IdUtil.GenerateShortKey();
        }
    }
}
