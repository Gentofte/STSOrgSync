using System;
using System.Collections.Generic;
using Organisation.IntegrationLayer;
using Organisation.BusinessLayer.DTO.V1_1;
using IntegrationLayer.OrganisationFunktion;

namespace Organisation.BusinessLayer
{
    public class UserService
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private BrugerStub brugerStub = new BrugerStub();
        private OrganisationFunktionStub orgFunctionStub = new OrganisationFunktionStub();
        private InspectorService inspectorService = new InspectorService();

        public void Create(UserRegistration user)
        {
            log.Debug("Performing Create on User '" + user.Uuid + "'");

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

                // create the position
                ServiceHelper.UpdatePosition(user);

                // create User object
                brugerStub.Importer(MapRegistrationToUserDTO(user));

                log.Debug("Create successful on User '" + user.Uuid + "'");
            }
            catch (Exception ex) when (ex is STSNotFoundException || ex is ServiceNotFoundException)
            {
                log.Warn("Create on UserService failed for '" + user.Uuid + "' due to unavailable KOMBIT services", ex);
                throw new TemporaryFailureException(ex.Message);
            }
        }

        public void Update(UserRegistration user)
        {
            log.Debug("Performing Update on User '" + user.Uuid + "'");

            ValidateAndEnforceCasing(user);

            try
            {
                var result = brugerStub.GetLatestRegistration(user.Uuid);
                if (result == null)
                {
                    log.Debug("Update on User '" + user.Uuid + "' changed to a Create because it does not exists as an active object within Organisation");
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
                            if (orgAddress.Rolle.Item.Equals(UUIDConstants.ADDRESS_ROLE_USER_PHONE))
                            {
                                orgPhoneUuid = orgAddress.ReferenceID.Item;
                            }
                            else if (orgAddress.Rolle.Item.Equals(UUIDConstants.ADDRESS_ROLE_USER_EMAIL))
                            {
                                orgEmailUuid = orgAddress.ReferenceID.Item;
                            } /*
                            else if (orgAddress.Rolle.Item.Equals(UUIDConstants.ADDRESS_ROLE_LOCATION))
                            {
                                orgLocationUuid = orgAddress.ReferenceID.Item;
                            } */
                        }
                    }

                    // run through all the input addresses, and deal with them one by one
                    ServiceHelper.UpdateAddress(user.Phone, orgPhoneUuid, user.Timestamp);
                    ServiceHelper.UpdateAddress(user.Email, orgEmailUuid, user.Timestamp);
                    ServiceHelper.UpdateAddress(user.Location, orgLocationUuid, user.Timestamp);
                    #endregion

                    #region Update Position if needed
                    ServiceHelper.UpdatePosition(user);
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

                    log.Debug("Update successful on User '" + user.Uuid + "'");
                }
            }
            catch (Exception ex) when (ex is STSNotFoundException || ex is ServiceNotFoundException)
            {
                log.Warn("Update on UserService failed for '" + user.Uuid + "' due to unavailable KOMBIT services", ex);
                throw new TemporaryFailureException(ex.Message);
            }
        }

        public void Delete(string uuid, DateTime timestamp)
        {
            try
            {
                // find the OrgFunctions that represents this users positions within the municipality
                // for each of these OrgFunctions and drop the relationship to both User and OrgUnit from that Function
                List<FiltreretOejebliksbilledeType> unitRoles = ServiceHelper.FindUnitRolesForUser(uuid);
                foreach (FiltreretOejebliksbilledeType unitRole in unitRoles)
                {
                    orgFunctionStub.Deactivate(unitRole.ObjektType.UUIDIdentifikator, timestamp);
                }

                // update the user object by
                //   -> terminating the users relationship to the Organisation
                brugerStub.Deactivate(uuid, timestamp);
            }
            catch (Exception ex) when (ex is STSNotFoundException || ex is ServiceNotFoundException)
            {
                log.Warn("Delete on UserService failed for '" + uuid + "' due to unavailable KOMBIT services", ex);
                throw new TemporaryFailureException(ex.Message);
            }
        }

        public List<string> List()
        {
            log.Debug("Performing List on Users");

            var result = inspectorService.FindAllUsers();

            log.Debug("Found " + result.Count + " Users");

            return result;
        }

        public UserRegistration Read(string uuid)
        {
            log.Debug("Performing Read on User " + uuid);

            UserRegistration registration = null;

            User user = inspectorService.ReadUserObject(uuid, ReadAddresses.YES, ReadParentDetails.NO);
            if (user != null)
            {
                registration = new UserRegistration();
                registration.Uuid = uuid;
                registration.UserId = user.UserId;
                registration.ShortKey = user.ShortKey;
                registration.Person = new DTO.V1_1.Person()
                {
                    Cpr = user.Person.Cpr,
                    Name = user.Person.Name,
                    ShortKey = user.Person.ShortKey,
                    Uuid = user.Person.Uuid
                };

                foreach (var position in user.Positions)
                {
                    DTO.V1_1.Position userPosition = new DTO.V1_1.Position();

                    userPosition.Name = position.Name;
                    userPosition.OrgUnitUuid = position.OU.Uuid;
                    userPosition.ShortKey = position.ShortKey;
                    userPosition.Uuid = position.Uuid;

                    registration.Positions.Add(userPosition);
                }

                foreach (var address in user.Addresses)
                {
                    if (address is Email)
                    {
                        registration.Email.Uuid = address.Uuid;
                        registration.Email.Value = address.Value;
                        registration.Email.ShortKey = address.ShortKey;
                    }
                    else if (address is Location)
                    {
                        registration.Location.Uuid = address.Uuid;
                        registration.Location.Value = address.Value;
                        registration.Location.ShortKey = address.ShortKey;
                    }
                    else if (address is Phone)
                    {
                        registration.Phone.Uuid = address.Uuid;
                        registration.Phone.Value = address.Value;
                        registration.Phone.ShortKey = address.ShortKey;
                    }
                    else
                    {
                        log.Warn("Trying to Read user " + uuid + " with unknown address type " + address.GetType().ToString());
                    }
                }

                log.Debug("Found User " + uuid + " when reading");

                registration.Timestamp = user.Timestamp;
            }
            else
            {
                log.Debug("Did not found User " + uuid + " when reading");
            }

            return registration;
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
            user.PersonUuid = registration.Person.Uuid;
            user.ShortKey = registration.ShortKey;
            user.Timestamp = registration.Timestamp;
            user.UserId = registration.UserId;
            user.Uuid = registration.Uuid;

            return user;
        }

        private void ValidateAndEnforceCasing(UserRegistration registration)
        {
            List<string> errors = new List<string>();

            if (string.IsNullOrEmpty(registration.Person.Name))
            {
                errors.Add("personName");
            }

            foreach (DTO.V1_1.Position position in registration.Positions)
            {
                if (string.IsNullOrEmpty(position.Name))
                {
                    errors.Add("positionName");
                }

                if (string.IsNullOrEmpty(position.OrgUnitUuid))
                {
                    errors.Add("positionOrgUnitUuid");
                }
                else // we need to know there is an OrgUnitUuid to compare with, otherwise this check is pointless
                {
                    // O(n^2), I know, but we will have 1-3 positions on an object, so...
                    foreach (DTO.V1_1.Position p2 in registration.Positions)
                    {
                        if (!position.Equals(p2) && position.OrgUnitUuid.Equals(p2.OrgUnitUuid))
                        {
                            errors.Add("two positions in " + position.OrgUnitUuid);
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(registration.UserId))
            {
                errors.Add("userId");
            }

            if (string.IsNullOrEmpty(registration.Uuid))
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

            if (registration.Person.Uuid != null)
            {
                registration.Person.Uuid = registration.Person.Uuid.ToLower();
            }

            foreach (DTO.V1_1.Position position in registration.Positions)
            {
                position.OrgUnitUuid = position.OrgUnitUuid.ToLower();

                if (position.Uuid != null)
                {
                    position.Uuid = position.Uuid.ToLower();
                }
            }

            if (registration.Person.Cpr != null)
            {
                // strip dashes, so 010101-0101 becomes 010101010101 (KOMBIT requirement)
                registration.Person.Cpr = registration.Person.Cpr.Replace("-", "");
            }

            registration.Uuid = registration.Uuid.ToLower();
        }
    }
}
