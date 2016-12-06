using Organisation.IntegrationLayer;
using System;
using System.Collections.Generic;

namespace OrganisationInspector
{     
    public class PersonValidator : Validator
    {
        private PersonStub personStub = new PersonStub();
        private string organisationUuid;

        private const string BRUGERVENDT_NOEGLE_NULL_MSG = "BrugervendtNoegle is mandatory";
        private const string NAVN_TEKST_MSG = "NavnTekst is mandatory";
       
        public PersonValidator(string uuid) {
            this.organisationUuid = uuid;
        }

        public List<string> Validate(string uuid)
        {
            /*
             *Person
             - Attribute fields that MUST be filled out
             - NavnTekst [X]
             */
            List<String> errors = new List<string>();
            IntegrationLayer.Person.RegistreringType1 person = personStub.GetLatestRegistration(uuid, false);

            if (string.IsNullOrEmpty(person.AttributListe?[0]?.NavnTekst))
            {
                errors.Add(NAVN_TEKST_MSG);
            }

            if (string.IsNullOrEmpty(person.AttributListe?[0]?.BrugervendtNoegleTekst)){
                errors.Add(BRUGERVENDT_NOEGLE_NULL_MSG);
            }
            return errors;
        }     
    }
}
