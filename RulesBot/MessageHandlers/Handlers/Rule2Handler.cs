using Discord.WebSocket;
using RulesBot.Core.Repositories;
using RulesBot.Data.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RulesBot.MessageHandlers.Handlers
{
    public class Rule2Handler : IMessageHandler
    {
        private readonly IPhraseRepository phraseRepository;
        public Rule2Handler(IPhraseRepository phraseRepository)
        {
            this.phraseRepository = phraseRepository;
        }

        public async Task<bool> Execute(SocketUserMessage message)
        {
            var phrases = await phraseRepository.FindPhrasesAsync(PhraseType.Rule2);
            if (!phrases.Any(item => message.Content.Contains(item.Value, StringComparison.InvariantCultureIgnoreCase)))
                return false;
            await message.Channel.SendMessageAsync("2) No Dying");

            return true;
        }
    }
}
