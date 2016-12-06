using System;
using System.Collections.Generic;
using Organisation.IntegrationLayer;

namespace Organisation.BusinessLayer
{
   public class OrgUnitService
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private OrganisationEnhedStub organisationEnhedStub = new OrganisationEnhedStub();

        /// <summary>
        /// This method will create the object in Organisation - note that if the object already exists, this method
        /// will fail. If unsure whether the object exists, use Update() instead, as that will fallback to Create
        /// if the object does not exist.
        /// </summary>
        public void Create(OrgUnitRegistration registration)
        {
            log.Debug("Performing Create on OrgUnit '" + registration.Uuid + "'");

            ValidateAndEnforceCasing(registration);

            try
            {
                string uuid = null;

                // Import addresses
                ServiceHelper.ImportAddress(registration.Phone, registration.Timestamp, out uuid);
                if (uuid != null) { registration.Phone.Uuid = uuid; }

                ServiceHelper.ImportAddress(registration.Email, registration.Timestamp, out uuid);
                if (uuid != null) { registration.Email.Uuid = uuid; }

                ServiceHelper.ImportAddress(registration.Location, registration.Timestamp, out uuid);
                if (uuid != null) { registration.Location.Uuid = uuid; }

                ServiceHelper.ImportAddress(registration.LOSShortName, registration.Timestamp, out uuid);
                if (uuid != null) { registration.LOSShortName.Uuid = uuid; }

                ServiceHelper.ImportAddress(registration.EmailRemarks, registration.Timestamp, out uuid);
                if (uuid != null) { registration.EmailRemarks.Uuid = uuid; }

                ServiceHelper.ImportAddress(registration.PostReturn, registration.Timestamp, out uuid);
                if (uuid != null) { registration.PostReturn.Uuid = uuid; }

                ServiceHelper.ImportAddress(registration.Contact, registration.Timestamp, out uuid);
                if (uuid != null) { registration.Contact.Uuid = uuid; }

                ServiceHelper.ImportAddress(registration.PhoneOpenHours, registration.Timestamp, out uuid);
                if (uuid != null) { registration.PhoneOpenHours.Uuid = uuid; }

                ServiceHelper.ImportAddress(registration.ContactOpenHours, registration.Timestamp, out uuid);
                if (uuid != null) { registration.ContactOpenHours.Uuid = uuid; }

                ServiceHelper.ImportAddress(registration.Ean, registration.Timestamp, out uuid);
                if (uuid != null) { registration.Ean.Uuid = uuid; }

                ServiceHelper.ImportAddress(registration.Post, registration.Timestamp, out uuid);
                if (uuid != null) { registration.Post.Uuid = uuid; }

                // Update functions for it-usage
                if (registration.ItSystemUuids != null)
                {
                    foreach (string itSystemUuid in registration.ItSystemUuids)
                    {
                        ServiceHelper.OrgUnitStartUsingItSystem(registration.Uuid, itSystemUuid, registration.Timestamp);
                    }
                }

                // mapping the unit must come after the addresses, as importing the address might set a UUID on the addresses if not supplied by the caller
                OrgUnitData orgUnitData = MapRegistrationToOrgUnitDTO(registration);

                // if this unit is a working unit, that does payouts in behalf of a payout unit, create a reference to that payout unit
                if (!string.IsNullOrEmpty(registration.PayoutUnitUuid))
                {
                    string payoutUnitFunctionUuid = ServiceHelper.EnsurePayoutUnitFunctionExists(registration.PayoutUnitUuid, registration.Timestamp);

                    orgUnitData.OrgFunctionUuids.Add(payoutUnitFunctionUuid);
                }

                organisationEnhedStub.Importer(orgUnitData);

                log.Debug("Create successful on OrgUnit '" + registration.Uuid + "'");
            }
            catch (Exception ex) when (ex is STSNotFoundException || ex is ServiceNotFoundException)
            {
                log.Warn("Create on OrgUnitService failed for '" + registration.Uuid + "' due to unavailable KOMBIT services", ex);
                throw new TemporaryFailureException(ex.Message);
            }
        }

        /// <summary>
        /// This method will perform a soft-delete on the given OrgUnit. As objects are never really deleted within Organisation,
        /// it means that the object will be orphaned in the sense that all direct and indirect relationships to and from the municipalities
        /// organisation object will be severed.
        /// </summary>
        public void Delete(string uuid, DateTime timestamp)
        {
            try
            {
                log.Debug("Performing Delete on OrgUnit '" + uuid + "'");

                // make sure all it-usage is terminated from this OU
                List<string> itSystemsInOrg = ServiceHelper.FindItSystemRolesForOrgUnit(uuid);
                if (itSystemsInOrg != null)
                {
                    foreach (string itSystemUuid in itSystemsInOrg)
                    {
                        ServiceHelper.OrgUnitStopUsingItSystem(uuid, itSystemUuid, timestamp);
                    }
                }

                // drop all relationsships from the OU to anything else
                organisationEnhedStub.Orphan(uuid, timestamp);

                log.Debug("Delete successful on OrgUnit '" + uuid + "'");
            }
            catch (Exception ex) when (ex is STSNotFoundException || ex is ServiceNotFoundException)
            {
                log.Warn("Delete on OrgUnitService failed for '" + uuid + "' due to unavailable KOMBIT services", ex);
                throw new TemporaryFailureException(ex.Message);
            }
        }

        /// <summary>
        /// This method will check whether the OrgUnit already exists inside Organisation, read it if it does, and perform
        /// the correct update (registering the delta-changes between the local object and the org-object). If the object
        /// does not already exist, it will pass the registration to the Create method.
        /// </summary>
        public void Update(OrgUnitRegistration registration)
        {
            log.Debug("Performing Update on OrgUnit '" + registration.Uuid + "'");

            ValidateAndEnforceCasing(registration);

            try
            {
                var result = organisationEnhedStub.GetLatestRegistration(registration.Uuid, true);
                if (result == null)
                {
                    log.Debug("Update on OrgUnit '" + registration.Uuid + "' changed to a Create because it does not exists as an active object within Organisation");
                    Create(registration);
                }
                else
                {
                    #region Update Addresses in Organisation if needed
                    // check what already exists in Organisation - and store the UUIDs of the existing addresses, we will need those later
                    string orgPhoneUuid = null, orgEmailUuid = null, orgLocationUuid = null, orgLOSShortNameUuid = null, orgEanUuid = null, orgContactHoursUuid = null, orgPhoneHoursUuid = null, orgPostUuid = null, orgPostReturnUuid = null, orgContactUuid = null, orgEmailRemarksUuid = null;
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
                            else if (orgAddress.Rolle.Item.Equals(UUIDConstants.ADDRESS_ROLE_LOSSHORTNAME))
                            {
                                orgLOSShortNameUuid = orgAddress.ReferenceID.Item;
                            }
                            else if (orgAddress.Rolle.Item.Equals(UUIDConstants.ADDRESS_ROLE_EAN))
                            {
                                orgEanUuid = orgAddress.ReferenceID.Item;
                            }
                            else if (orgAddress.Rolle.Item.Equals(UUIDConstants.ADDRESS_ROLE_PHONE_OPEN_HOURS))
                            {
                                orgPhoneHoursUuid = orgAddress.ReferenceID.Item;
                            }
                            else if (orgAddress.Rolle.Item.Equals(UUIDConstants.ADDRESS_ROLE_POST))
                            {
                                orgPostUuid = orgAddress.ReferenceID.Item;
                            }
                            else if (orgAddress.Rolle.Item.Equals(UUIDConstants.ADDRESS_ROLE_CONTACT_ADDRESS_OPEN_HOURS))
                            {
                                orgContactHoursUuid = orgAddress.ReferenceID.Item;
                            }
                            else if (orgAddress.Rolle.Item.Equals(UUIDConstants.ADDRESS_ROLE_POST_RETURN))
                            {
                                orgPostReturnUuid = orgAddress.ReferenceID.Item;
                            }
                            else if (orgAddress.Rolle.Item.Equals(UUIDConstants.ADDRESS_ROLE_CONTACT_ADDRESS))
                            {
                                orgContactUuid = orgAddress.ReferenceID.Item;
                            }
                            else if (orgAddress.Rolle.Item.Equals(UUIDConstants.ADDRESS_ROLE_EMAIL_REMARKS))
                            {
                                orgEmailRemarksUuid = orgAddress.ReferenceID.Item;
                            }
                        }
                    }

                    // run through all the input addresses, and deal with them one by one
                    ServiceHelper.UpdateAddress(registration.Phone, orgPhoneUuid, registration.Timestamp);
                    ServiceHelper.UpdateAddress(registration.Email, orgEmailUuid, registration.Timestamp);
                    ServiceHelper.UpdateAddress(registration.Location, orgLocationUuid, registration.Timestamp);
                    ServiceHelper.UpdateAddress(registration.LOSShortName, orgLOSShortNameUuid, registration.Timestamp);
                    ServiceHelper.UpdateAddress(registration.Ean, orgEanUuid, registration.Timestamp);
                    ServiceHelper.UpdateAddress(registration.ContactOpenHours, orgContactHoursUuid, registration.Timestamp);
                    ServiceHelper.UpdateAddress(registration.EmailRemarks, orgEmailRemarksUuid, registration.Timestamp);
                    ServiceHelper.UpdateAddress(registration.PostReturn, orgPostReturnUuid, registration.Timestamp);
                    ServiceHelper.UpdateAddress(registration.Contact, orgContactUuid, registration.Timestamp);
                    ServiceHelper.UpdateAddress(registration.PhoneOpenHours, orgPhoneHoursUuid, registration.Timestamp);
                    ServiceHelper.UpdateAddress(registration.Post, orgPostUuid, registration.Timestamp);
                    #endregion

                    #region Update it-usage
                    List<string> itSystemsInOrg = ServiceHelper.FindItSystemsForOrgUnit(registration.Uuid);

                    // TODO, it is not stopping the old ones, but it is adding the new ones (hurray) - this is a bug in KMD read method, which needs to be fixed before this will work correctly
                    foreach (string itSystemUuid in ServiceHelper.GetItSystemsNoLongerInUse(itSystemsInOrg, registration.ItSystemUuids))
                    {
                        ServiceHelper.OrgUnitStopUsingItSystem(registration.Uuid, itSystemUuid, registration.Timestamp);
                    }

                    foreach (string itSystemUuid in ServiceHelper.GetItSystemsNewInUse(itSystemsInOrg, registration.ItSystemUuids))
                    {
                        ServiceHelper.OrgUnitStartUsingItSystem(registration.Uuid, itSystemUuid, registration.Timestamp);
                    }
                    #endregion

                    // this must happen after addresses have been imported, as it might result in UUID's being created
                    OrgUnitData orgUnitData = MapRegistrationToOrgUnitDTO(registration);

                    #region Update payout units
                    // if this unit is a working unit, that does payouts in behalf of a payout unit, create a reference to that payout unit
                    if (!string.IsNullOrEmpty(registration.PayoutUnitUuid))
                    {
                        string payoutUnitFunctionUuid = ServiceHelper.EnsurePayoutUnitFunctionExists(registration.PayoutUnitUuid, registration.Timestamp);

                        orgUnitData.OrgFunctionUuids.Add(payoutUnitFunctionUuid);
                    }
                    #endregion

                    organisationEnhedStub.Ret(orgUnitData);

                    log.Debug("Update successful on OrgUnit '" + registration.Uuid + "'");
                }
            }
            catch (Exception ex) when (ex is STSNotFoundException || ex is ServiceNotFoundException)
            {
                log.Warn("Update on OrgUnitService failed for '" + registration.Uuid + "' due to unavailable KOMBIT services", ex);
                throw new TemporaryFailureException(ex.Message);
            }
        }

        private OrgUnitData MapRegistrationToOrgUnitDTO(OrgUnitRegistration registration)
        {
            // loop through all address information in the input, and build the corresponding relations for OrgUnit object
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

            if (registration.LOSShortName != null)
            {
                AddressRelation address = new AddressRelation();
                address.Type = AddressRelationType.LOSSHORTNAME;
                address.Uuid = registration.LOSShortName.Uuid;
                addressRelations.Add(address);
            }

            if (registration.Ean != null)
            {
                AddressRelation address = new AddressRelation();
                address.Type = AddressRelationType.EAN;
                address.Uuid = registration.Ean.Uuid;
                addressRelations.Add(address);
            }

            if (registration.Post != null)
            {
                AddressRelation address = new AddressRelation();
                address.Type = AddressRelationType.POST;
                address.Uuid = registration.Post.Uuid;
                addressRelations.Add(address);
            }

            if (registration.ContactOpenHours != null)
            {
                AddressRelation address = new AddressRelation();
                address.Type = AddressRelationType.CONTACT_ADDRESS_OPEN_HOURS;
                address.Uuid = registration.ContactOpenHours.Uuid;
                addressRelations.Add(address);
            }

            if (registration.EmailRemarks != null)
            {
                AddressRelation address = new AddressRelation();
                address.Type = AddressRelationType.EMAIL_REMARKS;
                address.Uuid = registration.EmailRemarks.Uuid;
                addressRelations.Add(address);
            }

            if (registration.PostReturn != null)
            {
                AddressRelation address = new AddressRelation();
                address.Type = AddressRelationType.POST_RETURN;
                address.Uuid = registration.PostReturn.Uuid;
                addressRelations.Add(address);
            }

            if (registration.Contact != null)
            {
                AddressRelation address = new AddressRelation();
                address.Type = AddressRelationType.CONTACT_ADDRESS;
                address.Uuid = registration.Contact.Uuid;
                addressRelations.Add(address);
            }

            if (registration.PhoneOpenHours != null)
            {
                AddressRelation address = new AddressRelation();
                address.Type = AddressRelationType.PHONE_OPEN_HOURS;
                address.Uuid = registration.PhoneOpenHours.Uuid;
                addressRelations.Add(address);
            }

            OrgUnitData organisationEnhed = new OrgUnitData();
            organisationEnhed.ShortKey = registration.ShortKey;
            organisationEnhed.Name = registration.Name;
            organisationEnhed.Addresses = addressRelations;
            organisationEnhed.Timestamp = registration.Timestamp;
            organisationEnhed.Uuid = registration.Uuid;
            organisationEnhed.ParentOrgUnitUuid = registration.ParentOrgUnitUuid;

            return organisationEnhed;
        }

        private void ValidateAndEnforceCasing(OrgUnitRegistration registration)
        {
            List<string> errors = new List<string>();

            if (string.IsNullOrEmpty(registration.Name))
            {
                errors.Add("name");
            }

            if (string.IsNullOrEmpty(registration.Uuid))
            {
                errors.Add("uuid");
            }

            if (registration.Timestamp == null)
            {
                errors.Add("timestamp");
            }

            if (errors.Count > 0)
            {
                throw new InvalidFieldsException("Invalid registration object - the following fields are invalid: " + string.Join(",", errors));
            }

            if (registration.Phone != null && string.IsNullOrEmpty(registration.Phone.Value))
            {
                registration.Phone = null;
            }

            if (registration.Email != null && string.IsNullOrEmpty(registration.Email.Value))
            {
                registration.Email = null;
            }

            if (registration.Location != null && string.IsNullOrEmpty(registration.Location.Value))
            {
                registration.Location = null;
            }

            if (registration.Post != null && string.IsNullOrEmpty(registration.Post.Value))
            {
                registration.Post = null;
            }

            if (registration.LOSShortName != null && string.IsNullOrEmpty(registration.LOSShortName.Value))
            {
                registration.LOSShortName = null;
            }

            if (registration.PhoneOpenHours != null && string.IsNullOrEmpty(registration.PhoneOpenHours.Value))
            {
                registration.PhoneOpenHours = null;
            }

            if (registration.ContactOpenHours != null && string.IsNullOrEmpty(registration.ContactOpenHours.Value))
            {
                registration.ContactOpenHours = null;
            }

            if (registration.EmailRemarks != null && string.IsNullOrEmpty(registration.EmailRemarks.Value))
            {
                registration.EmailRemarks = null;
            }

            if (registration.PostReturn != null && string.IsNullOrEmpty(registration.PostReturn.Value))
            {
                registration.PostReturn = null;
            }

            if (registration.Contact != null && string.IsNullOrEmpty(registration.Contact.Value))
            {
                registration.Contact = null;
            }

            if (registration.Ean != null && string.IsNullOrEmpty(registration.Ean.Value))
            {
                registration.Ean = null;
            }

            if (registration.Email?.Uuid != null)
            {
                registration.Email.Uuid = registration.Email.Uuid.ToLower();
            }

            if (registration.Location?.Uuid != null)
            {
                registration.Location.Uuid = registration.Location.Uuid.ToLower();
            }

            if (registration.LOSShortName?.Uuid != null)
            {
                registration.LOSShortName.Uuid = registration.LOSShortName.Uuid.ToLower();
            }

            if (registration.Post?.Uuid != null)
            {
                registration.Post.Uuid = registration.Post.Uuid.ToLower();
            }

            if (registration.Phone?.Uuid != null)
            {
                registration.Phone.Uuid = registration.Phone.Uuid.ToLower();
            }

            if (registration.PayoutUnitUuid != null)
            {
                registration.PayoutUnitUuid = registration.PayoutUnitUuid.ToLower();
            }

            if (registration.ParentOrgUnitUuid != null)
            {
                registration.ParentOrgUnitUuid = registration.ParentOrgUnitUuid.ToLower();
            }

            if (registration.ItSystemUuids != null)
            {
                List<string> lowerCaseValues = new List<string>();

                foreach (string uuid in registration.ItSystemUuids)
                {
                    lowerCaseValues.Add(uuid.ToLower());
                }

                registration.ItSystemUuids = lowerCaseValues;
            }

            registration.Uuid = registration.Uuid.ToLower();
        }
    }
}