using System;

namespace Organisation.BusinessLayer.DTO.V1_1
{
    [Serializable]
    public class Address
    {
        public string Uuid { get; set; }
        public string Value { get; set; }
        public string ShortKey { get; set; }
    }
}
