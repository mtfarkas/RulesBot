using Discord.WebSocket;
using RulesBot.Core;
using RulesBot.Core.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RulesBot.MessageHandlers.Handlers
{
    public class Rule2Handler : IMessageHandler
    {
        private readonly AppConfig Configuration;
        public Rule2Handler()
        {
            Configuration = ConfigurationHost.Current;
        }

        public async Task<bool> Execute(SocketUserMessage message)
        {
            if (!Configuration.Rule2Phrases.Any(item => message.Content.Contains(item, StringComparison.InvariantCultureIgnoreCase)))
                return false;

            await message.Channel.SendMessageAsync("2) No Dying");

            return true;
        }
    }
}
