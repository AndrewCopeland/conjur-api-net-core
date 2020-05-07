using System;
namespace ConjurClient.Exceptions
{
    [Serializable()]
    public class ConjurResourceNotFoundException : System.Exception
    {
        public ConjurResourceNotFoundException() : base() { }
        public ConjurResourceNotFoundException(string message) : base(message) { }
        public ConjurResourceNotFoundException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client.
        protected ConjurResourceNotFoundException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}

