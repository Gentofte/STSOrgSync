using System;
using System.Collections.Generic;
using Organisation.IntegrationLayer;

namespace Organisation.BusinessLayer
{
    public class UserService
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private BrugerStub brugerStub = new BrugerStub();
        private OrganisationFunktionStub orgFunctionStub = new OrganisationFunktionStub();

        public void Create(UserRegistration user)
        {
            log.Debug("Performing Create on User '" + user.UserUuid + "'");

            ValidateAndEnforceCasing(user);

            try
            {
                string uuid = null;

                // create addresses
                ServiceHelper.ImportAddress(user.Phone, user.Timestamp, out uuid);
                if (uuid != null) { user.Phone.Uuid = uuid; }

                ServiceHelper.ImportAddress(user.Email, user.Timestamp, out uuid);
                if (uuid != null) { user.Email.Uuid = uuid; }

                ServiceHelper.ImportAddress(user.Location, user.Timestamp, out uuid);
                if (uuid != null) { user.Location.Uuid = uuid; }

                // create person object
                ServiceHelper.UpdatePerson(user, null);

                // create the position (relationship between user and orgunit) - note that the function points to the two parties, not the other way around
                ServiceHelper.UpdatePosition(user, null);

                // create User object
                brugerStub.Importer(MapRegistrationToUserDTO(user));

                log.Debug("Create successful on User '" + user.UserUuid + "'");
            }
            catch (Exception ex) when (ex is STSNotFoundException || ex is ServiceNotFoundException)
            {
                log.Warn("Create on UserService failed for '" + user.UserUuid + "' due to unavailable KOMBIT services", ex);
                throw new TemporaryFailureException(ex.Message);
            }
        }

        public void Update(UserRegistration user)
        {
            log.Debug("Performing Update on User '" + user.UserUuid + "'");

            ValidateAndEnforceCasing(user);

            try
            {
                var result = brugerStub.GetLatestRegistration(user.UserUuid, true);
                if (result == null)
                {
                    log.Debug("Update on User '" + user.UserUuid + "' changed to a Create because it does not exists as an active object within Organisation");
                    Create(user);
                }
                else
                {
                    #region Update Addresses in Organisation if needed
                    // check what already exists in Organisation - and store the UUIDs of the existing addresses, we will need those later
                    string orgPhoneUuid = null, orgEmailUuid = null, orgLocationUuid = null;
                    if (result.RelationListe.Adresser != null)
                    {
                        foreach (var orgAddress in result.RelationListe.Adresser)
                        {
                            if (orgAddress.Rolle.Item.Equals(UUIDConstants.ADDRESS_ROLE_PHONE))
                            {
                                orgPhoneUuid = orgAddress.ReferenceID.Item;
                            }
                            else if (orgAddress.Rolle.Item.Equals(UUIDConstants.ADDRESS_ROLE_EMAIL))
                            {
                                orgEmailUuid = orgAddress.ReferenceID.Item;
                            }
                            else if (orgAddress.Rolle.Item.Equals(UUIDConstants.ADDRESS_ROLE_LOCATION))
                            {
                                orgLocationUuid = orgAddress.ReferenceID.Item;
                            }
                        }
                    }

                    // run through all the input addresses, and deal with them one by one
                    ServiceHelper.UpdateAddress(user.Phone, orgPhoneUuid, user.Timestamp);
                    ServiceHelper.UpdateAddress(user.Email, orgEmailUuid, user.Timestamp);
                    ServiceHelper.UpdateAddress(user.Location, orgLocationUuid, user.Timestamp);
                    #endregion

                    #region Update Position if needed
                    // TODO: this call also returns expired positions due to a bug at KMD, so this code is kinda buggy right now
                    //       but should work once KMD has fixed it in their end.
                    string unitRoleUuid = null;
                    List<string> unitRoles = ServiceHelper.FindUnitRolesForUser(user.UserUuid);
                    if (unitRoles != null && unitRoles.Count > 0)
                    {
                        // Bit of a hack, but because of the way the library work, we know that there is only a single function
                        unitRoleUuid = unitRoles.ToArray()[0];
                    }
                    ServiceHelper.UpdatePosition(user, unitRoleUuid);
                    #endregion

                    #region Update Person reference if needed
                    string orgPersonUuid = null;
                    if (result.RelationListe.TilknyttedePersoner != null && result.RelationListe.TilknyttedePersoner.Length > 0)
                    {
                        // we read actual-state-only, so there is precisely ONE person object - but better safe than sorry
                        orgPersonUuid = result.RelationListe.TilknyttedePersoner[0].ReferenceID.Item;
                    }
                    ServiceHelper.UpdatePerson(user, orgPersonUuid);
                    #endregion

                    // Update the User object (attributes and all relationships)
                    brugerStub.Ret(MapRegistrationToUserDTO(user));

                    log.Debug("Update successful on User '" + user.UserUuid + "'");
                }
            }
            catch (Exception ex) when (ex is STSNotFoundException || ex is ServiceNotFoundException)
            {
                log.Warn("Update on UserService failed for '" + user.UserUuid + "' due to unavailable KOMBIT services", ex);
                throw new TemporaryFailureException(ex.Message);
            }
        }

        public void Delete(string uuid, DateTime timestamp)
        {
            try
            {
                // find the OrgFunctions that represents this users positions within the municipality
                // for each of these OrgFunctions and drop the relationship to both User and OrgUnit from that Function
                List<string> unitRoleUUIDs = ServiceHelper.FindUnitRolesForUser(uuid);
                foreach (string unitRoleUUID in unitRoleUUIDs)
                {
                    orgFunctionStub.Orphan(unitRoleUUID, timestamp);
                }

                // update the user object by
                //   -> terminating the users relationship to the Organisation
                brugerStub.Orphan(uuid, timestamp);
            }
            catch (Exception ex) when (ex is STSNotFoundException || ex is ServiceNotFoundException)
            {
                log.Warn("Delete on UserService failed for '" + uuid + "' due to unavailable KOMBIT services", ex);
                throw new TemporaryFailureException(ex.Message);
            }
        }

        private UserData MapRegistrationToUserDTO(UserRegistration registration)
        {
            // loop through all address information in the input, and build the corresponding relations for User object
            List<AddressRelation> addressRelations = new List<AddressRelation>();

            if (registration.Phone != null)
            {
                AddressRelation address = new AddressRelation();
                address.Type = AddressRelationType.PHONE;
                address.Uuid = registration.Phone.Uuid;
                addressRelations.Add(address);
            }

            if (registration.Email != null)
            {
                AddressRelation address = new AddressRelation();
                address.Type = AddressRelationType.EMAIL;
                address.Uuid = registration.Email.Uuid;
                addressRelations.Add(address);
            }

            if (registration.Location != null)
            {
                AddressRelation address = new AddressRelation();
                address.Type = AddressRelationType.LOCATION;
                address.Uuid = registration.Location.Uuid;
                addressRelations.Add(address);
            }

            UserData user = new UserData();
            user.Addresses = addressRelations;
            user.PersonUuid = registration.PersonUuid;
            user.ShortKey = registration.UserShortKey;
            user.Timestamp = registration.Timestamp;
            user.UserId = registration.UserId;
            user.Uuid = registration.UserUuid;

            return user;
        }

        private void ValidateAndEnforceCasing(UserRegistration registration)
        {
            List<string> errors = new List<string>();

            if (string.IsNullOrEmpty(registration.PersonName))
            {
                errors.Add("personName");
            }

            if (string.IsNullOrEmpty(registration.PositionName))
            {
                errors.Add("positionName");
            }

            if (string.IsNullOrEmpty(registration.PositionOrgUnitUuid))
            {
                errors.Add("positionOrgUnitUuid");
            }

            if (string.IsNullOrEmpty(registration.UserId))
            {
                errors.Add("userId");
            }

            if (string.IsNullOrEmpty(registration.UserUuid))
            {
                errors.Add("userUuid");
            }

            if (registration.Timestamp == null)
            {
                errors.Add("timestamp");
            }

            if (errors.Count > 0)
            {
                throw new InvalidFieldsException("Invalid registration object - the following fields are invalid: " + string.Join(",", errors));
            }

            if (registration.Email != null && string.IsNullOrEmpty(registration.Email.Value))
            {
                registration.Email = null;
            }

            if (registration.Location != null && string.IsNullOrEmpty(registration.Location.Value))
            {
                registration.Location = null;
            }

            if (registration.Phone != null && string.IsNullOrEmpty(registration.Phone.Value))
            {
                registration.Phone = null;
            }

            if (registration.Email?.Uuid != null)
            {
                registration.Email.Uuid = registration.Email.Uuid.ToLower();
            }

            if (registration.Location?.Uuid != null)
            {
                registration.Location.Uuid = registration.Location.Uuid.ToLower();
            }

            if (registration.Phone?.Uuid != null)
            {
                registration.Phone.Uuid = registration.Phone.Uuid.ToLower();
            }

            if (registration.PersonUuid != null)
            {
                registration.PersonUuid = registration.PersonUuid.ToLower();
            }

            if (registration.PositionUuid != null)
            {
                registration.PositionUuid = registration.PositionUuid.ToLower();
            }

            if (registration.PersonCpr != null)
            {
                // strip dashes, so 010101-0101 becomes 010101010101 (KOMBIT requirement)
                registration.PersonCpr = registration.PersonCpr.Replace("-", "");
            }

            registration.UserUuid = registration.UserUuid.ToLower();
            registration.PositionOrgUnitUuid = registration.PositionOrgUnitUuid.ToLower();
        }
    }
}
