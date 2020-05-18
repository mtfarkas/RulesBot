using Discord.Commands;
using Flurl.Http;
using RulesBot.Core;
using RulesBot.Core.Data;
using System;
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
        private readonly AppConfig Configuration;

        public RandomModule()
        {
            Configuration = ConfigurationHost.Current;
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
        public Task ItIsWednesdayMyDudes()
        {
            if (DateTime.Today.DayOfWeek != DayOfWeek.Wednesday)
            {
                if ((DateTime.UtcNow - LastStubbornness).TotalMinutes < 1) Stubbornness++;
                else Stubbornness = 1;

                LastStubbornness = DateTime.UtcNow;

                if (Stubbornness < 3) return Context.Channel.SendMessageAsync("No it isn't >:(");
                else
                {
                    Stubbornness = 0;
                    return Context.Channel.SendMessageAsync($"FINE >:(\n\n{Configuration.WednesdayFrogs.Random()}");
                }
            }


            return Context.Channel.SendMessageAsync($"It is Wednesday, my dudes\n\n{Configuration.WednesdayFrogs.Random()}");
        }
    }
}
