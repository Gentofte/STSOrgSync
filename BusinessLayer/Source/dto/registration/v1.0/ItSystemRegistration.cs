﻿using System;
using System.Collections.Generic;

namespace Organisation.BusinessLayer.DTO.V1_0
{
    [Obsolete]
    [Serializable]
    public class ItSystemRegistration
    {
        // attributes for ItSystem object
        public string Uuid { get; set; }
        public string SystemShortKey { get; set; }
        public string JumpUrl { get; set; }

        // registration timestamp
        public DateTime Timestamp { get; set; } = DateTime.Now.AddMinutes(-5);
    }
}
