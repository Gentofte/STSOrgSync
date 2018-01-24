using System;
using System.Runtime.Serialization;

namespace Organisation.BusinessLayer
{
    [Serializable]
    public class Person
    {
        public string Uuid { get; set; }
        public string ShortKey { get; set; }
        public string Name { get; set; }
        public string Cpr { get; set; }
    }
}
