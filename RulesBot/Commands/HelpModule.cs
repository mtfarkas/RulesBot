using Discord;
using Discord.Commands;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RulesBot.Commands
{
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        private const string AllHelp =
@"DorxRulesBot

""Useful"" commands:
**!help** -- Displays this message
**!help <command>** -- Displays help text for commands
**!wiki <keyword>**  -- Performs a Wikipedia search
**!wednesday** -- It is Wednesday, my dudes
**!prime** -- Summons the Prime Chime";

        private static readonly Dictionary<string, string> HelpTopics;

        static HelpModule()
        {
            HelpTopics = new Dictionary<string, string>
            {
                {
                    "!help",
                    @"!help help:

I mean, it's pretty self explanatory"
                },

                {
                    "!wiki",
                    @"**!wiki** help:

Use the **!wiki** command to query Wikipedia for the 3 top results of the keywords. (Note: if you want to query using multiple keywords, put double quotes around them)
Example usage:
    **!wiki** snail => performs a search with the keyword ""snail"" and returns the 3 most popular results
    **!wiki** ""red apple"" => performs a search with the keywords ""red apple"" and returns the 3 most popular results"
                },

                {
                    "!wednesday",
                    @"**!wednesday ** help:

Gives you a random wednesday gif."
                },

                {
                    "!prime",
                    @"**!prime** help:

Summons the Prime Chime into the chat. "
                }
            };
        }

        [Command("help")]
        public Task GetHelp(string topic = null)
        {
            if (topic != null)
            {
                if (HelpTopics.TryGetValue(topic.ToLowerInvariant(), out string helpText))
                    return Context.Channel.SendMessageAsync(helpText);
                else
                    return Context.Channel.SendMessageAsync($"There's no help for {topic.ToLowerInvariant()}");
            }
            else
            {
                return Context.Channel.SendMessageAsync(AllHelp);
            }
        }
    }
}
