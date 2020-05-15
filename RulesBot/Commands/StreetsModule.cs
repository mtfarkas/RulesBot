using Discord.Commands;
using System.Threading.Tasks;

namespace RulesBot.Commands
{
    public class StreetsModule: RulesBotCommand
    {
        private const string StreetsUrl = @"https://www.youtube.com/watch?v=oYmqJl4MoNI";
        [Command("streets")]
        public Task StreetsLink()
            => Context.Channel.SendMessageAsync($"DUDE WHAT A RUSH\n{StreetsUrl}");
    }
}
