using System;

namespace Organisation.BusinessLayer.DTO.V1_1
{
    [Serializable]
    public class Position
    {
        public string Uuid { get; set; }
        public string ShortKey { get; set; }
        public string Name { get; set; }
        public string OrgUnitUuid { get; set; }
    }
}
