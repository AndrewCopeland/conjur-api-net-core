﻿using System;
namespace ConjurClient.Exceptions
{
    [Serializable()]
    public class InvalidConfigurationException : System.Exception
    {
        public InvalidConfigurationException() : base() { }
        public InvalidConfigurationException(string message) : base(message) { }
        public InvalidConfigurationException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client.
        protected InvalidConfigurationException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
