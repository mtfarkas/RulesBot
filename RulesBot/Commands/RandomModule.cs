using Discord.Commands;
using Flurl.Http;
using RulesBot.Core;
using RulesBot.Core.Data;
using RulesBot.Core.Repositories;
using RulesBot.Data.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using YAUL.Extensions;

namespace RulesBot.Commands
{
    public class RandomModule: RulesBotModule
    {
        private const string JokeEndpoint = @"https://official-joke-api.appspot.com/random_joke";
        private const string StreetsUrl = @"https://www.youtube.com/watch?v=oYmqJl4MoNI";
        private static int Stubbornness = 0;
        private static DateTime LastStubbornness;
        private readonly IPhraseRepository phraseRepository;

        public RandomModule(IPhraseRepository phraseRepository)
        {
            this.phraseRepository = phraseRepository;
        }


        [Command("joke")]
        public async Task RandomJoke()
        {
            var joke = await JokeEndpoint.GetJsonAsync<JokeModel>();

            await Context.Channel.SendMessageAsync(joke.Complete);
        }

        [Command("streets")]
        public Task StreetsLink()
            => Context.Channel.SendMessageAsync($"DUDE WHAT A RUSH\n{StreetsUrl}");

        [Command("wednesday")]
        public async Task ItIsWednesdayMyDudes()
        {
            var wednesday = (await phraseRepository.FindPhrasesAsync(PhraseType.Wednesday)).Select(item => item.Value).Shuffle();
            if (DateTime.Today.DayOfWeek != DayOfWeek.Wednesday)
            {
                if ((DateTime.UtcNow - LastStubbornness).TotalMinutes < 1) Stubbornness++;
                else Stubbornness = 1;

                LastStubbornness = DateTime.UtcNow;

                if (Stubbornness < 3) await Context.Channel.SendMessageAsync("No it isn't >:(");
                else
                {
                    Stubbornness = 0;
                    await Context.Channel.SendMessageAsync($"FINE >:(\n\n{wednesday.Random()}");
                }

                return;
            }


            await Context.Channel.SendMessageAsync($"It is Wednesday, my dudes\n\n{wednesday.Random()}");
        }
    }
}
