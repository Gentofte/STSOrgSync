using System;

namespace SDB
{
        [Serializable]
        public class SDBException : System.Exception
        {
            public SDBException() : base() { }
            public SDBException(string message) : base(message) { }
            public SDBException(string message, System.Exception inner) : base(message, inner) { }

            // A constructor is needed for serialization when an
            // exception propagates from a remoting server to the client. 
            protected SDBException(System.Runtime.Serialization.SerializationInfo info,
                System.Runtime.Serialization.StreamingContext context)
                : base(info, context) { }
        }
}
