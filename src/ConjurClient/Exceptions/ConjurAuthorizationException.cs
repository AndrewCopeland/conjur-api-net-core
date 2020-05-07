using System;
namespace ConjurClient.Exceptions
{
    [Serializable()]
    public class ConjurAuthorizationException : System.Exception
    {
        public ConjurAuthorizationException() : base() { }
        public ConjurAuthorizationException(string message) : base(message) { }
        public ConjurAuthorizationException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client.
        protected ConjurAuthorizationException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}

