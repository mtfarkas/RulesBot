using System;

namespace RulesBot.Core.Extensions
{
    public static class ObjectExtensions
    {
        public static void MustNotBeNull(this object source, string name)
        {
            if (source == null) throw new NullReferenceException(name);
        }
    }
}
