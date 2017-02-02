using System;
using System.Collections.Generic;

namespace Organisation.BusinessLayer.DTO.V1_1
{
    [Serializable]
    public class UserRegistration
    {
        // attributes for User object
        public string Uuid { get; set; }
        public string ShortKey { get; set; }
        public string UserId { get; set; }

        public Address Phone { get; set; } = new Address();
        public Address Email { get; set; } = new Address();
        public Address Location { get; set; } = new Address();

        public List<Position> Positions { get; set; } = new List<Position>();

        public Person Person { get; set; } = new Person();

        // registration timestamp
        public DateTime Timestamp { get; set; } = DateTime.Now.AddMinutes(-5);
    }
}
