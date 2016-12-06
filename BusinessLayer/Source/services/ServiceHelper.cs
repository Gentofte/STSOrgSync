using System;
using Organisation.IntegrationLayer;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Organisation.BusinessLayer
{
    class ServiceHelper
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static OrganisationFunktionStub organisationFunktionStub = new OrganisationFunktionStub();
        private static AdresseStub adresseStub = new AdresseStub();
        private static PersonStub personStub = new PersonStub();

        internal static List<string> FindUnitRolesForUser(string uuid)
        {
            return organisationFunktionStub.SoegAndGetUuids(UUIDConstants.ORGFUN_POSITION, uuid, null, null);
        }

        internal static List<string> FindUnitRolesForOrgUnit(string uuid)
        {
            return organisationFunktionStub.SoegAndGetUuids(UUIDConstants.ORGFUN_POSITION, null, uuid, null);
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

        internal static void UpdatePosition(UserRegistration user, string unitRoleUuid)
        {
            if (unitRoleUuid != null && (user.PositionUuid == null || user.PositionUuid.Equals(unitRoleUuid)))
            {
                // This is the expected case for an update operation - see if we have updates for the referenced OrgFunction object

                organisationFunktionStub.Ret(new OrgFunctionData()
                {
                    Uuid = unitRoleUuid,
                    ShortKey = user.PositionShortKey,
                    Name = user.PositionName,
                    FunctionTypeUuid = UUIDConstants.ORGFUN_POSITION,
                    OrgUnits = new List<string>() { user.PositionOrgUnitUuid },
                    Users = new List<string>() { user.UserUuid },
                    Timestamp = user.Timestamp
                }, UpdateIndicator.COMPARE, UpdateIndicator.COMPARE);
            }
            else
            {
                // this is either a Create case, with no existing position (OrgFunction), or we have changed position,
                // in which case we need to Orphan the existing position

                if (unitRoleUuid != null)
                {
                    organisationFunktionStub.Orphan(unitRoleUuid, user.Timestamp);
                }

                organisationFunktionStub.Importer(new OrgFunctionData()
                {
                    Uuid = user.PositionUuid,
                    ShortKey = user.PositionShortKey,
                    Name = user.PositionName,
                    FunctionTypeUuid = UUIDConstants.ORGFUN_POSITION,
                    OrgUnits = new List<string>() { user.PositionOrgUnitUuid },
                    Users = new List<string>() { user.UserUuid },
                    Timestamp = user.Timestamp
                });
            }
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
            if (orgPersonUuid != null && (user.PersonUuid == null || orgPersonUuid.Equals(user.PersonUuid)))
            {
                // This is the expected case for an update operation - see if we have updates for the referenced Person object

                personStub.Ret(orgPersonUuid, user.PersonName, user.PersonShortKey, user.PersonCpr, user.Timestamp);

                // ensure that we have the uuid of the person in the user object, as we will need it for later
                user.PersonUuid = orgPersonUuid;
            }
            else
            {
                // This is either because no Person object existed for the User (first time creation), or because
                // the local supplied uuid of the Person object differs from the one stored in Organisation. In both
                // cases we need to create the Person object from scratch

                PersonData personData = new PersonData()
                {
                    Cpr = user.PersonCpr,
                    Name = user.PersonName,
                    ShortKey = user.PersonShortKey,
                    Timestamp = user.Timestamp,
                    Uuid = user.PersonUuid
                };

                // TODO: this could potentially fail if we are re-using a Person object locally - but we really don't want to support that case
                personStub.Importer(personData);

                // ensure that we have the uuid of the person in the user object, as we will need it for later
                user.PersonUuid = personData.Uuid;
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

            if (uuidOfOldAddress == null || !uuidOfOldAddress.Equals(newAddress.Uuid))
            {
                adresseStub.Importer(newAddress);
            }
            else // same uuid (or no uuid supplied), so update latest registration of existing object
            {
                var result = adresseStub.GetLatestRegistration(uuidOfOldAddress, false);
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
                
                    organisationFunktionStub.Ret(orgFunction, UpdateIndicator.NONE, UpdateIndicator.REMOVE);
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
            organisationFunktionStub.Ret(orgFunction, UpdateIndicator.NONE, UpdateIndicator.ADD);
        }

        internal static string GetItSystemForRole(string itSystemRole)
        {
            var registration = organisationFunktionStub.GetLatestRegistration(itSystemRole, true);
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
