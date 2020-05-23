using Discord.WebSocket;
using RulesBot.Core;
using RulesBot.Core.Data;
using RulesBot.Core.Repositories;
using RulesBot.Data.Entities;
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

        private readonly IPhraseRepository phraseRepository;
        public StreetsQuoteHandler(IPhraseRepository phraseRepository)
        {
            this.phraseRepository = phraseRepository;
        }

        public async Task<bool> Execute(SocketUserMessage message)
        {
            if (!TriggerWords.Any(item => message.Content.Contains(item, StringComparison.InvariantCultureIgnoreCase))) return false;

            var quotes = (await phraseRepository.FindPhrasesAsync(PhraseType.Streets)).Select(item => item.Value).Shuffle();

            string selectedQuote = quotes.Random();

            await message.Channel.SendMessageAsync(selectedQuote);

            return true;
        }
    }
}
