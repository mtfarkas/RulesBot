using Discord.WebSocket;
using RulesBot.Core;
using RulesBot.Core.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using YAUL.Extensions;

namespace RulesBot.MessageHandlers.Handlers
{
    public class StreetsQuoteHandler : IMessageHandler
    {
        private static readonly string[] TriggerWords = new string[]
        {
            "1:12",
            "1 12",
        };

        private AppConfig Configuration;
        public StreetsQuoteHandler()
        {
            Configuration = ConfigurationHost.Current;
        }

        public async Task<bool> Execute(SocketUserMessage message)
        {
            if (!TriggerWords.Any(item => message.Content.Contains(item, StringComparison.InvariantCultureIgnoreCase))) return false;

            string selectedQuote = Configuration.StreetsQuotes.Random();

            await message.Channel.SendMessageAsync(selectedQuote);

            return true;
        }
    }
}
