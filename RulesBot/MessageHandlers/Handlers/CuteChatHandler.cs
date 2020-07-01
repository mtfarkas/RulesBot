using Discord.WebSocket;
using RulesBot.Core.Repositories;
using RulesBot.Data.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YAUL.Extensions;

namespace RulesBot.MessageHandlers.Handlers
{
    public class CuteChatHandler : IMessageHandler
    {
        private static Random rnd = new Random();
        private static int CooldownCounter = 0;
        private static readonly string[] TriggerWords = new string[]
        {
            "cute", "cutie"
        };
        
        private readonly IPhraseRepository phraseRepository;
        public CuteChatHandler(IPhraseRepository phraseRepository)
        {
            this.phraseRepository = phraseRepository;
        }

        public async Task<bool> Execute(SocketUserMessage message)
        {
            if (!TriggerWords.Any(item => message.Content.Contains(item, StringComparison.InvariantCultureIgnoreCase)))
                return false;

            Interlocked.Increment(ref CooldownCounter);

            if (CooldownCounter < 15) return false;

            const int chance = 66;
            if (rnd.Next(100) > chance) return false;

            var phrases = (await phraseRepository.FindPhrasesAsync(PhraseType.Cute)).Select(item => item.Value);

            string selectedPhrase = phrases.Random();

            if(selectedPhrase.Contains("{0}", StringComparison.InvariantCultureIgnoreCase))
            {
                var user = message.Author as SocketGuildUser;
                selectedPhrase = String.Format(selectedPhrase, (user?.Nickname ?? user?.Username) ?? message.Author.Username);
            }

            Interlocked.Exchange(ref CooldownCounter, 0);
            await message.Channel.SendMessageAsync(selectedPhrase);

            return true;
        }
    }
}
