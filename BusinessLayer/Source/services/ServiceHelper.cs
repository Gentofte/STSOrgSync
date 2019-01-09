using System;
using Organisation.IntegrationLayer;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Organisation.BusinessLayer.DTO.V1_1;
using IntegrationLayer.OrganisationFunktion;

namespace Organisation.BusinessLayer
{
    class ServiceHelper
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static OrganisationFunktionStub organisationFunktionStub = new OrganisationFunktionStub();
        private static AdresseStub adresseStub = new AdresseStub();
        private static PersonStub personStub = new PersonStub();

        internal static List<FiltreretOejebliksbilledeType> FindUnitRolesForUser(string uuid, List<FiltreretOejebliksbilledeType> allUnitRoles = null)
        {
            if (allUnitRoles != null)
            {
                var unitRoles = new List<FiltreretOejebliksbilledeType>();
                foreach (var unitRole in allUnitRoles)
                {
                    if (unitRole.Registrering.Length == 0)
                    {
                        continue;
                    }

                    if (unitRole.Registrering[0].RelationListe?.TilknyttedeBrugere == null)
                    {
                        continue;
                    }

                    if (unitRole.Registrering[0].RelationListe.TilknyttedeBrugere.Length == 0)
                    {
                        continue;
                    }

                    foreach (var brugerRelation in unitRole.Registrering[0].RelationListe.TilknyttedeBrugere)
                    {
                        if (brugerRelation.ReferenceID.Item.Equals(uuid))
                        {
                            unitRoles.Add(unitRole);
                        }
                    }
                }

                return unitRoles;
            }
            else
            {
                return organisationFunktionStub.SoegAndGetLatestRegistration(UUIDConstants.ORGFUN_POSITION, uuid, null, null);
            }
        }

        internal static List<string> FindUnitRolesForOrgUnit(string uuid)
        {
            return organisationFunktionStub.SoegAndGetUuids(UUIDConstants.ORGFUN_POSITION, null, uuid, null);
        }

        internal static List<FiltreretOejebliksbilledeType> FindUnitRolesForOrgUnitAsObjects(string uuid)
        {
            return organisationFunktionStub.SoegAndGetLatestRegistration(UUIDConstants.ORGFUN_POSITION, null, uuid, null);
        }

        internal static List<string> FindOrgUnitRolesForItSystem(string uuid)
        {
            return organisationFunktionStub.SoegAndGetUuids(UUIDConstants.ORGFUN_IT_USAGE, null, null, uuid);
        }

        internal static void ImportAddress(Address address, DateTime timestamp, out string uuid)
        {
            uuid = null;

            if (address == null)
            {
                return;
            }

            AddressData addressData = new AddressData()
            {
                AddressText = address.Value,
                Timestamp = timestamp,
                ShortKey = address.ShortKey,
                Uuid = address.Uuid
            };

            adresseStub.Importer(addressData);

            uuid = addressData.Uuid;
        }

        internal static void UpdatePosition(UserRegistration user)
        {
            /*
               1. fetch all positions that the user currently have
               2. compare positions from organisation with the positions in the registration, using the following rules
                  a) a position is a "match" if it points to the same unit (and no two positions can point to the same unit), or if it matches on UUID
                  b) a position should be updated if the name/shortKey/ou-pointer has been changed
                  c) a position should be removed if it no longer exist in the registration, but does in organisation
                  d) a position should be added, if it exists in the registration, but not in organisation
             */

            // fetch all the users existing positions
            List<FiltreretOejebliksbilledeType> unitRoles = FindUnitRolesForUser(user.Uuid);

            // loop through roles found in organisation, and find those that must be updated, and those that must be deleted
            foreach (FiltreretOejebliksbilledeType unitRole in unitRoles)
            {
                RegistreringType1 existingRoleRegistration = unitRole.Registrering[0];

                if (existingRoleRegistration.RelationListe.TilknyttedeEnheder.Length != 1)
                {
                    log.Warn("User '" + user.Uuid + "' has an existing position in Organisation with " + existingRoleRegistration.RelationListe.TilknyttedeEnheder.Length + " associated OrgUnits");
                    continue;
                }

                // figure out everything relevant about the position object in Organisation
                EgenskabType latestProperty = StubUtil.GetLatestProperty(existingRoleRegistration.AttributListe.Egenskab);
                string existingRoleUuid = unitRole.ObjektType.UUIDIdentifikator;
                string existingRoleOUUuid = existingRoleRegistration.RelationListe.TilknyttedeEnheder[0].ReferenceID.Item;
                string existingRoleName = latestProperty.FunktionNavn;
                string existingRoleShortKey = latestProperty.BrugervendtNoegleTekst;

                bool found = false;
                foreach (DTO.V1_1.Position position in user.Positions)
                {
                    // if the UUID of the function is controlled by the local system, the pointer to the OU could be changed,
                    // so we also need to check for equality on the UUID of the function itself
                    if (existingRoleUuid.Equals(position.Uuid) || (position.Uuid == null && existingRoleOUUuid.Equals(position.OrgUnitUuid)))
                    {
                        if (!existingRoleOUUuid.Equals(position.OrgUnitUuid) ||  // user has moved to a different OU
                            (existingRoleName == null || !existingRoleName.Equals(position.Name)) || // the users title has changed (null check deals with bad GUI data)
                            (position.ShortKey != null && existingRoleShortKey.Equals(position.ShortKey))) // there is a new ShortKey for the position
                        {
                            organisationFunktionStub.Ret(new OrgFunctionData()
                            {
                                Uuid = existingRoleUuid,
                                ShortKey = (position.ShortKey != null) ? position.ShortKey : existingRoleShortKey,
                                Name = position.Name,
                                FunctionTypeUuid = UUIDConstants.ORGFUN_POSITION,
                                OrgUnits = new List<string>() { position.OrgUnitUuid },
                                Users = new List<string>() { user.Uuid },
                                Timestamp = user.Timestamp
                            }, UpdateIndicator.NONE, UpdateIndicator.COMPARE, UpdateIndicator.NONE);
                        }

                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    organisationFunktionStub.Deactivate(existingRoleUuid, user.Timestamp);
                }
            }

            // loop through all roles found in the local registration, and create all those that do not exist in Organisation
            foreach (DTO.V1_1.Position position in user.Positions)
            {
                bool found = false;

                foreach (FiltreretOejebliksbilledeType unitRole in unitRoles)
                {
                    RegistreringType1 existingRoleRegistration = unitRole.Registrering[0];

                    if (existingRoleRegistration.RelationListe.TilknyttedeEnheder.Length != 1)
                    {
                        log.Warn("User '" + user.Uuid + "' has an existing position in Organisation with " + existingRoleRegistration.RelationListe.TilknyttedeEnheder.Length + " associated OrgUnits");
                        continue;
                    }

                    string existingRoleUuid = unitRole.ObjektType.UUIDIdentifikator;
                    string existingRoleOUUuid = existingRoleRegistration.RelationListe.TilknyttedeEnheder[0].ReferenceID.Item;

                    if (existingRoleUuid.Equals(position.Uuid) || (position.Uuid == null && existingRoleOUUuid.Equals(position.OrgUnitUuid)))
                    {
                        found = true;
                    }
                }

                if (!found)
                {
                    organisationFunktionStub.Importer(new OrgFunctionData()
                    {
                        Uuid = position.Uuid,
                        ShortKey = position.ShortKey,
                        Name = position.Name,
                        FunctionTypeUuid = UUIDConstants.ORGFUN_POSITION,
                        OrgUnits = new List<string>() { position.OrgUnitUuid },
                        Users = new List<string>() { user.Uuid },
                        Timestamp = user.Timestamp
                    });
                }
            }
        }

        internal static string CreateContactPlace(OrgUnitRegistration orgUnit, DTO.V1_1.ContactPlace contactPlace)
        {
            string uuid = IdUtil.GenerateUuid();

            organisationFunktionStub.Importer(new OrgFunctionData()
            {
                Uuid = uuid,
                Name = "Henvendelsessted",
                FunctionTypeUuid = UUIDConstants.ORGFUN_CONTACT_UNIT,
                OrgUnits = new List<string>() { contactPlace.OrgUnitUuid },
                Tasks = contactPlace.Tasks,
                Timestamp = orgUnit.Timestamp
            });

            return uuid;
        }

        // this method must be synchronized, as we create a function on the first entry into this method, and return the same value on all other calls
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal static string EnsurePayoutUnitFunctionExists(string payoutUnitUuid, DateTime timestamp)
        {
            // if there is an existing function, just return the uuid
            List<string> existingFunctions = organisationFunktionStub.SoegAndGetUuids(UUIDConstants.ORGFUN_PAYOUT_UNIT, null, payoutUnitUuid, null);
            if (existingFunctions != null && existingFunctions.Count > 0)
            {
                return existingFunctions[0];
            }

            // otherwise create a new function
            OrgFunctionData orgFunction = new OrgFunctionData();
            orgFunction.FunctionTypeUuid = UUIDConstants.ORGFUN_PAYOUT_UNIT;
            orgFunction.Name = "PayoutUnitFunction";
            orgFunction.OrgUnits.Add(payoutUnitUuid);
            orgFunction.ShortKey = IdUtil.GenerateShortKey();
            orgFunction.Timestamp = timestamp;
            orgFunction.Uuid = IdUtil.GenerateUuid();

            organisationFunktionStub.Importer(orgFunction);

            return orgFunction.Uuid;
        }

        internal static void UpdatePerson(UserRegistration user, string orgPersonUuid)
        {
            if (orgPersonUuid != null && (user.Person.Uuid == null || orgPersonUuid.Equals(user.Person.Uuid)))
            {
                // This is the expected case for an update operation - see if we have updates for the referenced Person object

                personStub.Ret(orgPersonUuid, user.Person.Name, user.Person.ShortKey, user.Person.Cpr, user.Timestamp);

                // ensure that we have the uuid of the person in the user object, as we will need it for later
                user.Person.Uuid = orgPersonUuid;
            }
            else
            {
                // This is either because no Person object existed for the User (first time creation), or because
                // the local supplied uuid of the Person object differs from the one stored in Organisation. In both
                // cases we need to create the Person object from scratch

                PersonData personData = new PersonData()
                {
                    Cpr = user.Person.Cpr,
                    Name = user.Person.Name,
                    ShortKey = user.Person.ShortKey,
                    Timestamp = user.Timestamp,
                    Uuid = user.Person.Uuid
                };

                // TODO: this could potentially fail if we are re-using a Person object locally - but we really don't want to support that case
                personStub.Importer(personData);

                // ensure that we have the uuid of the person in the user object, as we will need it for later
                user.Person.Uuid = personData.Uuid;
            }
        }

        internal static void UpdateAddress(Address address, string orgUuid, DateTime timestamp)
        {
            if (address != null)
            {
                // we allow null-uuids on the input, so overwrite null values with any existing values
                address.Uuid = (address.Uuid != null) ? address.Uuid : orgUuid;
                string uuid = null;

                UpdateAddress(orgUuid, new AddressData()
                {
                    AddressText = address.Value,
                    ShortKey = address.ShortKey,
                    Timestamp = timestamp,
                    Uuid = address.Uuid
                }, out uuid);

                // if we get a uuid back, store it so we can reference it later for the relationship
                if (uuid != null) { address.Uuid = uuid; }
            }
        }

        // TODO: merge this logic into the method above - no reason to split this
        private static void UpdateAddress(string uuidOfOldAddress, AddressData newAddress, out string uuid)
        {
            uuid = null;

            // catch-all: in case no shortkey is supplied, generate one
            if (string.IsNullOrEmpty(newAddress.ShortKey))
            {
                newAddress.ShortKey = Guid.NewGuid().ToString().ToLower();
            }

            if (uuidOfOldAddress == null || !uuidOfOldAddress.Equals(newAddress.Uuid))
            {
                adresseStub.Importer(newAddress);
            }
            else // same uuid (or no uuid supplied), so update latest registration of existing object
            {
                var result = adresseStub.GetLatestRegistration(uuidOfOldAddress);
                if (result == null)
                {
                    adresseStub.Importer(newAddress);
                }
                else
                {
                    adresseStub.Ret(uuidOfOldAddress, newAddress.AddressText, newAddress.ShortKey, newAddress.Timestamp, result);
                }
            }

            uuid = newAddress.Uuid;
        }

        internal static List<string> FindItSystemRolesForOrgUnit(string uuid)
        {
            return organisationFunktionStub.SoegAndGetUuids(UUIDConstants.ORGFUN_IT_USAGE, null, uuid, null);
        }

        internal static List<string> FindItSystemsForOrgUnit(string uuid)
        {
            List<string> itSystems = new List<string>();

            List<string> itSystemRoles = organisationFunktionStub.SoegAndGetUuids(UUIDConstants.ORGFUN_IT_USAGE, null, uuid, null);
            if (itSystemRoles != null)
            {
                foreach (string itSystemRole in itSystemRoles)
                {
                    itSystems.Add(ServiceHelper.GetItSystemForRole(itSystemRole));
                }
            }

            return itSystems;
        }

        internal static void OrgUnitStopUsingItSystem(string orgUnitUuid, string itSystemUuid, DateTime timestamp)
        {
            List<string> orgFunctions = organisationFunktionStub.SoegAndGetUuids(UUIDConstants.ORGFUN_IT_USAGE, null, orgUnitUuid, itSystemUuid);
            if (orgFunctions != null && orgFunctions.Count > 0)
            {
                log.Debug("Terminating relationship between itSystem " + itSystemUuid + " and orgUnit " + orgUnitUuid);

                // there shoud only be one, but might as well do a wide cleanup, just in case
                for (int i = 0; i < orgFunctions.Count; i++)
                {
                    OrgFunctionData orgFunction = new OrgFunctionData()
                    {
                        Timestamp = timestamp,
                        Uuid = orgFunctions[i],
                        OrgUnits = new List<string>() { orgUnitUuid }
                    };
                
                    organisationFunktionStub.Ret(orgFunction, UpdateIndicator.NONE, UpdateIndicator.REMOVE, UpdateIndicator.NONE);
                }
            }
            else
            {
                log.Debug("There is no active relationship between itsystem " + itSystemUuid + " and orgUnit " + orgUnitUuid + " to stop - so nothing updated!");
            }
        }

        // synchronized to avoid creating multiple functions
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal static void OrgUnitStartUsingItSystem(string orgUnitUuid, string itSystemUuid, DateTime timestamp)
        {
            List<string> orgFunctions = organisationFunktionStub.SoegAndGetUuids(UUIDConstants.ORGFUN_IT_USAGE, null, null, itSystemUuid);

            // ensure that there is an OrgFunction
            if (orgFunctions == null || orgFunctions.Count == 0)
            {
                List<string> itSystem = new List<string>() { itSystemUuid };

                string uuid = IdUtil.GenerateUuid();
                organisationFunktionStub.Importer(new OrgFunctionData()
                {
                    Uuid = uuid,
                    ShortKey = IdUtil.GenerateShortKey(),
                    Name = "IT-Usage",
                    FunctionTypeUuid = UUIDConstants.ORGFUN_IT_USAGE,
                    OrgUnits = null,
                    Users = null,
                    ItSystems = itSystem,
                    Addresses = null,
                    Timestamp = timestamp
                });

                orgFunctions = new List<string>() { uuid };
            }

            log.Debug("Starting relationship between itSystem " + itSystemUuid + " and orgUnit " + orgUnitUuid);

            OrgFunctionData orgFunction = new OrgFunctionData()
            {
                Timestamp = timestamp,
                Uuid = orgFunctions[0], // there will ever only be one OrgFunction, but even if there are more, we just pick the first one (any will do)
                OrgUnits = new List<string>() { orgUnitUuid }
            };
            organisationFunktionStub.Ret(orgFunction, UpdateIndicator.NONE, UpdateIndicator.ADD, UpdateIndicator.NONE);
        }

        internal static string GetItSystemForRole(string itSystemRole)
        {
            var registration = organisationFunktionStub.GetLatestRegistration(itSystemRole);
            if (registration.RelationListe.TilknyttedeItSystemer != null && registration.RelationListe.TilknyttedeItSystemer.Length > 0)
            {
                return registration.RelationListe.TilknyttedeItSystemer[0].ReferenceID.Item;
            }

            return null;
        }

        internal static List<string> GetItSystemsNewInUse(List<string> itSystemsInOrg, List<string> itSystemsInLocal)
        {
            List<string> itSystemsNewInUse = new List<string>();

            if (itSystemsInLocal != null)
            {
                foreach (string itSystemInLocal in itSystemsInLocal)
                {
                    bool found = false;

                    if (itSystemsInOrg != null)
                    {
                        foreach (string itSystemInOrg in itSystemsInOrg)
                        {
                            if (itSystemInOrg.Equals(itSystemInLocal))
                            {
                                found = true;
                            }
                        }
                    }

                    if (!found)
                    {
                        itSystemsNewInUse.Add(itSystemInLocal);
                    }
                }
            }

            return itSystemsNewInUse;
        }

        internal static void UpdateContactPlace(string uuid, List<string> tasks, DateTime timestamp)
        {
            organisationFunktionStub.Ret(new OrgFunctionData()
            {
                Uuid = uuid,
                Tasks = tasks,
                Timestamp = timestamp
            }, UpdateIndicator.NONE, UpdateIndicator.NONE, UpdateIndicator.COMPARE);
        }

        internal static List<string> GetItSystemsNoLongerInUse(List<string> itSystemsInOrg, List<string> itSystemsInLocal)
        {
            List<string> itSystemsNoLongerInUse = new List<string>();

            if (itSystemsInOrg != null)
            {
                foreach (string itSystemInOrg in itSystemsInOrg)
                {
                    bool found = false;

                    if (itSystemsInLocal != null)
                    {
                        foreach (string itSystemInLocal in itSystemsInLocal)
                        {
                            if (itSystemInLocal.Equals(itSystemInOrg))
                            {
                                found = true;
                            }
                        }
                    }

                    if (!found)
                    {
                        itSystemsNoLongerInUse.Add(itSystemInOrg);
                    }
                }
            }

            return itSystemsNoLongerInUse;
        }
    }
}
