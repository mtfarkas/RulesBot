using System;
using YAUL.Data;
using YAUL.Extensions;

namespace RulesBot.Core
{
    public static class Log
    {
        public static event EventHandler<GenericEventArgs<string>> LogEmitted;

        public static void Info(string message)
            => LogEmitted?.Invoke(null, new GenericEventArgs<string>(ConstructMessage(message, "INF")));
        public static void Debug(string message)
            => LogEmitted?.Invoke(null, new GenericEventArgs<string>(ConstructMessage(message, "DBG")));
        public static void Warning(string message)
            => LogEmitted?.Invoke(null, new GenericEventArgs<string>(ConstructMessage(message, "WRN")));
        public static void Error(string message)
            => LogEmitted?.Invoke(null, new GenericEventArgs<string>(ConstructMessage(message, "ERR")));
        public static void Exception(Exception ex)
            => LogEmitted?.Invoke(null, new GenericEventArgs<string>(ConstructMessage(ex)));

        private static string ConstructMessage(string message, string level)
            => $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.ff}] [{level.ToUpper()}] -- {message}";
        private static string ConstructMessage(Exception ex)
            => $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.ff}] [ERR] -- {ex.GetFullMessage()}";

    }
}
