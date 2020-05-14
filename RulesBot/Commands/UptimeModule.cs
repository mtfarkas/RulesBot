using Discord.Commands;
using RulesBot.Core.Extensions;
using System;
using System.Threading.Tasks;

namespace RulesBot.Commands
{
    public class UptimeModule: ModuleBase<SocketCommandContext>
    {
        [Command("uptime")]
        public Task Uptime()
            => Context.Channel.SendMessageAsync((DateTime.UtcNow - Program.Started).PrettyPrint());
    }
}
