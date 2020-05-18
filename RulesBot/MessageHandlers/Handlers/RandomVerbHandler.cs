using Discord.WebSocket;
using RulesBot.Core;
using RulesBot.Core.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using YAUL.Extensions;

namespace RulesBot.MessageHandlers.Handlers
{
    public class RandomVerbHandler: IMessageHandler
    {
        private readonly AppConfig Configuration;
        private static readonly Random rnd = new Random();
        public RandomVerbHandler()
        {
            Configuration = ConfigurationHost.Current;
        }

        public async Task<bool> Execute(SocketUserMessage message)
        {
            var messageWords = message.Content.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            string foundVerb = null;
            const int chance = 1;

            foreach(var word in messageWords)
            {
                foundVerb = Configuration.Verbs.FirstOrDefault(item => item.Equals(word, StringComparison.InvariantCultureIgnoreCase));

                if (foundVerb != null) break;
            }

            if(foundVerb != null && (Program.IsDebug || rnd.Next(100) < chance))
            {
                await message.Channel.SendMessageAsync($"{foundVerb.FirstCharToUpper()} me up inside");
                return true;
            }

            return false;
        }
    }
}
