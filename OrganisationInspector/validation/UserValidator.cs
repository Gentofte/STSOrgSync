using Organisation.IntegrationLayer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrganisationInspector
{
    /// <summary>
    /// Validate User (Bruger) according to Kombit defined rules
    /// 
    /// </summary>
    /// 

    /*
     Bruger
       - Attribute fields that must NOT be filled out
         - BrugerTypeTekst [X]
       - Relations that MUST be filled out
         - Tilhører(and it MUST point to the configured OrganisationUUID) [X]
         - TilknyttedePersoner(either 0 or 1 is allowed, but not more than 1) [X]
       - Relations that must NOT be filled out
         - BrugerTyper [X]
         - Opgaver [X]
         - TilknyttedeEnheder [X]
         - TilknyttedeFunktioner [X]
         - TilknyttedeInteresseFaellesskaber [X]
         - TilknyttedeOrganisationer [X]
         - TilknyttedeItSystemer [X]
       - Relations that have to be validated if they are filled out
         - Adresser(the role/type attributes on the relation should be validated to be known types) [ ] 
     */
    public class UserValidator : Validator
    {
        private BrugerStub brugerStub = new BrugerStub();
        private string organisationUuid;

        private const string TYPE_MSG = "For Address ({0}), there is an unknown type ({1})";
        private const string ROLE_MSG = "For Address ({0}), there is an unknown role ({1})";
        private const string BRUGERTYPE_TEKST_MSG = "BrugertypeTekst should not exist";
        private const string BRUGERTYPER_MSG = "Brugertyper should not exist";
        private const string OPGAVER_MSG = "Opgaver should not exist";
        private const string TILHOERER_NULL_MSG = "No Tilhører relation was found";
        private const string TILHOERER_BAD_RELATION_MSG = "Tilhoerer must point to {0} but it points to {1}";
        private const string TILHOERER_TILKNYTTEDE_ENHEDER_MSG = "TilknyttedeEnheder should not exist";
        private const string TILHOERER_TILKNYTTEDE_FUNKTIONER = "TilknyttedeFunktioner should not exist";
        private const string TILKNYTTEDE_INTERESSE_FAELLESSKABER_MSG = "TilknyttedeInteressefaellesskaber should not exist";
        private const string TILKNYTTEDE_ITSYSTEMER_MSG = "TilknyttedeItSystemer should not exist";
        private const string TILKNYTTEDE_ORGANISATIONER_MSG = "TilknyttedeOrganisationer should not exist";
        private const string TILKNYTTEDE_PERSONER_MSG = "There should not be more than one entry TilknyttedePersoner";
        public UserValidator(string uuid)
        {
            this.organisationUuid = uuid;
        }
        public List<string> Validate(string uuid)
        {

            List<String> errors = new List<string>();
            IntegrationLayer.Bruger.RegistreringType1 bruger = brugerStub.GetLatestRegistration(uuid, false);
            if (!string.IsNullOrEmpty(bruger.AttributListe.Egenskab?[0]?.BrugerTypeTekst))
            {
                errors.Add(BRUGERTYPE_TEKST_MSG);
            }

            if (bruger.RelationListe?.BrugerTyper != null)
            {
                errors.Add(BRUGERTYPER_MSG);
            }

            if (bruger.RelationListe?.Opgaver != null)
            {
                errors.Add(OPGAVER_MSG);
            }

            if (bruger.RelationListe.Tilhoerer == null)
            {
                errors.Add(TILHOERER_NULL_MSG);
            }
            else if (!bruger.RelationListe.Tilhoerer.ReferenceID.Item.Equals(organisationUuid))
            {
                errors.Add(string.Format(TILHOERER_BAD_RELATION_MSG, organisationUuid, bruger.RelationListe.Tilhoerer.ReferenceID.Item));
            }

            if (bruger.RelationListe.TilknyttedeEnheder != null)
            {
                errors.Add(string.Format(TILHOERER_TILKNYTTEDE_ENHEDER_MSG));
            }

            if (bruger.RelationListe.TilknyttedeFunktioner != null)
            {
                errors.Add(string.Format(TILHOERER_TILKNYTTEDE_FUNKTIONER));
            }

            if (bruger.RelationListe.TilknyttedeInteressefaellesskaber != null)
            {
                errors.Add(string.Format(TILKNYTTEDE_INTERESSE_FAELLESSKABER_MSG));
            }

            if (bruger.RelationListe.TilknyttedePersoner != null)
            {
                if (bruger.RelationListe.TilknyttedePersoner.Length != 1)
                {
                    errors.Add(string.Format(TILKNYTTEDE_PERSONER_MSG));
                }
            }


            if (bruger.RelationListe.TilknyttedeOrganisationer != null)
            {
                errors.Add(string.Format(TILKNYTTEDE_ORGANISATIONER_MSG));
            }

            if (bruger.RelationListe.TilknyttedeItSystemer != null)
            {
                errors.Add(string.Format(TILKNYTTEDE_ITSYSTEMER_MSG));
            }

            if (bruger.RelationListe.Adresser != null)
            {
                foreach (var address in bruger.RelationListe.Adresser ?? Enumerable.Empty<global::IntegrationLayer.Bruger.AdresseFlerRelationType>())
                {
                    string type = address.Type?.Item;
                    string role = address.Rolle?.Item;

                    if (!ValidateUserAddressType(type))
                    {
                        errors.Add(string.Format(TYPE_MSG, address.ReferenceID.Item, type));
                    }
                    if (!ValidateRole(role))
                    {
                        errors.Add(string.Format(ROLE_MSG, address.ReferenceID.Item, type));

                    }
                }
            }
            return errors;
        }

        private bool ValidateUserAddressType(string type)
        {
            return type.Equals(UUIDConstants.USER_ADDRESS_TYPE);
        }

        private bool ValidateRole(string rolle)
        {
            return (rolle.Equals(UUIDConstants.ADDRESS_ROLE_CONTACT_ADDRESS) ||
                rolle.Equals(UUIDConstants.ADDRESS_ROLE_CONTACT_ADDRESS_OPEN_HOURS) ||
                rolle.Equals(UUIDConstants.ADDRESS_ROLE_EAN) ||
                rolle.Equals(UUIDConstants.ADDRESS_ROLE_EMAIL) ||
                rolle.Equals(UUIDConstants.ADDRESS_ROLE_EMAIL_REMARKS) ||
                rolle.Equals(UUIDConstants.ADDRESS_ROLE_LOCATION) ||
                rolle.Equals(UUIDConstants.ADDRESS_ROLE_LOSSHORTNAME) ||
                rolle.Equals(UUIDConstants.ADDRESS_ROLE_PHONE) ||
                rolle.Equals(UUIDConstants.ADDRESS_ROLE_PHONE_OPEN_HOURS) ||
                rolle.Equals(UUIDConstants.ADDRESS_ROLE_POST) ||
                rolle.Equals(UUIDConstants.ADDRESS_ROLE_POST_RETURN));
        }
    }
}
