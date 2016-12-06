using System;
using System.Collections.Generic;
using Organisation.IntegrationLayer;

namespace Organisation.BusinessLayer
{
    /* DEPRECATED - KOMBIT decided against registered information like JumpURL on it-systems. We will keep the code, just in case KOMBIT redecides again
    public class ItSystemService
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private OrganisationFunktionStub organisationFunktionStub = new OrganisationFunktionStub();

        public void Update(ItSystemRegistration registration)
        {
            log.Debug("Performing Update on OrgFunctions for ItSystem '" + registration.Uuid + "'");

            ValidateAndEnforceCasing(registration);

            List<string> existingOrgUnitFunctions = ServiceHelper.FindOrgUnitRolesForItSystem(registration.Uuid);
            if (existingOrgUnitFunctions.Count > 0)
            {
                if (existingOrgUnitFunctions.Count > 1)
                {
                    log.Warn("There is more than one function of type 'it-usage' pointing to it-system '" + registration.Uuid + "'");
                }

                UpdateFunctionForOrgUnits(registration, existingOrgUnitFunctions[0]);
            }
            else
            {
                CreateFunctionForOrgUnits(registration);
            }

            log.Debug("Update on OrgFunctions for ItSystem '" + registration.Uuid + "' succeeded");
        }

        // we do not actually delete the function, instead we set the AttributListes Virkning to ended, effectively removing the function according to KOMBIT
        public void Delete(string uuid, DateTime timestamp)
        {
            log.Debug("Performing Delete on OrgFunctions for ItSystem '" + uuid + "'");

            List<string> existingOrgUnitFunctions = ServiceHelper.FindOrgUnitRolesForItSystem(uuid);
            if (existingOrgUnitFunctions.Count > 0)
            {
                // there should only be one, but let us for good effect end all of them
                foreach (string existingOrgUnitFunction in existingOrgUnitFunctions)
                {
                    organisationFunktionStub.Orphan(existingOrgUnitFunction, timestamp);
                }
            }

            log.Debug("Delete on OrgFunctions for ItSystem '" + uuid + "' succeeded");
        }

        private void UpdateFunctionForOrgUnits(ItSystemRegistration registration, string existingFunctionUuid)
        {
            var result = organisationFunktionStub.GetLatestRegistration(existingFunctionUuid, true);

            // check what already exists in Organisation - and store the UUIDs of the existing addresses, we will need those later
            string orgUrlAddress = null;
            if (result.RelationListe.Adresser != null)
            {
                foreach (var orgAddress in result.RelationListe.Adresser)
                {
                    if (orgAddress.Rolle.Item.Equals(UUIDConstants.ADDRESS_ROLE_URL))
                    {
                        orgUrlAddress = orgAddress.ReferenceID.Item;
                    }
                }
            }

            List<AddressRelation> addressRelations = new List<AddressRelation>();
            if (!string.IsNullOrEmpty(registration.JumpUrl))
            {
                Address jumpUrl = new Address();
                jumpUrl.Value = registration.JumpUrl;

                ServiceHelper.UpdateAddress(jumpUrl, orgUrlAddress, registration.Timestamp);

                AddressRelation address = new AddressRelation();
                address.Type = AddressRelationType.URL;
                address.Uuid = jumpUrl.Uuid;
                addressRelations.Add(address);
            }

            OrgFunctionData orgFunction = new OrgFunctionData()
            {
                Addresses = addressRelations,
                Timestamp = registration.Timestamp,
                Uuid = existingFunctionUuid
            };

            organisationFunktionStub.Ret(orgFunction, UpdateIndicator.NONE, UpdateIndicator.NONE);
        }

        private void CreateFunctionForOrgUnits(ItSystemRegistration registration)
        {
            List<string> itSystem = new List<string>() { registration.Uuid };
            List<AddressRelation> addressRelations = new List<AddressRelation>();

            if (!string.IsNullOrEmpty(registration.JumpUrl))
            {
                Address jumpUrl = new Address();
                jumpUrl.Value = registration.JumpUrl;
                string uuid = null;

                ServiceHelper.ImportAddress(jumpUrl, registration.Timestamp, out uuid);

                AddressRelation address = new AddressRelation();
                address.Type = AddressRelationType.URL;
                address.Uuid = uuid;
                addressRelations.Add(address);
            }

            organisationFunktionStub.Importer(new IntegrationLayer.OrgFunctionData()
            {
                Uuid = IdUtil.GenerateUuid(),
                ShortKey = registration.SystemShortKey,
                Name = registration.SystemShortKey,
                FunctionTypeUuid = UUIDConstants.ORGFUN_IT_USAGE,
                OrgUnits = null,
                Users = null,
                ItSystems = itSystem,
                Addresses = addressRelations,
                Timestamp = registration.Timestamp
            });
        }

        private void ValidateAndEnforceCasing(ItSystemRegistration registration)
        {
            List<string> errors = new List<string>();

            if (string.IsNullOrEmpty(registration.Uuid))
            {
                errors.Add("uuid");
            }

            if (string.IsNullOrEmpty(registration.SystemShortKey))
            {
                errors.Add("systemShortKey");
            }

            if (registration.Timestamp == null)
            {
                errors.Add("timestamp");
            }

            if (errors.Count > 0)
            {
                throw new Exception("Invalid registration object - the following fields are invalid: " + string.Join(",", errors));
            }

            registration.Uuid = registration.Uuid.ToLower();
        }
    }
    */
}
