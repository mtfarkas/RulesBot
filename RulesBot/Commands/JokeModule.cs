using Discord.Commands;
using Flurl.Http;
using RulesBot.Core.Data;
using System.Threading.Tasks;

namespace RulesBot.Commands
{
    public class JokeModule: ModuleBase<SocketCommandContext>
    {
        private const string JokeEndpoint = @"https://official-joke-api.appspot.com/random_joke";

        [Command("joke")]
        public async Task RandomJoke()
        {
            var joke = await JokeEndpoint.GetJsonAsync<JokeModel>();

            await Context.Channel.SendMessageAsync(joke.Complete);
        }
    }
}
