using System;
using System.Collections.Generic;
using Organisation.IntegrationLayer;

namespace Organisation.BusinessLayer
{
    public enum ReadAddresses { YES, NO };
    public enum ReadParentDetails { YES, NO };
    public enum ReadPayoutUnit { YES, NO };
    public enum ReadPositions { YES, NO };
    public enum ReadItSystems { YES, NO };

    public class InspectorService
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private BrugerStub brugerStub = new BrugerStub();
        private OrganisationFunktionStub orgFunctionStub = new OrganisationFunktionStub();
        private AdresseStub adresseStub = new AdresseStub();
        private PersonStub personStub = new PersonStub();
        private OrganisationEnhedStub organisationEnhedStub = new OrganisationEnhedStub();

        public string ReadUserRaw(string uuid)
        {
            var registration = brugerStub.GetLatestRegistration(uuid, false);
            if (registration == null)
            {
                throw new RegistrationNotFoundException("Could not locate User with uuid '" + uuid + "'");
            }

            return XmlUtil.SerializeObject(registration);
        }

        public string ReadFunctionRaw(string uuid)
        {
            var registration = orgFunctionStub.GetLatestRegistration(uuid, false);
            if (registration == null)
            {
                throw new RegistrationNotFoundException("Could not locate function with uuid '" + uuid + "'");
            }

            return XmlUtil.SerializeObject(registration);
        }

        public string ReadOURaw(string uuid)
        {
            var registration = organisationEnhedStub.GetLatestRegistration(uuid, false);
            if (registration == null)
            {
                throw new RegistrationNotFoundException("Could not locate OU with uuid '" + uuid + "'");
            }

            return XmlUtil.SerializeObject(registration);
        }

        public string ReadPersonRaw(string uuid)
        {
            var registration = personStub.GetLatestRegistration(uuid, false);
            if (registration == null)
            {
                throw new RegistrationNotFoundException("Could not locate Person with uuid '" + uuid + "'");
            }

            return XmlUtil.SerializeObject(registration);
        }

        public string ReadAddressRaw(string uuid)
        {
            var registration = adresseStub.GetLatestRegistration(uuid, false);
            if (registration == null)
            {
                throw new RegistrationNotFoundException("Could not locate Address with uuid '" + uuid + "'");
            }

            return XmlUtil.SerializeObject(registration);
        }

        public Person ReadPersonObject(string uuid)
        {
            global::IntegrationLayer.Person.RegistreringType1 registration = personStub.GetLatestRegistration(uuid, true);
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
            global::IntegrationLayer.OrganisationFunktion.RegistreringType1 registration = orgFunctionStub.GetLatestRegistration(uuid, true);
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

            return new Function()
            {
                Uuid = uuid,
                Name = name,
                ShortKey = shortKey,
                FunctionType = functionType
            };
        }

        public AddressHolder ReadAddressObject(string uuid)
        {
            global::IntegrationLayer.Adresse.RegistreringType1 registration = adresseStub.GetLatestRegistration(uuid, true);
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

        public OU ReadOUObject(string uuid, ReadAddresses readAddress = ReadAddresses.YES, ReadParentDetails readParentDetails = ReadParentDetails.YES, ReadPayoutUnit readPayoutUnit = ReadPayoutUnit.YES, ReadPositions readPositions = ReadPositions.YES, ReadItSystems readItSystems = ReadItSystems.YES)
        {
            global::IntegrationLayer.OrganisationEnhed.RegistreringType1 registration = organisationEnhedStub.GetLatestRegistration(uuid, true);
            if (registration == null)
            {
                throw new RegistrationNotFoundException("Could not locate OU with uuid '" + uuid + "'");
            }
            global::IntegrationLayer.OrganisationEnhed.EgenskabType property = StubUtil.GetLatestProperty(registration.AttributListe.Egenskab);

            string ouName = (property != null) ? property.EnhedNavn : null;
            string ouShortKey = (property != null) ? property.BrugervendtNoegleTekst : null;

            List<AddressHolder> addresses = new List<AddressHolder>();

            if (readAddress.Equals(ReadAddresses.YES) && registration.RelationListe.Adresser != null)
            {
                foreach (global::IntegrationLayer.OrganisationEnhed.AdresseFlerRelationType address in registration.RelationListe.Adresser)
                {
                    string addressUuid = address.ReferenceID.Item;
                    global::IntegrationLayer.Adresse.RegistreringType1 addressRegistration = adresseStub.GetLatestRegistration(addressUuid, false);

                    string addressValue = "The address object does not exist in Organisation";
                    if (addressRegistration != null)
                    {
                        global::IntegrationLayer.Adresse.EgenskabType addressProperty = StubUtil.GetLatestProperty(addressRegistration.AttributListe);
                        if (addressProperty != null)
                        {
                            addressValue = addressProperty.AdresseTekst;
                        }
                    }

                    if (address.Rolle.Item.Equals(UUIDConstants.ADDRESS_ROLE_EMAIL))
                    {
                        addresses.Add(new Email()
                        {
                            Uuid = addressUuid,
                            Value = addressValue
                        });
                    }
                    else if (address.Rolle.Item.Equals(UUIDConstants.ADDRESS_ROLE_LOCATION))
                    {
                        addresses.Add(new Location()
                        {
                            Uuid = addressUuid,
                            Value = addressValue
                        });
                    }
                    else if (address.Rolle.Item.Equals(UUIDConstants.ADDRESS_ROLE_PHONE))
                    {
                        addresses.Add(new Phone()
                        {
                            Uuid = addressUuid,
                            Value = addressValue
                        });
                    }
                    else if (address.Rolle.Item.Equals(UUIDConstants.ADDRESS_ROLE_LOSSHORTNAME))
                    {
                        addresses.Add(new LOSShortName()
                        {
                            Uuid = addressUuid,
                            Value = addressValue
                        });
                    }
                    else if (address.Rolle.Item.Equals(UUIDConstants.ADDRESS_ROLE_EAN))
                    {
                        addresses.Add(new Ean()
                        {
                            Uuid = addressUuid,
                            Value = addressValue
                        });
                    }
                    else if (address.Rolle.Item.Equals(UUIDConstants.ADDRESS_ROLE_CONTACT_ADDRESS_OPEN_HOURS))
                    {
                        addresses.Add(new ContactHours()
                        {
                            Uuid = addressUuid,
                            Value = addressValue
                        });
                    }
                    else if (address.Rolle.Item.Equals(UUIDConstants.ADDRESS_ROLE_EMAIL_REMARKS))
                    {
                        addresses.Add(new EmailRemarks()
                        {
                            Uuid = addressUuid,
                            Value = addressValue
                        });
                    }
                    else if (address.Rolle.Item.Equals(UUIDConstants.ADDRESS_ROLE_POST_RETURN))
                    {
                        addresses.Add(new PostReturn()
                        {
                            Uuid = addressUuid,
                            Value = addressValue
                        });
                    }
                    else if (address.Rolle.Item.Equals(UUIDConstants.ADDRESS_ROLE_CONTACT_ADDRESS))
                    {
                        addresses.Add(new Contact()
                        {
                            Uuid = addressUuid,
                            Value = addressValue
                        });
                    }
                    else if (address.Rolle.Item.Equals(UUIDConstants.ADDRESS_ROLE_POST))
                    {
                        addresses.Add(new Post()
                        {
                            Uuid = addressUuid,
                            Value = addressValue
                        });
                    }
                    else if (address.Rolle.Item.Equals(UUIDConstants.ADDRESS_ROLE_PHONE_OPEN_HOURS))
                    {
                        addresses.Add(new PhoneHours()
                        {
                            Uuid = addressUuid,
                            Value = addressValue
                        });
                    }
                }
            }

            OUReference parentOU = null;
            if (registration.RelationListe.Overordnet != null)
            {
                string parentOUUuid = registration.RelationListe.Overordnet.ReferenceID.Item;
                string parentOUName = "";

                if (readParentDetails.Equals(ReadParentDetails.YES))
                {
                    global::IntegrationLayer.OrganisationEnhed.RegistreringType1 parentRegistration = organisationEnhedStub.GetLatestRegistration(parentOUUuid, true);
                    if (parentRegistration != null)
                    {
                        global::IntegrationLayer.OrganisationEnhed.EgenskabType parentProperty = StubUtil.GetLatestProperty(parentRegistration.AttributListe.Egenskab);
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
            if (readPayoutUnit.Equals(ReadPayoutUnit.YES))
            {
                if (registration.RelationListe.TilknyttedeFunktioner != null)
                {
                    foreach (var function in registration.RelationListe.TilknyttedeFunktioner)
                    {
                        var functionState = orgFunctionStub.GetLatestRegistration(function.ReferenceID.Item, true);

                        if (functionState.RelationListe.Funktionstype != null)
                        {
                            if (functionState.RelationListe.Funktionstype.ReferenceID.Item.Equals(UUIDConstants.ORGFUN_PAYOUT_UNIT))
                            {
                                if (functionState.RelationListe.TilknyttedeEnheder != null && functionState.RelationListe.TilknyttedeEnheder.Length > 0)
                                {
                                    string payoutUnitUuid = functionState.RelationListe.TilknyttedeEnheder[0].ReferenceID.Item;
                                    string payoutUnitName = "The payout unit does not eixst in Organisation";

                                    global::IntegrationLayer.OrganisationEnhed.RegistreringType1 payoutUnitRegistration = organisationEnhedStub.GetLatestRegistration(payoutUnitUuid, true);
                                    if (payoutUnitRegistration != null)
                                    {
                                        global::IntegrationLayer.OrganisationEnhed.EgenskabType payoutUnitProperty = StubUtil.GetLatestProperty(payoutUnitRegistration.AttributListe.Egenskab);

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

                                    break;
                                }
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
                    global::IntegrationLayer.OrganisationFunktion.RegistreringType1 orgFunctionRegistration = orgFunctionStub.GetLatestRegistration(positionUuid, true);
                    if (orgFunctionRegistration != null)
                    {
                        global::IntegrationLayer.OrganisationFunktion.EgenskabType orgFunctionProperty = StubUtil.GetLatestProperty(orgFunctionRegistration.AttributListe.Egenskab);

                        if (orgFunctionProperty != null)
                        {
                            orgFunctionName = orgFunctionProperty.FunktionNavn;
                            orgFunctionShortKey = orgFunctionProperty.BrugervendtNoegleTekst;
                        }

                        if (orgFunctionRegistration.RelationListe.TilknyttedeBrugere != null && orgFunctionRegistration.RelationListe.TilknyttedeBrugere.Length > 0)
                        {
                            global::IntegrationLayer.OrganisationFunktion.BrugerFlerRelationType bruger = orgFunctionRegistration.RelationListe.TilknyttedeBrugere[0];
                            string brugerUuid = bruger.ReferenceID.Item;
                            user.Uuid = brugerUuid;
                        }
                    }

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

            List<string> itSystems = new List<string>();
            if (readItSystems.Equals(ReadItSystems.YES))
            {
                itSystems = ServiceHelper.FindItSystemsForOrgUnit(uuid);
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
                ItSystems = itSystems
            };
        }

        public User ReadUserObject(string uuid, ReadAddresses readAddresses = ReadAddresses.YES, ReadParentDetails readParentDetails = ReadParentDetails.YES)
        {
            global::IntegrationLayer.Bruger.RegistreringType1 registration = brugerStub.GetLatestRegistration(uuid, true);
            if (registration == null)
            {
                throw new RegistrationNotFoundException("Could not locate User with uuid '" + uuid + "'");
            }
            global::IntegrationLayer.Bruger.EgenskabType property = StubUtil.GetLatestProperty(registration.AttributListe.Egenskab);

            string userId = (property != null) ? property.BrugerNavn : null;
            string userShortKey = (property != null) ? property.BrugervendtNoegleTekst : null;

            List<AddressHolder> addresses = new List<AddressHolder>();
            if (readAddresses.Equals(ReadAddresses.YES))
            {
                if (registration.RelationListe.Adresser != null)
                {
                    foreach (global::IntegrationLayer.Bruger.AdresseFlerRelationType address in registration.RelationListe.Adresser)
                    {
                        string addressUuid = address.ReferenceID.Item;
                        global::IntegrationLayer.Adresse.RegistreringType1 addressRegistration = adresseStub.GetLatestRegistration(addressUuid, false);

                        string addressValue = "The address object does not exist in Organisation";
                        if (addressRegistration != null)
                        {
                            global::IntegrationLayer.Adresse.EgenskabType addressProperty = StubUtil.GetLatestProperty(addressRegistration.AttributListe);

                            if (addressProperty != null)
                            {
                                addressValue = addressProperty.AdresseTekst;
                            }
                        }

                        if (address.Rolle.Item.Equals(UUIDConstants.ADDRESS_ROLE_EMAIL))
                        {
                            addresses.Add(new Email()
                            {
                                Uuid = addressUuid,
                                Value = addressValue
                            });
                        }
                        else if (address.Rolle.Item.Equals(UUIDConstants.ADDRESS_ROLE_LOCATION))
                        {
                            addresses.Add(new Location()
                            {
                                Uuid = addressUuid,
                                Value = addressValue
                            });
                        }
                        else if (address.Rolle.Item.Equals(UUIDConstants.ADDRESS_ROLE_PHONE))
                        {
                            addresses.Add(new Phone()
                            {
                                Uuid = addressUuid,
                                Value = addressValue
                            });
                        }
                    }
                }
            }

            Person person = null;
            if (registration.RelationListe.TilknyttedePersoner != null && registration.RelationListe.TilknyttedePersoner.Length > 0)
            {
                global::IntegrationLayer.Bruger.PersonFlerRelationType personRelation = registration.RelationListe.TilknyttedePersoner[0];
                string personUuid = personRelation.ReferenceID.Item;

                global::IntegrationLayer.Person.RegistreringType1 personRegistration = personStub.GetLatestRegistration(personUuid, true);
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

            Position position = null;
            List<string> unitRoles = ServiceHelper.FindUnitRolesForUser(uuid);
            if (unitRoles != null && unitRoles.Count > 0)
            {
                string orgFunctionName = "OrgFunction object does not exist in Organisation";
                string orgFunctionShortKey = "OrgFunction object does not exist in Organisation";
                OUReference ou = new OUReference()
                {
                    Uuid = null,
                    Name = "OrgUnit object does not exist in Organisation"
                };

                // only pick the first one
                string positionUuid = unitRoles.ToArray()[0];
                global::IntegrationLayer.OrganisationFunktion.RegistreringType1 orgFunctionRegistration = orgFunctionStub.GetLatestRegistration(positionUuid, true);
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
                            global::IntegrationLayer.OrganisationEnhed.RegistreringType1 parentRegistration = organisationEnhedStub.GetLatestRegistration(parentOuUuid, true);
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

                position = new Position()
                {
                    Name = orgFunctionName,
                    OU = ou,
                    ShortKey = orgFunctionShortKey,
                    Uuid = positionUuid
                };
            }

            return new User()
            {
                ShortKey = userShortKey,
                Uuid = uuid,
                UserId = userId,
                Addresses = addresses,
                Person = person,
                Position = position
            };
        }

        public List<string> FindAllUsers()
        {
            return brugerStub.Soeg();
        }

        public List<string> FindAllOUs()
        {
            return organisationEnhedStub.Soeg();
        }
    }
}
