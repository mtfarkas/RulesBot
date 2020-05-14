using System;
using System.Collections.Generic;
using System.Linq;

namespace RulesBot.Core.Extensions
{
    public static class EnumerableExtensions
    {
        public static readonly Random rnd = new Random();

        public static T Random<T>(this IEnumerable<T> source)
            => source.ElementAt(rnd.Next(source.Count()));
    }
}
