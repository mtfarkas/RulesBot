using RulesBot.Core.Exceptions;
using System;

namespace RulesBot.Core.Utils
{
    public static class EnvironmentUtils
    {
        public static string VariableOrThrow(string key)
            => Environment.GetEnvironmentVariable(key) ?? throw new MissingEnvironmentVariableException(key);
    }
}
