using System;
using System.Collections.Generic;
using Organisation.IntegrationLayer;
using IntegrationLayer.OrganisationFunktion;
using System.Linq;

namespace Organisation.BusinessLayer
{
    public enum ReadAddresses { YES, NO };
    public enum ReadParentDetails { YES, NO };
    public enum ReadPayoutUnit { YES, NO };
    public enum ReadPositions { YES, NO };
    public enum ReadItSystems { YES, NO };
    public enum ReadContactPlaces { YES, NO };

    public class InspectorService
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private BrugerStub brugerStub = new BrugerStub();
        private OrganisationFunktionStub orgFunctionStub = new OrganisationFunktionStub();
        private AdresseStub adresseStub = new AdresseStub();
        private PersonStub personStub = new PersonStub();
        private OrganisationEnhedStub organisationEnhedStub = new OrganisationEnhedStub();
        private OrganisationSystemStub organisationSystemStub = new OrganisationSystemStub();
        private OrganisationFunktionStub organisationFunktionStub = new OrganisationFunktionStub();

        public string ReadUserRaw(string uuid)
        {
            var registration = brugerStub.GetLatestRegistration(uuid);
            if (registration == null)
            {
                throw new RegistrationNotFoundException("Could not locate User with uuid '" + uuid + "'");
            }

            return XmlUtil.SerializeObject(registration);
        }

        public string ReadFunctionRaw(string uuid)
        {
            var registration = orgFunctionStub.GetLatestRegistration(uuid);
            if (registration == null)
            {
                throw new RegistrationNotFoundException("Could not locate function with uuid '" + uuid + "'");
            }

            return XmlUtil.SerializeObject(registration);
        }

        public string ReadOURaw(string uuid)
        {
            var registration = organisationEnhedStub.GetLatestRegistration(uuid);
            if (registration == null)
            {
                throw new RegistrationNotFoundException("Could not locate OU with uuid '" + uuid + "'");
            }

            return XmlUtil.SerializeObject(registration);
        }

        public string ReadPersonRaw(string uuid)
        {
            var registration = personStub.GetLatestRegistration(uuid);
            if (registration == null)
            {
                throw new RegistrationNotFoundException("Could not locate Person with uuid '" + uuid + "'");
            }

            return XmlUtil.SerializeObject(registration);
        }

        public string ReadAddressRaw(string uuid)
        {
            var registration = adresseStub.GetLatestRegistration(uuid);
            if (registration == null)
            {
                throw new RegistrationNotFoundException("Could not locate Address with uuid '" + uuid + "'");
            }

            return XmlUtil.SerializeObject(registration);
        }

        public Person ReadPersonObject(string uuid)
        {
            global::IntegrationLayer.Person.RegistreringType1 registration = personStub.GetLatestRegistration(uuid);
            if (registration == null)
            {
                throw new RegistrationNotFoundException("Could not locate Person with uuid '" + uuid + "'");
            }
            global::IntegrationLayer.Person.EgenskabType property = StubUtil.GetLatestProperty(registration.AttributListe);

            string cpr = (property != null) ? property.CPRNummerTekst : null;
            string shortKey = (property != null) ? property.BrugervendtNoegleTekst : null;
            string name = (property != null) ? property.NavnTekst : null;

            return new Person()
            {
                Name = name,
                ShortKey = shortKey,
                Uuid = uuid,
                Cpr = cpr
            };
        }

        public Function ReadFunctionObject(string uuid)
        {
            global::IntegrationLayer.OrganisationFunktion.RegistreringType1 registration = orgFunctionStub.GetLatestRegistration(uuid);
            if (registration == null)
            {
                throw new RegistrationNotFoundException("Could not locate Function with uuid '" + uuid + "'");
            }
            global::IntegrationLayer.OrganisationFunktion.EgenskabType property = StubUtil.GetLatestProperty(registration.AttributListe.Egenskab);

            string shortKey = property.BrugervendtNoegleTekst;
            string name = property.FunktionNavn;

            // TODO: perhaps map this to known types?
            string functionType = registration.RelationListe?.Funktionstype?.ReferenceID?.Item;

            // TODO: depending on type, find relevant relations

            Status status = Status.ACTIVE;
            var latestState = StubUtil.GetLatestGyldighed(registration.TilstandListe.Gyldighed);
            if (latestState == null)
            {
                status = Status.UNKNOWN;
            }
            else if (global::IntegrationLayer.OrganisationFunktion.GyldighedStatusKodeType.Inaktiv.Equals(latestState.GyldighedStatusKode))
            {
                status = Status.INACTIVE;
            }

            return new Function()
            {
                Uuid = uuid,
                Name = name,
                ShortKey = shortKey,
                FunctionType = functionType,
                Status = status
            };
        }

        public AddressHolder ReadAddressObject(string uuid)
        {
            global::IntegrationLayer.Adresse.RegistreringType1 registration = adresseStub.GetLatestRegistration(uuid);
            if (registration == null)
            {
                throw new RegistrationNotFoundException("Could not locate Address with uuid '" + uuid + "'");
            }
            global::IntegrationLayer.Adresse.EgenskabType property = StubUtil.GetLatestProperty(registration.AttributListe);

            string shortKey = (property != null) ? property.BrugervendtNoegleTekst : null;
            string value = (property != null) ? property.AdresseTekst : null;

            return new AnonymousAddress()
            {                
                Uuid = uuid,
                Value = value
            };
        }

        public void FindAllUsersInOU(OU ou, List<User> users)
        {
            List<User> newUsers = new List<User>();

            var orgFunctionRegistrations = orgFunctionStub.SoegAndGetLatestRegistration(UUIDConstants.ORGFUN_POSITION, null, ou.Uuid, null);

            if (orgFunctionRegistrations.Count == 0)
            {
                return;
            }

            // extract relevant references into partial user objects
            foreach (var orgFunctionRegistration in orgFunctionRegistrations)
            {
                string orgFunctionName = "Ukendt";

                if (orgFunctionRegistration.Registrering != null && orgFunctionRegistration.Registrering.Length > 0)
                {
                    if (orgFunctionRegistration.Registrering.Length > 1)
                    {
                        log.Warn("More than one registration in output for function: " + orgFunctionRegistration.ObjektType.UUIDIdentifikator);
                    }

                    var orgFunctionProperty = StubUtil.GetLatestProperty(orgFunctionRegistration.Registrering[0].AttributListe.Egenskab);
                    if (orgFunctionProperty != null)
                    {
                        orgFunctionName = orgFunctionProperty.FunktionNavn;
                    }

                    if (orgFunctionRegistration.Registrering[0].RelationListe.TilknyttedeBrugere != null && orgFunctionRegistration.Registrering[0].RelationListe.TilknyttedeBrugere.Length > 0)
                    {
                        // the registration pattern allows for multiple users to share an OrgFunction
                        foreach (var bruger in orgFunctionRegistration.Registrering[0].RelationListe.TilknyttedeBrugere)
                        {
                            bool found = false;
                            User user = new User();

                            // see if we have the user already and throw away the default new user in that case
                            foreach (User u in users)
                            {
                                if (u.Uuid.Equals(bruger.ReferenceID.Item))
                                {
                                    user = u;
                                    found = true;
                                    break;
                                }
                            }

                            if (!found)
                            {
                                user.Uuid = bruger.ReferenceID.Item;
                                user.Positions = new List<Position>();
                                users.Add(user);
                                newUsers.Add(user);
                            }

                            user.Positions.Add(new Position()
                            {
                                Name = orgFunctionName,
                                OU = new OUReference()
                                {
                                    Name = ou.Name,
                                    Uuid = ou.Uuid
                                },
                                User = new UserReference()
                                {
                                    Uuid = user.Uuid
                                }
                            });
                        }
                    }
                }
            }

            var personUuids = new List<string>();
            var emailUuids = new List<string>();
            var telephoneUuids = new List<string>();

            // enrich users with User object details
            var userRegistrations = brugerStub.GetLatestRegistrations(newUsers.Select(u => u.Uuid).ToList());
            foreach (var user in newUsers)
            {
                user.Addresses = new List<AddressHolder>();
                user.Person = new Person();

                foreach (var registration in userRegistrations)
                {
                    if (user.Uuid.Equals(registration.Key))
                    {
                        var properties = StubUtil.GetLatestProperty(registration.Value.AttributListe.Egenskab);
                        if (properties != null)
                        {
                            user.UserId = properties.BrugerNavn;
                        }

                        if (registration.Value.RelationListe.Adresser != null)
                        {
                            foreach (var address in registration.Value.RelationListe.Adresser)
                            {
                                if (UUIDConstants.ADDRESS_ROLE_USER_EMAIL.Equals(address.Rolle.Item))
                                {
                                    emailUuids.Add(address.ReferenceID.Item);

                                    user.Addresses.Add(new Email()
                                    {
                                        Uuid = address.ReferenceID.Item                                        
                                    });
                                }
                                else if (UUIDConstants.ADDRESS_ROLE_USER_PHONE.Equals(address.Rolle.Item))
                                {
                                    telephoneUuids.Add(address.ReferenceID.Item);

                                    user.Addresses.Add(new Phone()
                                    {
                                        Uuid = address.ReferenceID.Item
                                    });
                                }
                            }
                        }

                        if (registration.Value.RelationListe.TilknyttedePersoner != null && registration.Value.RelationListe.TilknyttedePersoner.Length > 0)
                        {
                            user.Person.Uuid = registration.Value.RelationListe.TilknyttedePersoner[0].ReferenceID.Item;

                            personUuids.Add(registration.Value.RelationListe.TilknyttedePersoner[0].ReferenceID.Item);
                        }

                        break;
                    }
                }
            }

            // enrich with email address information
            if (emailUuids.Count > 0)
            {
                var emailRegistrations = adresseStub.GetLatestRegistrations(emailUuids);
                foreach (var user in newUsers)
                {
                    foreach (var address in user.Addresses)
                    {
                        foreach (var emailRegistration in emailRegistrations)
                        {
                            if (emailRegistration.Key.Equals(address.Uuid))
                            {
                                var properties = StubUtil.GetLatestProperty(emailRegistration.Value.AttributListe);
                                if (properties != null)
                                {
                                    address.Value = properties.AdresseTekst;
                                }

                                break;
                            }
                        }
                    }
                }
            }

            // enrich with telephone address information
            if (telephoneUuids.Count > 0)
            {
                var phoneRegistrations = adresseStub.GetLatestRegistrations(telephoneUuids);
                foreach (var user in newUsers)
                {
                    foreach (var address in user.Addresses)
                    {
                        foreach (var phoneRegistration in phoneRegistrations)
                        {
                            if (phoneRegistration.Key.Equals(address.Uuid))
                            {
                                var properties = StubUtil.GetLatestProperty(phoneRegistration.Value.AttributListe);
                                if (properties != null)
                                {
                                    address.Value = properties.AdresseTekst;
                                }

                                break;
                            }
                        }
                    }
                }
            }

            // enrich with person information
            var personRegistrations = personStub.GetLatestRegistrations(personUuids);
            foreach (var user in newUsers)
            {
                foreach (var personRegistration in personRegistrations)
                {
                    if (personRegistration.Key.Equals(user.Person?.Uuid))
                    {
                        var properties = StubUtil.GetLatestProperty(personRegistration.Value.AttributListe);
                        if (properties != null)
                        {
                            user.Person.Name = properties.NavnTekst;
                        }

                        break;
                    }
                }
            }
        }

        public OU ReadOUObject(string uuid, ReadAddresses readAddress = ReadAddresses.YES, ReadParentDetails readParentDetails = ReadParentDetails.YES, ReadPayoutUnit readPayoutUnit = ReadPayoutUnit.YES, ReadPositions readPositions = ReadPositions.YES, ReadItSystems readItSystems = ReadItSystems.YES, ReadContactPlaces readContactPlaces = ReadContactPlaces.YES)
        {
            global::IntegrationLayer.OrganisationEnhed.RegistreringType1 registration = organisationEnhedStub.GetLatestRegistration(uuid);
            if (registration == null)
            {
                throw new RegistrationNotFoundException("Could not locate OU with uuid '" + uuid + "'");
            }

            return MapRegistrationToOU(registration, uuid, readAddress, readParentDetails, readPayoutUnit, readPositions, readItSystems, readContactPlaces);
        }

        private OU MapRegistrationToOU(dynamic registration, string uuid, ReadAddresses readAddress = ReadAddresses.YES, ReadParentDetails readParentDetails = ReadParentDetails.YES, ReadPayoutUnit readPayoutUnit = ReadPayoutUnit.YES, ReadPositions readPositions = ReadPositions.YES, ReadItSystems readItSystems = ReadItSystems.YES, ReadContactPlaces readContactPlaces = ReadContactPlaces.YES)
        {
            DateTime timestamp = registration.Tidspunkt;

            var property = StubUtil.GetLatestProperty(registration.AttributListe.Egenskab);

            string ouName = (property != null) ? property.EnhedNavn : null;
            string ouShortKey = (property != null) ? property.BrugervendtNoegleTekst : null;

            List<AddressHolder> addresses = new List<AddressHolder>();

            if (readAddress.Equals(ReadAddresses.YES) && registration.RelationListe?.Adresser != null)
            {
                foreach (var address in registration.RelationListe.Adresser)
                {
                    string addressUuid = address.ReferenceID.Item;
                    global::IntegrationLayer.Adresse.RegistreringType1 addressRegistration = adresseStub.GetLatestRegistration(addressUuid);

                    string addressValue = "The address object does not exist in Organisation";
                    string addressShortKey = "";
                    if (addressRegistration != null)
                    {
                        global::IntegrationLayer.Adresse.EgenskabType addressProperty = StubUtil.GetLatestProperty(addressRegistration.AttributListe);
                        if (addressProperty != null)
                        {
                            addressValue = addressProperty.AdresseTekst;
                            addressShortKey = addressProperty.BrugervendtNoegleTekst;
                        }
                    }

                    if (address.Rolle.Item.Equals(UUIDConstants.ADDRESS_ROLE_ORGUNIT_EMAIL))
                    {
                        addresses.Add(new Email()
                        {
                            Uuid = addressUuid,
                            ShortKey = addressShortKey,
                            Value = addressValue
                        });
                    }
                    else if (address.Rolle.Item.Equals(UUIDConstants.ADDRESS_ROLE_ORGUNIT_LOCATION))
                    {
                        addresses.Add(new Location()
                        {
                            Uuid = addressUuid,
                            ShortKey = addressShortKey,
                            Value = addressValue
                        });
                    }
                    else if (address.Rolle.Item.Equals(UUIDConstants.ADDRESS_ROLE_ORGUNIT_PHONE))
                    {
                        addresses.Add(new Phone()
                        {
                            Uuid = addressUuid,
                            ShortKey = addressShortKey,
                            Value = addressValue
                        });
                    }
                    else if (address.Rolle.Item.Equals(UUIDConstants.ADDRESS_ROLE_ORGUNIT_LOSSHORTNAME))
                    {
                        addresses.Add(new LOSShortName()
                        {
                            Uuid = addressUuid,
                            ShortKey = addressShortKey,
                            Value = addressValue
                        });
                    }
                    else if (address.Rolle.Item.Equals(UUIDConstants.ADDRESS_ROLE_ORGUNIT_EAN))
                    {
                        addresses.Add(new Ean()
                        {
                            Uuid = addressUuid,
                            ShortKey = addressShortKey,
                            Value = addressValue
                        });
                    }
                    else if (address.Rolle.Item.Equals(UUIDConstants.ADDRESS_ROLE_ORGUNIT_CONTACT_ADDRESS_OPEN_HOURS))
                    {
                        addresses.Add(new ContactHours()
                        {
                            Uuid = addressUuid,
                            ShortKey = addressShortKey,
                            Value = addressValue
                        });
                    }
                    else if (address.Rolle.Item.Equals(UUIDConstants.ADDRESS_ROLE_ORGUNIT_EMAIL_REMARKS))
                    {
                        addresses.Add(new EmailRemarks()
                        {
                            Uuid = addressUuid,
                            ShortKey = addressShortKey,
                            Value = addressValue
                        });
                    }
                    else if (address.Rolle.Item.Equals(UUIDConstants.ADDRESS_ROLE_ORGUNIT_POST_RETURN))
                    {
                        addresses.Add(new PostReturn()
                        {
                            Uuid = addressUuid,
                            ShortKey = addressShortKey,
                            Value = addressValue
                        });
                    }
                    else if (address.Rolle.Item.Equals(UUIDConstants.ADDRESS_ROLE_ORGUNIT_CONTACT_ADDRESS))
                    {
                        addresses.Add(new Contact()
                        {
                            Uuid = addressUuid,
                            ShortKey = addressShortKey,
                            Value = addressValue
                        });
                    }
                    else if (address.Rolle.Item.Equals(UUIDConstants.ADDRESS_ROLE_ORGUNIT_POST))
                    {
                        addresses.Add(new Post()
                        {
                            Uuid = addressUuid,
                            ShortKey = addressShortKey,
                            Value = addressValue
                        });
                    }
                    else if (address.Rolle.Item.Equals(UUIDConstants.ADDRESS_ROLE_ORGUNIT_PHONE_OPEN_HOURS))
                    {
                        addresses.Add(new PhoneHours()
                        {
                            Uuid = addressUuid,
                            ShortKey = addressShortKey,
                            Value = addressValue
                        });
                    }
                }
            }

            OUReference parentOU = null;
            if (registration.RelationListe?.Overordnet != null)
            {
                string parentOUUuid = registration.RelationListe.Overordnet.ReferenceID.Item;
                string parentOUName = "";

                if (readParentDetails.Equals(ReadParentDetails.YES))
                {
                    var parentRegistration = organisationEnhedStub.GetLatestRegistration(parentOUUuid);
                    if (parentRegistration != null)
                    {
                        var parentProperty = StubUtil.GetLatestProperty(parentRegistration.AttributListe.Egenskab);
                        if (parentProperty != null)
                        {
                            parentOUName = parentProperty.EnhedNavn;
                        }
                    }
                }

                parentOU = new OUReference()
                {
                    Name = parentOUName,
                    Uuid = parentOUUuid
                };
            }

            OUReference payoutOU = null;
            List<ContactPlace> contactPlaces = new List<ContactPlace>();
            if (readPayoutUnit.Equals(ReadPayoutUnit.YES) || readContactPlaces.Equals(ReadContactPlaces.YES))
            {
                if (registration.RelationListe?.TilknyttedeFunktioner != null)
                {
                    foreach (var function in registration.RelationListe.TilknyttedeFunktioner)
                    {
                        var functionState = orgFunctionStub.GetLatestRegistration(function.ReferenceID.Item);

                        if (functionState.RelationListe.Funktionstype != null)
                        {
                            if (readPayoutUnit.Equals(ReadPayoutUnit.YES) && functionState.RelationListe.Funktionstype.ReferenceID.Item.Equals(UUIDConstants.ORGFUN_PAYOUT_UNIT))
                            {
                                if (functionState.RelationListe.TilknyttedeEnheder != null && functionState.RelationListe.TilknyttedeEnheder.Length > 0)
                                {
                                    string payoutUnitUuid = functionState.RelationListe.TilknyttedeEnheder[0].ReferenceID.Item;
                                    string payoutUnitName = "The payout unit does not exist in Organisation";

                                    var payoutUnitRegistration = organisationEnhedStub.GetLatestRegistration(payoutUnitUuid);
                                    if (payoutUnitRegistration != null)
                                    {
                                        var payoutUnitProperty = StubUtil.GetLatestProperty(payoutUnitRegistration.AttributListe.Egenskab);

                                        if (payoutUnitProperty != null)
                                        {
                                            payoutUnitName = payoutUnitProperty.EnhedNavn;
                                        }
                                    }

                                    payoutOU = new OUReference()
                                    {
                                        Name = payoutUnitName,
                                        Uuid = payoutUnitUuid
                                    };
                                }
                            }
                            else if (readContactPlaces.Equals(ReadContactPlaces.YES) && functionState.RelationListe.Funktionstype.ReferenceID.Item.Equals(UUIDConstants.ORGFUN_CONTACT_UNIT))
                            {
                                List<string> tasks = new List<string>();
                                OUReference contactUnit = new OUReference();

                                if (functionState.RelationListe.Opgaver != null && functionState.RelationListe.Opgaver.Length > 0)
                                {
                                    foreach (var opgave in functionState.RelationListe.Opgaver)
                                    {
                                        tasks.Add(opgave.ReferenceID.Item);
                                    }
                                }

                                if (functionState.RelationListe.TilknyttedeEnheder != null && functionState.RelationListe.TilknyttedeEnheder.Length > 0)
                                {
                                    contactUnit.Uuid = functionState.RelationListe.TilknyttedeEnheder[0].ReferenceID.Item;
                                    contactUnit.Name = "The contact unit does not exist in Organisation";

                                    /*global::IntegrationLayer.OrganisationEnhed.RegistreringType1*/ var payoutUnitRegistration = organisationEnhedStub.GetLatestRegistration(contactUnit.Uuid);
                                    if (payoutUnitRegistration != null)
                                    {
                                        /*global::IntegrationLayer.OrganisationEnhed.EgenskabType*/ var payoutUnitProperty = StubUtil.GetLatestProperty(payoutUnitRegistration.AttributListe.Egenskab);

                                        if (payoutUnitProperty != null)
                                        {
                                            contactUnit.Name = payoutUnitProperty.EnhedNavn;
                                        }
                                    }
                                }

                                contactPlaces.Add(new ContactPlace()
                                {
                                    OrgUnit = contactUnit,
                                    Tasks = tasks
                                });
                            }
                        }
                    }
                }
            }

            List<Position> positions = new List<Position>();
            if (readPositions.Equals(ReadPositions.YES))
            {
                List<string> unitRoles = ServiceHelper.FindUnitRolesForOrgUnit(uuid);
                foreach (string positionUuid in unitRoles)
                {
                    string orgFunctionName = "OrgFunction object does not exist in Organisation";
                    string orgFunctionShortKey = "OrgFunction object does not exist in Organisation";
                    UserReference user = new UserReference()
                    {
                        Uuid = null
                    };

                    // only pick the first one
                    var orgFunctionRegistration = orgFunctionStub.GetLatestRegistration(positionUuid);
                    if (orgFunctionRegistration != null)
                    {
                        var orgFunctionProperty = StubUtil.GetLatestProperty(orgFunctionRegistration.AttributListe.Egenskab);

                        if (orgFunctionProperty != null)
                        {
                            orgFunctionName = orgFunctionProperty.FunktionNavn;
                            orgFunctionShortKey = orgFunctionProperty.BrugervendtNoegleTekst;
                        }

                        if (orgFunctionRegistration.RelationListe.TilknyttedeBrugere != null && orgFunctionRegistration.RelationListe.TilknyttedeBrugere.Length > 0)
                        {
                            // the registration pattern allows for multiple users to share an OrgFunction
                            foreach (var bruger in orgFunctionRegistration.RelationListe.TilknyttedeBrugere)
                            {
                                string brugerUuid = bruger.ReferenceID.Item;
                                user.Uuid = brugerUuid;

                                Position position = new Position()
                                {
                                    Name = orgFunctionName,
                                    User = user,
                                    ShortKey = orgFunctionShortKey,
                                    Uuid = positionUuid
                                };

                                positions.Add(position);
                            }
                        }
                    }
                }
            }

            List<string> itSystems = new List<string>();
            if (readItSystems.Equals(ReadItSystems.YES))
            {
                itSystems = ServiceHelper.FindItSystemsForOrgUnit(uuid);
            }

            Status status = Status.UNKNOWN;
            if (registration.TilstandListe.Gyldighed != null)
            {
                var latestState = StubUtil.GetLatestGyldighed(registration.TilstandListe.Gyldighed);
                if (latestState == null)
                {
                    status = Status.UNKNOWN;
                }
                else if (global::IntegrationLayer.OrganisationEnhed.GyldighedStatusKodeType.Inaktiv.Equals(latestState.GyldighedStatusKode))
                {
                    status = Status.INACTIVE;
                }
                else
                {
                    status = Status.ACTIVE;
                }
            }

            return new OU()
            {
                Name = ouName,
                ShortKey = ouShortKey,
                Uuid = uuid,
                ParentOU = parentOU,
                Positions = positions,
                PayoutOU = payoutOU,
                Addresses = addresses,
                ItSystems = itSystems,
                ContactPlaces = contactPlaces,
                Status = status,
                Timestamp = timestamp
            };
        }

        public User ReadUserObject(string uuid, ReadAddresses readAddresses = ReadAddresses.YES, ReadParentDetails readParentDetails = ReadParentDetails.YES)
        {
            global::IntegrationLayer.Bruger.RegistreringType1 registration = brugerStub.GetLatestRegistration(uuid);
            if (registration == null)
            {
                throw new RegistrationNotFoundException("Could not locate User with uuid '" + uuid + "'");
            }

            DateTime timestamp = registration.Tidspunkt;

            global::IntegrationLayer.Bruger.EgenskabType property = StubUtil.GetLatestProperty(registration.AttributListe.Egenskab);

            string userId = (property != null) ? property.BrugerNavn : null;
            string userShortKey = (property != null) ? property.BrugervendtNoegleTekst : null;

            List<AddressHolder> addresses = new List<AddressHolder>();
            if (readAddresses.Equals(ReadAddresses.YES))
            {
                if (registration.RelationListe?.Adresser != null)
                {
                    foreach (global::IntegrationLayer.Bruger.AdresseFlerRelationType address in registration.RelationListe.Adresser)
                    {
                        string addressUuid = address.ReferenceID.Item;
                        global::IntegrationLayer.Adresse.RegistreringType1 addressRegistration = adresseStub.GetLatestRegistration(addressUuid);

                        string addressValue = "The address object does not exist in Organisation";
                        string addressShortKey = "";
                        if (addressRegistration != null)
                        {
                            global::IntegrationLayer.Adresse.EgenskabType addressProperty = StubUtil.GetLatestProperty(addressRegistration.AttributListe);

                            if (addressProperty != null)
                            {
                                addressValue = addressProperty.AdresseTekst;
                                addressShortKey = addressProperty.BrugervendtNoegleTekst;
                            }
                        }

                        if (address.Rolle.Item.Equals(UUIDConstants.ADDRESS_ROLE_USER_EMAIL))
                        {
                            addresses.Add(new Email()
                            {
                                Uuid = addressUuid,
                                ShortKey = addressShortKey,
                                Value = addressValue
                            });
                        }
                        else if (address.Rolle.Item.Equals(UUIDConstants.ADDRESS_ROLE_USER_LOCATION))
                        {
                            addresses.Add(new Location()
                            {
                                Uuid = addressUuid,
                                ShortKey = addressShortKey,
                                Value = addressValue
                            });
                        }
                        else if (address.Rolle.Item.Equals(UUIDConstants.ADDRESS_ROLE_USER_PHONE))
                        {
                            addresses.Add(new Phone()
                            {
                                Uuid = addressUuid,
                                ShortKey = addressShortKey,
                                Value = addressValue
                            });
                        }
                    }
                }
            }

            Person person = null;
            if (registration.RelationListe?.TilknyttedePersoner != null && registration.RelationListe?.TilknyttedePersoner.Length > 0)
            {
                global::IntegrationLayer.Bruger.PersonFlerRelationType personRelation = registration.RelationListe.TilknyttedePersoner[0];
                string personUuid = personRelation.ReferenceID.Item;

                global::IntegrationLayer.Person.RegistreringType1 personRegistration = personStub.GetLatestRegistration(personUuid);
                string personName = "The person object does not exist in Organisation";
                string personShortKey = "The person object does not exist in Organisation";
                string personCpr = "The person object does not exist in Organisation";

                if (personRegistration != null)
                {
                    global::IntegrationLayer.Person.EgenskabType personProperty = StubUtil.GetLatestProperty(personRegistration.AttributListe);

                    if (personProperty != null)
                    {
                        personName = personProperty.NavnTekst;
                        personShortKey = personProperty.BrugervendtNoegleTekst;
                        personCpr = personProperty.CPRNummerTekst;
                    }
                }

                person = new Person()
                {
                    Name = personName,
                    ShortKey = personShortKey,
                    Cpr = personCpr,
                    Uuid = personUuid
                };
            }

            List<Position> positions = new List<Position>();
            List<FiltreretOejebliksbilledeType> unitRoles = ServiceHelper.FindUnitRolesForUser(uuid);
            if (unitRoles != null && unitRoles.Count > 0)
            {
                foreach (var unitRole in unitRoles)
                {
                    string orgFunctionName = "OrgFunction object does not exist in Organisation";
                    string orgFunctionShortKey = "OrgFunction object does not exist in Organisation";
                    OUReference ou = new OUReference()
                    {
                        Uuid = null,
                        Name = "OrgUnit object does not exist in Organisation"
                    };

                    string positionUuid = unitRole.ObjektType.UUIDIdentifikator;
                    RegistreringType1 orgFunctionRegistration = unitRole.Registrering[0];
                    if (orgFunctionRegistration != null)
                    {
                        global::IntegrationLayer.OrganisationFunktion.EgenskabType orgFunctionProperty = StubUtil.GetLatestProperty(orgFunctionRegistration.AttributListe.Egenskab);

                        if (orgFunctionProperty != null)
                        {
                            orgFunctionName = orgFunctionProperty.FunktionNavn;
                            orgFunctionShortKey = orgFunctionProperty.BrugervendtNoegleTekst;
                        }

                        if (orgFunctionRegistration.RelationListe.TilknyttedeEnheder != null && orgFunctionRegistration.RelationListe.TilknyttedeEnheder.Length > 0)
                        {
                            global::IntegrationLayer.OrganisationFunktion.OrganisationEnhedFlerRelationType parentOu = orgFunctionRegistration.RelationListe.TilknyttedeEnheder[0];
                            string parentOuUuid = parentOu.ReferenceID.Item;
                            ou.Uuid = parentOuUuid;

                            if (readParentDetails.Equals(ReadParentDetails.YES))
                            {
                                global::IntegrationLayer.OrganisationEnhed.RegistreringType1 parentRegistration = organisationEnhedStub.GetLatestRegistration(parentOuUuid);
                                if (parentRegistration != null)
                                {
                                    global::IntegrationLayer.OrganisationEnhed.EgenskabType parentProperties = StubUtil.GetLatestProperty(parentRegistration.AttributListe.Egenskab);
                                    if (parentProperties != null)
                                    {
                                        ou.Name = parentProperties.EnhedNavn;
                                    }
                                }
                            }
                        }
                    }

                    Position position = new Position()
                    {
                        Name = orgFunctionName,
                        OU = ou,
                        ShortKey = orgFunctionShortKey,
                        Uuid = positionUuid
                    };

                    positions.Add(position);
                }
            }

            Status status = Status.ACTIVE;
            var latestState = StubUtil.GetLatestGyldighed(registration.TilstandListe.Gyldighed);
            if (latestState == null)
            {
                status = Status.UNKNOWN;
            }
            else if (global::IntegrationLayer.Bruger.GyldighedStatusKodeType.Inaktiv.Equals(latestState.GyldighedStatusKode))
            {
                status = Status.INACTIVE;
            }

            return new User()
            {
                ShortKey = userShortKey,
                Uuid = uuid,
                UserId = userId,
                Addresses = addresses,
                Person = person,
                Positions = positions,
                Status = status,
                Timestamp = timestamp
            };
        }

        public List<string> FindAllUsers(List<OU> ous = null)
        {
            List<string> result = new List<string>();

            if (ous != null)
            {
                foreach (var ou in ous)
                {
                    if (ou.Positions != null)
                    {
                        foreach (var position in ou.Positions)
                        {
                            result.Add(position.User.Uuid);
                        }
                    }
                }
            }
            else
            {
                result = brugerStub.Soeg();
            }

            return result;
        }

        public List<string> FindAllOUs()
        {
            return organisationEnhedStub.Soeg();
        }

        public List<OU> ReadOUHierarchy(ReadAddresses readAddress = ReadAddresses.YES, ReadParentDetails readParentDetails = ReadParentDetails.YES, ReadPayoutUnit readPayoutUnit = ReadPayoutUnit.YES, ReadPositions readPositions = ReadPositions.YES, ReadItSystems readItSystems = ReadItSystems.YES, ReadContactPlaces readContactPlaces = ReadContactPlaces.YES)
        {
            List<OU> result = new List<OU>();

            var registrations = new List<OrgUnitRegWrapper>();
            int offset = 0, hardstop = 0;
            while (true)
            {
                // TODO: KMD BUG - change this back once fixed - they do not allow more than 2 reads before breaking
                if (hardstop++ >= 2)
//              if (hardstop++ >= 100)
                {
                    log.Warn("Did 100 pages on object hierarchy, without seeing the end - aborting!");
                    break;
                }

                var res = organisationSystemStub.Read("500", "" + offset);
                offset += res.Count;

                if (res.Count == 0)
                {
                    break;
                }

                registrations.AddRange(res);
            }

            foreach (var reg in registrations)
            {
                var ou = MapRegistrationToOU(reg.Registration, reg.Uuid, readAddress, readParentDetails, readPayoutUnit, readPositions, readItSystems, readContactPlaces);

                result.Add(ou);
            }

            return result;
        }

        public Hierarchy ReadHiearchy()
        {
            Hierarchy result = new Hierarchy();

            var ous = ReadOUHierarchy(ReadAddresses.NO, ReadParentDetails.NO, ReadPayoutUnit.NO, ReadPositions.NO, ReadItSystems.NO, ReadContactPlaces.NO);
            foreach (var ou in ous)
            {
                BasicOU basicOU = new BasicOU()
                {
                    Name = ou.Name,
                    ParentOU = ou.ParentOU?.Uuid,
                    Uuid = ou.Uuid
                };

                result.OUs.Add(basicOU);
            }

            log.Debug("ReadHierarchy: " + ous.Count + " ous read!");

            var users = new List<User>();
            foreach (var ou in ous)
            {
                int count = users.Count;
                FindAllUsersInOU(ou, users);

                if ((users.Count - count) > 0)
                {
                    log.Debug("ReadHierarchy: " + (users.Count - count) + " users read from " + ou.Name);
                }
            }

            log.Debug("ReadHierarchy: " + users.Count + " users read in total");

            foreach (var user in users)
            {
                BasicUser basicUser = new BasicUser()
                {
                    Name = user.Person?.Name,
                    UserId = user.UserId,
                    Uuid = user.Uuid
                };

                foreach (var position in user.Positions)
                {
                    basicUser.Positions.Add(new BasicPosition() {
                        Uuid = position.OU.Uuid,
                        Name = position.Name
                    });
                }

                foreach (var address in user.Addresses)
                {
                    if (address is Email)
                    {
                        basicUser.Email = address.Value;
                    }
                    else if (address is Phone)
                    {
                        basicUser.Telephone = address.Value;
                    }
                }

                result.Users.Add(basicUser);
            }

            return result;
        }
    }
}
