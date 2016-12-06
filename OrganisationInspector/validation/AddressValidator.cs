using Organisation.IntegrationLayer;
using System;
using System.Collections.Generic;

namespace OrganisationInspector
{
    /// <summary>
    /// Validate User (Address) according to Kombit defined rules
    /// </summary>
    /// 

    /* Adresse
        - Attribute fields that MUST be filled out
        - AdresseTekst [X]
    */
public class AddressValidator : Validator
{
    private const string ADRESSE_TEKST_MSG = "Adressetekst is mandatory";

    private AdresseStub adresseStub = new AdresseStub();
    private string organisationUuid;

    public AddressValidator(string uuid) {
        this.organisationUuid = uuid;
    }

    public List<string> Validate(string uuid)
    {
    List<String> errors = new List<string>();
            IntegrationLayer.Adresse.RegistreringType1 adresse = adresseStub.GetLatestRegistration(uuid, false);

            if (string.IsNullOrEmpty(adresse.AttributListe?[0]?.AdresseTekst))
            {
                errors.Add(ADRESSE_TEKST_MSG);
            }
            return errors;
        }     
    }
}
