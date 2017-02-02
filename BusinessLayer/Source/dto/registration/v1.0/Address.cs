using System;

namespace Organisation.BusinessLayer.DTO.V1_0
{
    [Serializable]
    public class Address
    {
        public string Uuid { get; set; }
        public string Value { get; set; }
        public string ShortKey { get; set; }

        public V1_1.Address ConvertToV1_1()
        {
            return new V1_1.Address()
            {
                Uuid = this.Uuid,
                Value = this.Value,
                ShortKey = this.ShortKey
            };
        }
    }
}
