using Organisation.IntegrationLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using IntegrationLayer.OrganisationEnhed;

namespace OrganisationInspector
{
    /*
    - Relations that must NOT be filled out
        - Ansatte [X]
        - Branche [X]
        - Produktionsenhed [X]
        - Skatteenhed [X]
        - TilknyttedeBrugere [X]
        - TilknyttedeEnheder 
        - TilknyttedeInteressefællesskaber [X]
        - TilknyttedeOrganisationer [X]
        - TilknyttedePersoner [X]
        - TilknyttedeItSystemer [X]
    - Relationships that must be validated IF filled out
     - TilknyttedeFunktioner
      - special validation rules, the only two types of functions allowed are PAYOUT_UNIT and CONTACT_UNIT [X]
        - validation rule for PAYOUT_UNIT
          - there can only be one such relationships (but 0 are allowed) [X]
        - validation rule for CONTACT_UNIT
          - there can be multiple such relationships (but 0 are allowed)
          - very important validation rule: there cannot be overlapping functions for KLE codes [[X]
        - Adresser (the role/type attributes on the relation should be validated to be known types (as for Bruger) [X]
    - Relationships that MUST be be filled out 
        - Tilhører (and it MUST point to the configured OrganisationUUID) [X]
     */
    public class OrgUnitValidator : Validator
    {
        private OrganisationEnhedStub organisationEnhedStub = new OrganisationEnhedStub();
        private OrganisationFunktionStub organisationFunktionStub = new OrganisationFunktionStub();
        private string organisationUuid;

        private const string BRUGERVENDT_NOEGLE_TEKS_NULL_MSG = "BrugervendtNoegleTekst is null";
        private const string ROLE_MSG = "For Address ({0}), there is an unknown role ({1})";
        private const string TYPE_MSG = "For Address ({0}), there is an unknown type ({1})";
        private const string ANSATTE_RELATION_MSG = "Ansatte relation should not exist";
        private const string BRANCHE_RELATION_MSG = "Branche relation should not exist";
        private const string PRODUKTIONS_ENHED_MSG = "Produktion relation should not exist";
        private const string SKATTEENHED_MSG = "Skatteenhed relation should not exist";
        private const string TILHOERER_NULL_MSG = "No Tilhører relation was found";
        private const string TILHOERER_BAD_RELATION_MSG = "Tilhoerer must point to {0} but it points to {1}";
        private const string TILHOERER_TILKNYTTEDE_BRUGER_MSG = "TilknyttedeBrugere should not exist";
        private const string TILKNYTTEDE_INTERESSE_FAELLESSKABER_MSG = "TilknyttedeInteressefaellesskaber should not exist";
        private const string TILKNYTTEDE_ORGANISATIONER_MSG = "TilknyttedeOrganisationer should not exist";
        private const string TILKNYTTEDE_PERSONER_MSG = "TilknyttedePersoner should not exist";
        private const string TILKNYTTEDE_ITSYSTEMER_MSG = "TilknyttedeItSystemer should not exist";
        private const string INVALID_FUNKTION_TYPE = "TilknyttedeFunktioner has unknown funktionstype {0}";
        private const string TOO_MANY_PAYOUT_UNITS = "Too many payout units, only one is allowed";
        private const string CONFLICTING_KLE = "Conflicting kle {0}.";

        public OrgUnitValidator(string uuid)
        {
            this.organisationUuid = uuid;
        }

        public List<string> Validate(string uuid)
        {
            List<String> errors = new List<string>();
            RegistreringType1 orgEnhed = organisationEnhedStub.GetLatestRegistration(uuid);

            if (string.IsNullOrEmpty(orgEnhed.AttributListe?.Egenskab?[0]?.BrugervendtNoegleTekst))
            {
                errors.Add(BRUGERVENDT_NOEGLE_TEKS_NULL_MSG);
            }
            foreach (var address in orgEnhed.RelationListe.Adresser ?? Enumerable.Empty<AdresseFlerRelationType>())
            {
                string role = address.Rolle?.Item;
                string type = address.Type?.Item;

                if (!ValidateOrgUnitAddressRole(role))
                {
                    errors.Add(string.Format(ROLE_MSG, address.ReferenceID.Item, role));
                }
                if (!ValidateOrgUnitAddressType(type))
                {
                    errors.Add(string.Format(TYPE_MSG, address.ReferenceID.Item, type));
                }
            }

            if (orgEnhed.RelationListe.Ansatte != null)
            {
                errors.Add(ANSATTE_RELATION_MSG);
            }
            if (orgEnhed.RelationListe.Branche != null)
            {
                errors.Add(BRANCHE_RELATION_MSG);
            }
            if (orgEnhed.RelationListe.Produktionsenhed != null)
            {
                errors.Add(PRODUKTIONS_ENHED_MSG);
            }
            if (orgEnhed.RelationListe.Skatteenhed != null)
            {
                errors.Add(SKATTEENHED_MSG);
            }
            if (orgEnhed.RelationListe.Tilhoerer == null)
            {
                errors.Add(TILHOERER_NULL_MSG);
            }
            else if (!orgEnhed.RelationListe.Tilhoerer.ReferenceID.Item.Equals(organisationUuid))
            {
                errors.Add(string.Format(TILHOERER_BAD_RELATION_MSG, organisationUuid, orgEnhed.RelationListe.Tilhoerer.ReferenceID.Item));
            }

            if (orgEnhed.RelationListe.TilknyttedeBrugere != null)
            {
                errors.Add(TILHOERER_TILKNYTTEDE_BRUGER_MSG);
            }
            if (orgEnhed.RelationListe.TilknyttedeInteressefaellesskaber != null)
            {
                errors.Add(TILKNYTTEDE_INTERESSE_FAELLESSKABER_MSG);
            }
            if (orgEnhed.RelationListe.TilknyttedeOrganisationer != null)
            {
                errors.Add(TILKNYTTEDE_ORGANISATIONER_MSG);
            }
            if (orgEnhed.RelationListe.TilknyttedePersoner != null)
            {
                errors.Add(TILKNYTTEDE_PERSONER_MSG);
            }
            if (orgEnhed.RelationListe.TilknyttedeItSystemer != null)
            {
                errors.Add(TILKNYTTEDE_ITSYSTEMER_MSG);
            }
            if (orgEnhed.RelationListe.TilknyttedeFunktioner != null)
            {
                ValidateTilknyttedeFunktionerTypes(errors, orgEnhed.RelationListe.TilknyttedeFunktioner);
            }

            return errors;
        }



        private void ValidateTilknyttedeFunktionerTypes(List<string> errors, OrganisationFunktionFlerRelationType[] tilknyttedeFunktioner)
        {
            int payoutUnitCount = 0;
            List<string> known_kles = new List<string>();
            foreach (OrganisationFunktionFlerRelationType orgFunc in tilknyttedeFunktioner ?? Enumerable.Empty<OrganisationFunktionFlerRelationType>())
            {
                // can only be PAYOUT_UNIT or CONTACT_UNIT
                string funcUUID = orgFunc.ReferenceID.Item;
                var orgFuncRegistration = organisationFunktionStub.GetLatestRegistration(funcUUID);
                string type = orgFuncRegistration.RelationListe?.Funktionstype?.ReferenceID?.Item;

                if (!UUIDConstants.ORGFUN_PAYOUT_UNIT.Equals(type) && !UUIDConstants.ORGFUN_CONTACT_UNIT.Equals(type))
                {
                    errors.Add(string.Format(INVALID_FUNKTION_TYPE, type));
                }
                else if (UUIDConstants.ORGFUN_PAYOUT_UNIT.Equals(type))
                {
                    payoutUnitCount++;
                }
                else if (UUIDConstants.ORGFUN_CONTACT_UNIT.Equals(type))
                {
                    if (orgFuncRegistration.RelationListe.Opgaver != null)
                    {
                        foreach (var task in orgFuncRegistration.RelationListe.Opgaver)
                        {
                            string kle = task?.ReferenceID?.Item;
                            if (kle != null)
                            {
                                if (known_kles.Contains(kle))
                                {
                                    errors.Add(string.Format(CONFLICTING_KLE, kle));
                                }
                                else
                                {
                                    known_kles.Add(kle);
                                }
                            }
                        }
                    }
                }
            }

            if (payoutUnitCount > 1)
            {
                errors.Add(TOO_MANY_PAYOUT_UNITS);
            }
        }

        private bool ValidateOrgUnitAddressType(string type)
        {
            return type.Equals(UUIDConstants.ADDRESS_TYPE_ORGUNIT);
        }

        private bool ValidateOrgUnitAddressRole(string rolle)
        {
            return (rolle.Equals(UUIDConstants.ADDRESS_ROLE_ORGUNIT_CONTACT_ADDRESS) ||
                rolle.Equals(UUIDConstants.ADDRESS_ROLE_ORGUNIT_CONTACT_ADDRESS_OPEN_HOURS) ||
                rolle.Equals(UUIDConstants.ADDRESS_ROLE_ORGUNIT_EAN) ||
                rolle.Equals(UUIDConstants.ADDRESS_ROLE_ORGUNIT_URL) ||
                rolle.Equals(UUIDConstants.ADDRESS_ROLE_ORGUNIT_EMAIL) ||
                rolle.Equals(UUIDConstants.ADDRESS_ROLE_ORGUNIT_EMAIL_REMARKS) ||
                rolle.Equals(UUIDConstants.ADDRESS_ROLE_ORGUNIT_LOCATION) ||
                rolle.Equals(UUIDConstants.ADDRESS_ROLE_ORGUNIT_LOSSHORTNAME) ||
                rolle.Equals(UUIDConstants.ADDRESS_ROLE_ORGUNIT_PHONE) ||
                rolle.Equals(UUIDConstants.ADDRESS_ROLE_ORGUNIT_PHONE_OPEN_HOURS) ||
                rolle.Equals(UUIDConstants.ADDRESS_ROLE_ORGUNIT_POST) ||
                rolle.Equals(UUIDConstants.ADDRESS_ROLE_ORGUNIT_POST_RETURN));
        }
    }
}
