namespace Organisation.IntegrationLayer
{
    internal static class UUIDConstants
    {
        // These UUIDs are copied from "Anvisninger til anvendelse af STS-Organisation v 1.1"

        // These are the UUIDs used to indicate a specific type of address
        /* these are the old ones, they are not used anymore
        public const string ADDRESS_TYPE_ADDRESS = "68719027-5061-49da-9b31-53b348408f68";
        public const string ADDRESS_TYPE_DAR = ""; // unknown, not included in version 1.1 of the specs
        */
        public const string ADDRESS_TYPE_ORGUNIT = "9b33c0a0-a566-4ec0-8200-325cb1e5bb9a";
        public const string ADDRESS_TYPE_ORGFUNCTION = "1a8374f6-0ee8-4201-b27c-0e84d57db0ba";
        public const string ADDRESS_TYPE_USER = "71a08d28-3af7-4bb4-9964-bc2b76b93d64";

        // The are the UUIDs used to indicate the role of the address
        /* these are the old ones, they are not used anymore
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
        */
        public const string ADDRESS_ROLE_ORGUNIT_URL = "a99c073d-482e-47d3-9275-13c79f453c3a";
        public const string ADDRESS_ROLE_ORGUNIT_FOA = "0dac99a2-c419-4113-bb1d-757e00669bf2"; // TODO: what is this?
        public const string ADDRESS_ROLE_ORGUNIT_PHONE = "8dcfa714-5ed3-4000-b551-2ba520e7d8ad";
        public const string ADDRESS_ROLE_ORGUNIT_EAN = "9ccaafe4-c4b2-4d25-942a-2ec5730d4ed8";
        public const string ADDRESS_ROLE_ORGUNIT_POST = "80b610c6-314b-485a-a014-a9a1d7070bc4";
        public const string ADDRESS_ROLE_ORGUNIT_EMAIL = "2b670843-ce42-411a-8fb5-311dfdd5caf0";
        public const string ADDRESS_ROLE_ORGUNIT_LOSSHORTNAME = "47a33082-4687-4a68-b82f-5bf6f9d8ee13";

        public const string ADDRESS_ROLE_USER_EMAIL = "5d13e891-162a-456b-abf2-fd9b864df96d";
        public const string ADDRESS_ROLE_USER_PHONE = "5ef6be2d-59f4-4652-a680-585929924ba9";

        public const string ADDRESS_ROLE_ORGFUNCTION_URL = "560cb83d-386d-43c0-aaa2-986a915b087c";

        // These are the UUIDs used to indicate the type of a function
        public const string ORGFUN_POSITION = "02e61900-33e0-407f-a2a7-22f70221f003";
        public const string ORGFUN_IT_USAGE = "82fcfc0f-88c4-4dc2-a30d-d091986a6112";
        public const string ORGFUN_PAYOUT_UNIT = "faf29ba2-da6d-49c4-8a2f-0739172f4227";
        public const string ORGFUN_CONTACT_UNIT = "7368482a-177e-4e04-8574-f558e6f1ef45";
    }
}
