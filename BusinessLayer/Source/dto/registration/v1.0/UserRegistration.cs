using System;
using System.Collections.Generic;

namespace Organisation.BusinessLayer.DTO.V1_0
{
    [Obsolete]
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

        public V1_1.UserRegistration ConvertToV1_1()
        {
            V1_1.UserRegistration user = new V1_1.UserRegistration() {
                Email = this.Email.ConvertToV1_1(),
                Location = this.Location.ConvertToV1_1(),
                Phone = this.Phone.ConvertToV1_1(),
                ShortKey = this.UserShortKey,
                Timestamp = this.Timestamp,
                UserId = this.UserId,
                Uuid = this.UserUuid
            };

            user.Person = new V1_1.Person()
            {
                Cpr = this.PersonCpr,
                Name = this.PersonName,
                ShortKey = this.PersonShortKey,
                Uuid = this.PersonUuid
            };

            V1_1.Position position = new V1_1.Position()
            {
                Name = this.PositionName,
                OrgUnitUuid = this.PositionOrgUnitUuid,
                ShortKey = this.PositionShortKey,
                Uuid = this.PositionUuid
            };
            user.Positions.Add(position);

            return user;
        }
    }
}
