using System;
using System.Collections.Generic;

namespace Organisation.BusinessLayer
{
    [Serializable]
    public class UserRegistration
    {
        // attributes for User object
        public string UserUuid { get; set; }
        public string UserShortKey { get; set; }
        public string UserId { get; set; }
        public Address Phone { get; set; } = new Address();
        public Address Email { get; set; } = new Address();
        public Address Location { get; set; } = new Address();

        // relevant information about the Users position in the municipality
        public string PositionOrgUnitUuid { get; set; }
        public string PositionUuid { get; set; }
        public string PositionName { get; set; }
        public string PositionShortKey { get; set; }

        // attributes for Person object
        public string PersonUuid { get; set; }
        public string PersonShortKey { get; set; }
        public string PersonName { get; set; }
        public string PersonCpr { get; set; }

        // registration timestamp
        public DateTime Timestamp { get; set; } = DateTime.Now.AddMinutes(-5);
    }
}
