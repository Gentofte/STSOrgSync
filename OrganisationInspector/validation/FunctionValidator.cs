using Organisation.IntegrationLayer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrganisationInspector
{
    /// <summary>
    /// Validate Function according to Kombit defined rules
    /// 
    /// </summary>
    /// 
     /*
        OrganisationFunktion
      - Relations that must NOT be filled out
        - TilknyttedeInteresseFaellesskaber [X]
        - TilknyttedePersoner [X]
      - Relations that MUST be filled out
        - Funktionstype(and it must be validated that this points to one of the four known types)[X]
          - the four known types are POSITION, IT_USAGE, PAYOUT_UNIT, CONTACT_UNIT [X]
   
        After this, we validate depending on the type
    - Type = POSITION
    - Allowed Relationships
        - TilknyttedeBrugere (optional, so 0 or more is allowed) [X] (no rule)
        - TilknytedeEnheder (exactly 1 requried) [X]
    - Prohibited Relationships
        - TilknyttedeItSystemer [X]
        - Opgaver [X]
    - Type = IT_USAGE
    - Allowed Relationships
        - TilknyttedeItSystemer (exactly 1 required) [X]
        - TilknytedeEnheder
    - Prohibited Relationships
        - Opgaver [X]
        - TilknyttedeBrugere [X]
    - Type = PAYOUT_UNIT
    - Allowed Relationships
        - TilknytedeEnheder (exactly 1 is required)
            - the OrgUnit that is referenced must be read, and it must be validated that the Unit has a LOSShortName address relationship [X]
    - Prohibited Relationships
        - TilknyttedeItSystemer [X]
        - Opgaver [X]
        - TilknyttedeBrugere [X]
    - TYPE = CONTACT_UNIT
    - Allowed Relationships
        - Opgaver (at least 1 relationship is required, more is allowed) [X]
        - TilknytedeEnheder (exactly 1 is required) [X]
    - Prohibited Relationships
        - TilknyttedeItSystemer [X]
        - TilknyttedeBrugere [X]
        */
    public class FunctionValidator : Validator
    {
        private string organisationUuid;
        private OrganisationFunktionStub organisationFunktionStub = new OrganisationFunktionStub();
        private OrganisationEnhedStub organisationEnhedStub = new OrganisationEnhedStub();

        private const string SHOULD_NOT_EXIST_TILKNYTTEDEINTERESSEFAELLESSKABER = "TilknyttedeInteressefaellesskaber relation should not exist";
        private const string SHOULD_NOT_EXIST_TILKNYTTEDEPERSONER = "TilknyttededePersoner relation should not exist";
        private const string SHOULD_EXIST_FUNKTIONSTYPE = "TilknyttededePersoner relation should not exist";
        private const string BAD_TYPE_ORGFUN = "Bad function type {0}";
        private const string BAD_ORGFUN_POSITION = "Functions of type Position can only have one OrgUnit relation";
        private const string BAD_ORGFUN_POSITION_TILKNYTTEDEITSYSTEMER = "Functions of type Position cannot have TilknyttedeItSystemer";
        private const string BAD_ORGFUN_POSITION_OPGAVER = "Functions of type Position cannot have Opgaver";
        private const string BAD_ORGFUN_ITUSAGE_TILKNYTTEDEITSYSTEMER = "Functions of type ITusage should have only one TilknyttedeItSystemer";
        private const string BAD_ORGFUN_ITUSAGE_OPGAVER = "Functions of type ITusage should not have Opgaver";
        private const string BAD_ORGFUN_POSITION_TILKNYTTEDEBRUGER = "Functions of type ITusage should not have TilknyttedeBruger";
        private const string BAD_ORGFUN_PAYOUT_UNIT_TILKNYTTEDENHEDER = "Functions of type PayoutUnit should have only one TilknyttededeEnheder";
        private const string BAD_ORGFUN_PAYOUT_UNIT_LOSSNAME_MISSING = "Functions of type PayoutUnit should point to a OrgUnit that has a LOSSShortName";
        private const string BAD_ORGFUN_PAYOUTUNIT_OPGAVER = "Functions of type PayoutUnit cannot have Opgaver";
        private const string BAD_ORGFUN_PAYOUTUNIT_TILKNYTTEDEBRUGER = "Functions of type PayoutUnit cannot have TilknyttedeBrugere";
        private const string BAD_ORGFUN_PAYOUTUNIT_TILKNYTTEDEITSYSTEMER = "Functions of type PayoutUnit cannot have TilknyttedeItSystemer";
        private const string BAD_ORGFUN_CONTACT_UNIT_TILKNYTTEDEENHEDER = "Functions of type ContactUnit can only have one TilknyttedeEnheder";
        private const string BAD_ORGFUN_CONTACT_UNIT_OPGAVER = "Functions of type ContactUnit should have one or more Opgaver";
        private const string BAD_ORGFUN_CONTACT_UNIT_TILKNYTTEDEITSYSTEMER = "Functions of type ContactUnit should not have TilknyttedeItSystemer";
        private const string BAD_ORGFUN_CONTACT_UNIT_TILKNYTTEDEBRUGER = "Functions of type ContactUnit should not TilknyttedeBrugere ";

        public FunctionValidator(string uuid)
        {
            this.organisationUuid = uuid;
        }

        public List<string> Validate(string uuid)
        {
            List<String> errors = new List<string>();
            var orgFunction = organisationFunktionStub.GetLatestRegistration(uuid);

            if (orgFunction.RelationListe.TilknyttedeInteressefaellesskaber != null)
            {
                errors.Add(SHOULD_NOT_EXIST_TILKNYTTEDEINTERESSEFAELLESSKABER);
            }
            if (orgFunction.RelationListe.TilknyttedePersoner != null)
            {
                errors.Add(SHOULD_NOT_EXIST_TILKNYTTEDEPERSONER);
            }
            if (orgFunction.RelationListe.Funktionstype == null)
            {
                errors.Add(SHOULD_EXIST_FUNKTIONSTYPE);
            }
            else
            {
                string type = orgFunction.RelationListe.Funktionstype.ReferenceID.Item;
                if (!(UUIDConstants.ORGFUN_CONTACT_UNIT.Equals(type) ||
                        UUIDConstants.ORGFUN_IT_USAGE.Equals(type) ||
                        UUIDConstants.ORGFUN_PAYOUT_UNIT.Equals(type) ||
                        UUIDConstants.ORGFUN_POSITION.Equals(type)) )
                {
                    errors.Add(string.Format(BAD_TYPE_ORGFUN, type));
                }

                if (UUIDConstants.ORGFUN_POSITION.Equals(type))
                {
                    if (orgFunction.RelationListe.TilknyttedeEnheder == null || orgFunction.RelationListe.TilknyttedeEnheder.Length != 1)
                    {
                        errors.Add(BAD_ORGFUN_POSITION);
                    }

                    if (orgFunction.RelationListe.TilknyttedeItSystemer != null)
                    {
                        errors.Add(BAD_ORGFUN_POSITION_TILKNYTTEDEITSYSTEMER);
                    }

                    if (orgFunction.RelationListe.Opgaver != null)
                    {
                        errors.Add(BAD_ORGFUN_POSITION_OPGAVER);
                    }
                }

                if (UUIDConstants.ORGFUN_IT_USAGE.Equals(type))
                {
                    if (orgFunction.RelationListe.TilknyttedeItSystemer == null || orgFunction.RelationListe.TilknyttedeItSystemer.Length != 1)
                    {
                        errors.Add(BAD_ORGFUN_ITUSAGE_TILKNYTTEDEITSYSTEMER);
                    }

                    if (orgFunction.RelationListe.Opgaver != null)
                    {
                        errors.Add(BAD_ORGFUN_ITUSAGE_OPGAVER);
                    }

                    if (orgFunction.RelationListe.TilknyttedeBrugere != null)
                    {
                        errors.Add(BAD_ORGFUN_POSITION_TILKNYTTEDEBRUGER);
                    }
                }

                if (UUIDConstants.ORGFUN_PAYOUT_UNIT.Equals(type))
                {
                    if (orgFunction.RelationListe.TilknyttedeEnheder == null || orgFunction.RelationListe.TilknyttedeEnheder.Length != 1)
                    {
                        errors.Add(BAD_ORGFUN_PAYOUT_UNIT_TILKNYTTEDENHEDER);
                    }
                    else
                    {
                        string orgEnhedUUID = orgFunction.RelationListe.TilknyttedeEnheder[0].ReferenceID.Item;
                        var orgEnhed = organisationEnhedStub.GetLatestRegistration(orgEnhedUUID);

                        bool found = false;
                        foreach (var address in orgEnhed?.RelationListe?.Adresser ?? Enumerable.Empty<global::IntegrationLayer.OrganisationEnhed.AdresseFlerRelationType>())
                        {
                            string role = address.Rolle?.Item;

                            if (UUIDConstants.ADDRESS_ROLE_ORGUNIT_LOSSHORTNAME.Equals(role))
                            {
                                found = true;
                            }
                        }

                        if (!found)
                        {
                            errors.Add(BAD_ORGFUN_PAYOUT_UNIT_LOSSNAME_MISSING);
                        }
                    }

                    if (orgFunction.RelationListe.Opgaver != null)
                    {
                        errors.Add(BAD_ORGFUN_PAYOUTUNIT_TILKNYTTEDEITSYSTEMER);
                    }

                    if (orgFunction.RelationListe.Opgaver != null)
                    {
                        errors.Add(BAD_ORGFUN_PAYOUTUNIT_OPGAVER);
                    }

                    if (orgFunction.RelationListe.TilknyttedeBrugere != null)
                    {
                        errors.Add(BAD_ORGFUN_PAYOUTUNIT_TILKNYTTEDEBRUGER);
                    }
                }
                if (UUIDConstants.ORGFUN_CONTACT_UNIT.Equals(type))
                {
                    if (orgFunction.RelationListe.Opgaver == null || orgFunction.RelationListe.Opgaver.Length < 1)
                    {
                        errors.Add(BAD_ORGFUN_CONTACT_UNIT_OPGAVER);
                    }

                    if (orgFunction.RelationListe.TilknyttedeEnheder == null || orgFunction.RelationListe.TilknyttedeEnheder.Length != 1)
                    {
                        errors.Add(BAD_ORGFUN_CONTACT_UNIT_TILKNYTTEDEENHEDER);
                    }

                    if (orgFunction.RelationListe.TilknyttedeItSystemer != null)
                    {
                        errors.Add(BAD_ORGFUN_CONTACT_UNIT_TILKNYTTEDEITSYSTEMER);
                    }

                    if (orgFunction.RelationListe.TilknyttedeBrugere != null)
                    {
                        errors.Add(BAD_ORGFUN_CONTACT_UNIT_TILKNYTTEDEBRUGER);
                    }
                }

            }

            return errors;
        }
    }
}
