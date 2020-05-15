using Discord.WebSocket;
using RulesBot.Core;
using RulesBot.Core.Data;
using RulesBot.Core.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RulesBot.MessageHandlers.Handlers
{
    public class CuteChatHandler : IMessageHandler
    {
        private readonly AppConfig Configuration;
        private static readonly string[] TriggerWords = new string[]
        {
            "cute", "cutie"
        };
        
        public CuteChatHandler()
        {
            Configuration = ConfigurationHost.Current;
        }

        public async Task<bool> Execute(SocketUserMessage message)
        {
            if (!TriggerWords.Any(item => message.Content.Contains(item, StringComparison.InvariantCultureIgnoreCase)))
                return false;

            string selectedPhrase = Configuration.CutePhrases.Random();

            if(selectedPhrase.Contains("{0}", StringComparison.InvariantCultureIgnoreCase))
            {
                var user = message.Author as SocketGuildUser;
                selectedPhrase = String.Format(selectedPhrase, (user?.Nickname ?? user?.Username) ?? message.Author.Username);
            }

            await message.Channel.SendMessageAsync(selectedPhrase);

            return true;
        }
    }
}
