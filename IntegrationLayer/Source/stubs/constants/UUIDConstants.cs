namespace Organisation.IntegrationLayer
{
    internal static class UUIDConstants
    {
        // These UUIDs are copied from "Anvisninger til anvendelse af STS-Organisation v 1.0"

        // These are the UUIDs used to indicate a specific type of address
        public const string ORGUNIT_ADDRESS_TYPE = "9b33c0a0-a566-4ec0-8200-325cb1e5bb9ac";
        public const string USER_ADDRESS_TYPE = "71a08d28-3af7-4bb4-9964-bc2b76b93d64";
        public const string ITSYSTEM_ADDRESS_TYPE = "fe5092b8-aee7-488f-a881-6dd088227bf4";

        // The are the UUIDs used to indicate the role of the address
        public const string ADDRESS_ROLE_POST = "80b610c6-314b-485a-a014-a9a1d7070bc4";
        public const string ADDRESS_ROLE_EMAIL = "2b670843-ce42-411a-8fb5-311dfdd5caf0";
        public const string ADDRESS_ROLE_EAN = "9ccaafe4-c4b2-4d25-942a-2ec5730d4ed8";
        public const string ADDRESS_ROLE_PHONE = "8dcfa714-5ed3-4000-b551-2ba520e7d8ad";
        public const string ADDRESS_ROLE_LOSSHORTNAME = "47a33082-4687-4a68-b82f-5bf6f9d8ee13";
        public const string ADDRESS_ROLE_LOCATION = "c06cd146-ae9d-46e7-8c32-1f59a112e1b3";
        public const string ADDRESS_ROLE_PHONE_OPEN_HOURS = "3ffa11d1-cf69-4c1a-91d2-5791b1221833";
        public const string ADDRESS_ROLE_CONTACT_ADDRESS_OPEN_HOURS = "835f8356-53fa-489d-9ad9-0827a224db24";
        public const string ADDRESS_ROLE_URL = "a99c073d-482e-47d3-9275-13c79f453c3a";
        public const string ADDRESS_ROLE_CONTACT_ADDRESS = "e7145fbb-2205-48b6-9579-74699e1e1fa0";
        public const string ADDRESS_ROLE_POST_RETURN = "33b18ee9-428c-492b-bbc2-761ed92240ad";
        public const string ADDRESS_ROLE_EMAIL_REMARKS = "11c7d157-20c7-42e9-a4f1-34c213194bfa";

        // These are the UUIDs used to indicate the type of a function
        public const string ORGFUN_POSITION = "02e61900-33e0-407f-a2a7-22f70221f003";
        public const string ORGFUN_IT_USAGE = "82fcfc0f-88c4-4dc2-a30d-d091986a6112";
        public const string ORGFUN_PAYOUT_UNIT = "faf29ba2-da6d-49c4-8a2f-0739172f4227";
        public const string ORGFUN_CONTACT_UNIT = "7368482a-177e-4e04-8574-f558e6f1ef45";
    }
}
