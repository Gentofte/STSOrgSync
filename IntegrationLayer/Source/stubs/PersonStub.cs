﻿using System;
using IntegrationLayer.Person;
using System.ServiceModel;
using System.IO;
using System.Net;
using System.Collections.Generic;

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
            request.ImporterRequest1 = new ImporterRequestType();
            request.ImporterRequest1.ImportInput = importInput;
            request.ImporterRequest1.AuthorityContext = new AuthorityContextType();
            request.ImporterRequest1.AuthorityContext.MunicipalityCVR = OrganisationRegistryProperties.GetMunicipality();

            // send request
            PersonPortType channel = StubUtil.CreateChannel<PersonPortType>(PersonStubHelper.SERVICE,  "Import", helper.CreatePort());

            try
            {
                importerResponse response = channel.importer(request);
                int statusCode = Int32.Parse(response.ImporterResponse1.ImportOutput.StandardRetur.StatusKode);
                if (statusCode != 20)
                {
                    string message = StubUtil.ConstructSoapErrorMessage(statusCode, "Import", PersonStubHelper.SERVICE, response.ImporterResponse1.ImportOutput.StandardRetur.FejlbeskedTekst);
                    log.Error(message);
                    throw new SoapServiceException(message);
                }

                log.Debug("Import successful on Person with uuid " + person.Uuid);
            }
            catch (Exception ex) when (ex is CommunicationException || ex is IOException || ex is TimeoutException || ex is WebException)
            {
                throw new ServiceNotFoundException("Failed to establish connection to the Importer service on Person", ex);
            }
        }

        // this method only performs a call to Ret on Organisation if there are actual changes
        public void Ret(string uuid, string newName, string newShortKey, string newCpr, DateTime timestamp)
        {
            log.Debug("Attempting Ret on Person with uuid " + uuid);

            // this should never fail - we always import the Person before the User, and we have a User with a reference to this object
            RegistreringType1 registration = GetLatestRegistration(uuid);
            if (registration == null)
            {
                log.Debug("Cannot call Ret on Person with uuid " + uuid + " because it does not exist in Organisation");
                return;
            }

            PersonPortType channel = StubUtil.CreateChannel<PersonPortType>(PersonStubHelper.SERVICE, "Ret", helper.CreatePort());

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
                    // create a new property
                    EgenskabType newProperty = new EgenskabType();
                    newProperty.Virkning = helper.GetVirkning(timestamp);
                    newProperty.BrugervendtNoegleTekst = ((newShortKey != null) ? newShortKey : latestProperty.BrugervendtNoegleTekst);
                    newProperty.NavnTekst = newName;
                    newProperty.CPRNummerTekst = newCpr;

                    // overwrite existing property set
                    input.AttributListe = new EgenskabType[1];
                    input.AttributListe[0] = newProperty;
                }
                else
                {
                    log.Debug("No changes on Person, so returning without calling Organisation");

                    // if there are no changes to the attributes, we do not call the Organisation service
                    return;
                }

                // send Ret request
                retRequest request = new retRequest();
                request.RetRequest1 = new RetRequestType();
                request.RetRequest1.RetInput = input;
                request.RetRequest1.AuthorityContext = new AuthorityContextType();
                request.RetRequest1.AuthorityContext.MunicipalityCVR = OrganisationRegistryProperties.GetMunicipality();

                retResponse response = channel.ret(request);

                int statusCode = Int32.Parse(response.RetResponse1.RetOutput.StandardRetur.StatusKode);
                if (statusCode != 20)
                {
                    string message = StubUtil.ConstructSoapErrorMessage(statusCode, "Ret", PersonStubHelper.SERVICE, response.RetResponse1.RetOutput.StandardRetur.FejlbeskedTekst);
                    log.Error(message);
                    throw new SoapServiceException(message);
                }

                log.Debug("Ret successful on Person with uuid " + uuid);
            }
            catch (Exception ex) when (ex is CommunicationException || ex is IOException || ex is TimeoutException || ex is WebException)
            {
                throw new ServiceNotFoundException("Failed to establish connection to the Ret service on Person", ex);
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
            request.LaesRequest1.AuthorityContext.MunicipalityCVR = OrganisationRegistryProperties.GetMunicipality();

            PersonPortType channel = StubUtil.CreateChannel<PersonPortType>(PersonStubHelper.SERVICE, "Laes", helper.CreatePort());

            try
            {
                laesResponse response = channel.laes(request);

                int statusCode = Int32.Parse(response.LaesResponse1.LaesOutput.StandardRetur.StatusKode);
                if (statusCode != 20)
                {
                    // note that statusCode 44 means that the object does not exists, so that is a valid response
                    log.Debug("Lookup on Person with uuid '" + uuid + "' failed with statuscode " + statusCode);
                    return null; 
                }

                RegistreringType1[] resultSet = response.LaesResponse1.LaesOutput.FiltreretOejebliksbillede.Registrering;
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
                throw new ServiceNotFoundException("Failed to establish connection to the Laes service on Person", ex);
            }
        }

        public Dictionary<string, RegistreringType1> GetLatestRegistrations(List<string> uuids)
        {
            var result = new Dictionary<string, RegistreringType1>();

            ListInputType listInput = new ListInputType();
            listInput.UUIDIdentifikator = uuids.ToArray();

            listRequest request = new listRequest();
            request.ListRequest1 = new ListRequestType();
            request.ListRequest1.ListInput = listInput;
            request.ListRequest1.AuthorityContext = new AuthorityContextType();
            request.ListRequest1.AuthorityContext.MunicipalityCVR = OrganisationRegistryProperties.GetMunicipality();

            PersonPortType channel = StubUtil.CreateChannel<PersonPortType>(PersonStubHelper.SERVICE, "List", helper.CreatePort());

            try
            {
                listResponse response = channel.list(request);

                int statusCode = Int32.Parse(response.ListResponse1.ListOutput.StandardRetur.StatusKode);
                if (statusCode != 20)
                {
                    // note that statusCode 44 means that the object does not exists, so that is a valid response
                    log.Debug("List on Person failed with statuscode " + statusCode);
                    return result;
                }

                if (response.ListResponse1.ListOutput.FiltreretOejebliksbillede == null || response.ListResponse1.ListOutput.FiltreretOejebliksbillede.Length == 0)
                {
                    log.Debug("List on Person has 0 hits");
                    return result;
                }

                foreach (var person in response.ListResponse1.ListOutput.FiltreretOejebliksbillede)
                {
                    RegistreringType1[] resultSet = person.Registrering;
                    if (resultSet.Length == 0)
                    {
                        log.Warn("Person with uuid '" + person.ObjektType.UUIDIdentifikator + "' exists, but has no registration");
                        continue;
                    }

                    RegistreringType1 reg = null;
                    if (resultSet.Length > 1)
                    {
                        log.Warn("Person with uuid " + person.ObjektType.UUIDIdentifikator + " has more than one registration when reading latest registration, this should never happen");

                        DateTime winner = DateTime.MinValue;
                        foreach (RegistreringType1 res in resultSet)
                        {
                            // first time through will always result in a True evaluation here
                            if (DateTime.Compare(winner, res.Tidspunkt) < 0)
                            {
                                reg = res;
                                winner = res.Tidspunkt;
                            }
                        }
                    }
                    else
                    {
                        reg = resultSet[0];
                    }

                    // we cannot perform any kind of updates on Slettet/Passiveret, så it makes sense to filter them out on lookup,
                    // so the rest of the code will default to Import op top of this
                    if (reg.LivscyklusKode.Equals(LivscyklusKodeType.Slettet) || reg.LivscyklusKode.Equals(LivscyklusKodeType.Passiveret))
                    {
                        continue;
                    }

                    result.Add(person.ObjektType.UUIDIdentifikator, reg);
                }

                return result;
            }
            catch (Exception ex) when (ex is CommunicationException || ex is IOException || ex is TimeoutException || ex is WebException)
            {
                throw new ServiceNotFoundException("Failed to establish connection to the List service on Person", ex);
            }
        }

        private void EnsureKeys(PersonData person)
        {
            person.Uuid = (person.Uuid != null) ? person.Uuid : IdUtil.GenerateUuid();
            person.ShortKey = (person.ShortKey != null) ? person.ShortKey : IdUtil.GenerateShortKey();
        }
    }
}
