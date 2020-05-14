using Discord.Commands;
using RulesBot.Core;
using RulesBot.Core.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RulesBot.Commands
{
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        private AppConfig Configuration;
        public HelpModule()
        {
            Configuration = ConfigurationHost.Current;
        }

        [Command("help")]
        public Task GetHelp(string topic = null)
        {
            if (topic != null)
            {
                var result = Configuration.HelpTexts.FirstOrDefault(item => item.Topic.Equals(topic, StringComparison.InvariantCultureIgnoreCase)
                                                                                || $"{Configuration.BotSettings.CommandCharacter}{item.Topic}".Equals(topic, StringComparison.InvariantCultureIgnoreCase));

                if (result != null)
                    return Context.Channel.SendMessageAsync(result.HelpText);
                else
                    return Context.Channel.SendMessageAsync($"There's no help for {topic.ToLowerInvariant()}");
            }
            else
            {
                return Context.Channel.SendMessageAsync(Configuration.PlainHelpText);
            }
        }
    }
}
