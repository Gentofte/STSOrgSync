using System;

namespace SDB
{
        [Serializable]
        public class IncompleteObjectException : System.Exception
        {
            public IncompleteObjectException() : base() { }
            public IncompleteObjectException(string message) : base(message) { }
            public IncompleteObjectException(string message, System.Exception inner) : base(message, inner) { }

            // A constructor is needed for serialization when an
            // exception propagates from a remoting server to the client. 
            protected IncompleteObjectException(System.Runtime.Serialization.SerializationInfo info,
                System.Runtime.Serialization.StreamingContext context)
                : base(info, context) { }
        }
}
