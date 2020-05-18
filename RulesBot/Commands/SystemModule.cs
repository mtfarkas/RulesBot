using Discord.Commands;
using RulesBot.Core;
using System;
using System.Threading.Tasks;
using YAUL.Extensions;

namespace RulesBot.Commands
{
    public class SystemModule: RulesBotModule
    {
        [Command("del")]
        public async Task Delete(ulong message)
        {
            var msg = await Context.Channel.GetMessageAsync(message);
            if (msg != null && msg.Author.Id == Context.Client.CurrentUser.Id) await msg.DeleteAsync();
        }

        [Command("ping")]
        public Task Pong()
            => Context.Channel.SendMessageAsync("pong!");

        [Command("uptime")]
        public Task Uptime()
            => Context.Channel.SendMessageAsync((DateTime.UtcNow - Program.Started).PrettyPrint());
    }
}
