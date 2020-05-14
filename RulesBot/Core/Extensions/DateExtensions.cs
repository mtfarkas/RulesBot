using System;
using System.Collections.Generic;
using System.Text;

namespace RulesBot.Core.Extensions
{
    public static class DateExtensions
    {
        public static string PrettyPrint(this TimeSpan source)
        {
            var sb = new StringBuilder();

            if (source.Days == 1) sb.AppendFormat("{0} day, ", source.Days);
            else sb.AppendFormat("{0} days, ", source.Days);

            if (source.Hours == 1) sb.AppendFormat("{0} hour, ", source.Hours);
            else sb.AppendFormat("{0} hours, ", source.Hours);

            if (source.Minutes == 1) sb.AppendFormat("{0} minute, ", source.Minutes);
            else sb.AppendFormat("{0} minutes, ", source.Minutes);

            if (source.Seconds == 1) sb.AppendFormat("{0} second", source.Seconds);
            else sb.AppendFormat("{0} seconds", source.Seconds);

            return sb.ToString();
        }
    }
}
