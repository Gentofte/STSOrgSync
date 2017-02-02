using System;
using System.Runtime.Serialization;

namespace Organisation.BusinessLayer
{
    [Serializable]
    public class UserReference
    {
        public string Uuid { get; set; }
    }
}
