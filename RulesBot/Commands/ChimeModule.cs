using Discord.Commands;
using RulesBot.Core.Utils;
using System.Threading.Tasks;

namespace RulesBot.Commands
{
    public class ChimeModule: ModuleBase<SocketCommandContext>
    {
        [Command("prime")]
        public Task PrimeChime()
            => Context.Channel.SendFileAsync(FileUtils.MakeAbsolute("Assets", "prime_chime.png"), "Did someone say [Prime Chime]?");
    }
}
