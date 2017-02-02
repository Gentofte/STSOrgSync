using IntegrationLayer.Adresse;
using System;
using System.IdentityModel.Tokens;
using System.IO;
using System.Net;
using System.ServiceModel;

namespace Organisation.IntegrationLayer
{
    internal class AdresseStub
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private AdresseStubHelper helper = new AdresseStubHelper();
        private OrganisationRegistryProperties registry = OrganisationRegistryProperties.GetInstance();

        public void Importer(AddressData address)
        {
            // create ShortKey and Uuid if not supplied
            EnsureKeys(address);

            log.Debug("Attempting Import on Address with uuid " + address.Uuid);

            // create timestamp object to be used on all registrations, properties and relations
            VirkningType virkning = helper.GetVirkning(address.Timestamp);

            // setup registration
            RegistreringType1 registration = helper.CreateRegistration(address.Timestamp, LivscyklusKodeType.Importeret);

            // add properties
            helper.AddProperties(address.AddressText, address.ShortKey, virkning, registration);

            // wire everything together
            AdresseType addresseType = helper.GetAdresseType(address.Uuid, registration);
            ImportInputType inportInput = new ImportInputType();
            inportInput.Adresse = addresseType;

            // construct request
            importerRequest request = new importerRequest();
            request.AdresseImporterRequest = new AdresseImporterRequestType();
            request.AdresseImporterRequest.ImportInput = inportInput;
            request.AdresseImporterRequest.AuthorityContext = new AuthorityContextType();
            request.AdresseImporterRequest.AuthorityContext.MunicipalityCVR = registry.Municipality;

            // send request
            SecurityToken token = TokenCache.IssueToken(AdresseStubHelper.SERVICE);
            AdressePortType channel = StubUtil.CreateChannel<AdressePortType>(AdresseStubHelper.SERVICE, "Importer", helper.CreatePort(), token);

            try
            {
                importerResponse response = channel.importer(request);
                int statusCode = Int32.Parse(response.AdresseImporterResponse.ImportOutput.StandardRetur.StatusKode);
                if (statusCode != 20)
                {
                    if (statusCode == 49) // object already exists is the most likely scenario here
                    {
                        // TODO: a better approach would be to try the read-then-update-if-exists-else-create approach we use elsewhere
                        log.Info("Skipping import on Address " + address.Uuid + " as Organisation returned status 49. The most likely cause is that the object already exists");
                        return;
                    }

                    string message = StubUtil.ConstructSoapErrorMessage(statusCode, "Import", AdresseStubHelper.SERVICE, response.AdresseImporterResponse.ImportOutput.StandardRetur.FejlbeskedTekst);
                    log.Error(message);
                    throw new SoapServiceException(message);
                }

                log.Debug("Import successful on Address with uuid " + address.Uuid);
            }
            catch (Exception ex) when (ex is CommunicationException || ex is IOException || ex is TimeoutException || ex is WebException)
            {
                string message = "Failed to establish connection to the Importer service on Adresse";
                log.Error(message, ex);

                throw new ServiceNotFoundException(message, ex);
            }
        }

        // this method only performs a call to Ret on Organisation if there are actual changes
        public void Ret(string uuid, string newValue, string newShortKey, DateTime timestamp, RegistreringType1 registration)
        {
            log.Debug("Attempting Ret on Address with uuid " + uuid);

            SecurityToken token = TokenCache.IssueToken(AdresseStubHelper.SERVICE);
            AdressePortType channel = StubUtil.CreateChannel<AdressePortType>(AdresseStubHelper.SERVICE, "Ret", helper.CreatePort(), token);

            try
            {
                RetInputType1 input = new RetInputType1();
                input.UUIDIdentifikator = uuid;
                input.AttributListe = registration.AttributListe;
                input.TilstandListe = registration.TilstandListe;
                input.RelationListe = registration.RelationListe;

                // compare latest property to the local object
                EgenskabType latestProperty = StubUtil.GetLatestProperty(input.AttributListe);
                if (latestProperty == null || !latestProperty.AdresseTekst.Equals(newValue) || (newShortKey != null && !latestProperty.BrugervendtNoegleTekst.Equals(newShortKey)))
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
                    newProperty.AdresseTekst = newValue;

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
                    log.Debug("No changes on Address, so returning without calling Organisation");
                    // if there are no changes to the attributes, we do not call the Organisation service
                    return;
                }

                // send Ret request
                retRequest request = new retRequest();
                request.AdresseRetRequest = new AdresseRetRequestType();
                request.AdresseRetRequest.RetInput = input;
                request.AdresseRetRequest.AuthorityContext = new AuthorityContextType();
                request.AdresseRetRequest.AuthorityContext.MunicipalityCVR = registry.Municipality;

                retResponse response = channel.ret(request);

                int statusCode = Int32.Parse(response.AdresseRetResponse.RetOutput.StandardRetur.StatusKode);
                if (statusCode != 20)
                {
                    string message = StubUtil.ConstructSoapErrorMessage(statusCode, "Ret", AdresseStubHelper.SERVICE, response.AdresseRetResponse.RetOutput.StandardRetur.FejlbeskedTekst);
                    log.Error(message);
                    throw new SoapServiceException(message);
                }

                log.Debug("Ret successful on Address with uuid " + uuid);
            }
            catch (Exception ex) when (ex is CommunicationException || ex is IOException || ex is TimeoutException || ex is WebException)
            {
                string message = "Failed to establish connection to the Ret service on Adresse";
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
            request.AdresseListeRequest = new AdresseListeRequestType();
            request.AdresseListeRequest.ListInput = listInput;
            request.AdresseListeRequest.AuthorityContext = new AuthorityContextType();
            request.AdresseListeRequest.AuthorityContext.MunicipalityCVR = registry.Municipality;

            SecurityToken token = TokenCache.IssueToken(AdresseStubHelper.SERVICE);
            AdressePortType channel = StubUtil.CreateChannel<AdressePortType>(AdresseStubHelper.SERVICE, "Laes", helper.CreatePort(), token);

            try
            {
                listResponse response = channel.list(request);

                int statusCode = Int32.Parse(response.AdresseListeResponse.ListOutput.StandardRetur.StatusKode);
                if (statusCode != 20)
                {
                    // note that statusCode 44 means that the object does not exists, so that is a valid response
                    log.Debug("Lookup on Adresse with uuid '" + uuid + "' failed with statuscode " + statusCode);
                    return null;
                }

                if (response.AdresseListeResponse.ListOutput.FiltreretOejebliksbillede.Length != 1)
                {
                    log.Warn("Lookup Adresse with uuid '" + uuid + "' returned multiple objects");
                    return null;
                }

                RegistreringType1[] resultSet = response.AdresseListeResponse.ListOutput.FiltreretOejebliksbillede[0].Registrering;
                if (resultSet.Length == 0)
                {
                    log.Warn("Adresse with uuid '" + uuid + "' exists, but has no registration");
                    return null;
                }

                RegistreringType1 result = null;
                if (resultSet.Length > 1)
                {
                    log.Warn("Adresse with uuid " + uuid + " has more than one registration when reading latest registration, this should never happen");

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
                string message = "Failed to establish connection to the Laes service on Adresse";
                log.Error(message, ex);
                throw new ServiceNotFoundException(message, ex);
            }
        }

        private void EnsureKeys(AddressData address)
        {
            address.Uuid = (address.Uuid != null) ? address.Uuid : IdUtil.GenerateUuid();
            address.ShortKey = (address.ShortKey != null) ? address.ShortKey : IdUtil.GenerateShortKey();
        }
    }
}