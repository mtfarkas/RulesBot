using Discord.Commands;
using System.Threading.Tasks;

namespace RulesBot.Commands
{
    public class DeleteModule: ModuleBase<SocketCommandContext>
    {
        [Command("del")]
        public async Task Delete(ulong message)
        {
            var msg = await Context.Channel.GetMessageAsync(message);
            if(msg != null && msg.Author.Id == Context.Client.CurrentUser.Id) await msg.DeleteAsync();
        }
    }
}
