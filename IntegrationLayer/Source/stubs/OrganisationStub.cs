using IntegrationLayer.Organisation;
using System;
using System.IO;
using System.Net;
using System.ServiceModel;

namespace Organisation.IntegrationLayer
{
    internal class OrganisationStub
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private OrganisationStubHelper helper = new OrganisationStubHelper();
        private OrganisationRegistryProperties registry = OrganisationRegistryProperties.GetInstance();

        public void Ret(string overordnetUuid)
        {
            log.Debug("Attempting Ret on Organisation with uuid " + registry.MunicipalityOrganisationUUID);

            RegistreringType1 registration = GetLatestRegistration(registry.MunicipalityOrganisationUUID);
            if (registration == null)
            {
                log.Error("Cannot call Ret on Organisation with uuid " + registry.MunicipalityOrganisationUUID + " because it does not exist in Organisation");
                return;
            }

            VirkningType virkning = helper.GetVirkning(DateTime.Now);

            OrganisationPortType channel = StubUtil.CreateChannel<OrganisationPortType>(OrganisationStubHelper.SERVICE, "Ret", helper.CreatePort());

            try
            {
                bool changes = false;

                RetInputType1 input = new RetInputType1();
                input.UUIDIdentifikator = registry.MunicipalityOrganisationUUID;
                input.AttributListe = registration.AttributListe;
                input.TilstandListe = registration.TilstandListe;
                input.RelationListe = registration.RelationListe;
                
                if (registration.RelationListe.Overordnet != null)
                {
                    // we have an existing Overordnet, let us see if we need to change it
                    bool expired = false;
                    object endDate = registration.RelationListe.Overordnet.Virkning.TilTidspunkt.Item;

                    if (endDate != null && endDate is DateTime && DateTime.Compare(DateTime.Now, (DateTime)endDate) >= 0)
                    {
                        expired = true;
                    }

                    if (expired || !registration.RelationListe.Overordnet.ReferenceID.Item.Equals(overordnetUuid))
                    {
                        registration.RelationListe.Overordnet.ReferenceID = StubUtil.GetReference<UnikIdType>(overordnetUuid, ItemChoiceType.UUIDIdentifikator);
                        registration.RelationListe.Overordnet.Virkning = virkning;
                        changes = true;
                    }
                }
                else
                {
                    // no existing parent, so just create one
                    helper.AddOverordnetEnhed(overordnetUuid, virkning, registration);
                    changes = true;
                }

                // if no changes are made, we do not call the service
                if (!changes)
                {
                    log.Debug("Ret on Organisation with uuid " + registry.MunicipalityOrganisationUUID + " cancelled because of no changes");
                    return;
                }

                // send Ret request
                retRequest request = new retRequest();
                request.RetRequest1 = new RetRequestType();
                request.RetRequest1.RetInput = input;
                request.RetRequest1.AuthorityContext = new AuthorityContextType();
                request.RetRequest1.AuthorityContext.MunicipalityCVR = registry.Municipality;

                retResponse response = channel.ret(request);

                int statusCode = Int32.Parse(response.RetResponse1.RetOutput.StandardRetur.StatusKode);
                if (statusCode != 20)
                {
                    string message = StubUtil.ConstructSoapErrorMessage(statusCode, "Ret", OrganisationStubHelper.SERVICE, response.RetResponse1.RetOutput.StandardRetur.FejlbeskedTekst);
                    log.Error(message);
                    throw new SoapServiceException(message);
                }

                log.Debug("Ret succesful on Organisation with uuid " + registry.MunicipalityOrganisationUUID);
            }
            catch (Exception ex) when (ex is CommunicationException || ex is IOException || ex is TimeoutException || ex is WebException)
            {
                string message = "Failed to establish connection to the Ret service on Organisation";
                log.Error(message, ex);
                throw new ServiceNotFoundException(message, ex);
            }
        }

        public RegistreringType1 GetLatestRegistration(string uuid)
        {
            LaesInputType laesInput = new LaesInputType();
            laesInput.UUIDIdentifikator = uuid;

            laesRequest request = new laesRequest();
            request.LaesRequest1 = new LaesRequestType();
            request.LaesRequest1.LaesInput = laesInput;
            request.LaesRequest1.AuthorityContext = new AuthorityContextType();
            request.LaesRequest1.AuthorityContext.MunicipalityCVR = registry.Municipality;

            OrganisationPortType channel = StubUtil.CreateChannel<OrganisationPortType>(OrganisationStubHelper.SERVICE, "Laes", helper.CreatePort());

            try
            {
                laesResponse response = channel.laes(request);

                int statusCode = Int32.Parse(response.LaesResponse1.LaesOutput.StandardRetur.StatusKode);
                if (statusCode != 20)
                {
                    // note that statusCode 44 means that the object does not exists, so that is a valid response
                    log.Debug("Lookup on Organisation with uuid '" + uuid + "' failed with statuscode " + statusCode);
                    return null;
                }

                RegistreringType1[] resultSet = response.LaesResponse1.LaesOutput.FiltreretOejebliksbillede.Registrering;
                if (resultSet.Length == 0)
                {
                    log.Warn("Organisation with uuid '" + uuid + "' exists, but has no registration");
                    return null;
                }

                RegistreringType1 result = null;
                if (resultSet.Length > 1)
                {
                    log.Warn("Organisation with uuid " + uuid + " has more than one registration when reading latest registration, this should never happen");

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

                // we cannot perform any kind of updates on Slettet/Passiveret, så it makes sense to filter them out on lookup,
                // so the rest of the code will default to Import op top of this
                if (result.LivscyklusKode.Equals(LivscyklusKodeType.Slettet) || result.LivscyklusKode.Equals(LivscyklusKodeType.Passiveret))
                {
                    return null;
                }

                return result;
            }
            catch (Exception ex) when (ex is CommunicationException || ex is IOException || ex is TimeoutException || ex is WebException)
            {
                string message = "Failed to establish connection to the Laes service on Organisation";
                log.Error(message, ex);
                throw new ServiceNotFoundException(message, ex);
            }
        }
    }
}
