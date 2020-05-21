using Discord.Commands;
using Discord.WebSocket;
using System.Text.RegularExpressions;
using YAUL.Extensions;

namespace RulesBot.Core.Extensions
{
    public static class SocketUserMessageExtensions
    {
        public static bool ContainsCommand(this SocketUserMessage source, char prefix, ref int argPos)
        {
            if (source.HasCharPrefix(prefix, ref argPos)) return true;

            char usedPrefix = ConfigurationHost.Current.BotSettings.CommandCharacter;
            var commandRegex = new Regex($@"({usedPrefix}\w+)");

            var match = commandRegex.Match(source.Content);
            if (match.Success && match.Groups[0]?.Value.IsMeaningful() == true)
            {
                string text = match.Groups[0].Value;
                argPos = source.Content.IndexOf(text) + 1;
                return true;
            }

            return false;
        }
    }
}
