using System;

namespace RulesBot.Core.Exceptions
{

    [Serializable]
    public class MissingEnvironmentVariableException : Exception
    {
        public MissingEnvironmentVariableException() { }
        public MissingEnvironmentVariableException(string varName) : base($"Missing environment variable: {varName}") { }
        protected MissingEnvironmentVariableException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
