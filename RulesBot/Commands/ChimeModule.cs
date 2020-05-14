using Discord.Commands;
using System.IO;
using System.Threading.Tasks;

namespace RulesBot.Commands
{
    public class ChimeModule: ModuleBase<SocketCommandContext>
    {
        [Command("prime")]
        public Task PrimeChime()
            => Context.Channel.SendFileAsync(Path.Combine("Assets", "prime_chime.png"), "Did someone say [Prime Chime]?");
    }
}
