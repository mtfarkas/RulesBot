using Discord.Commands;
using System.Threading.Tasks;

namespace RulesBot.Commands
{
    public class PingModule: ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public Task Pong()
            => Context.Channel.SendMessageAsync("pong!");
    }
}
