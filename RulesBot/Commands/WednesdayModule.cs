using Discord.Commands;
using RulesBot.Core.Data;
using RulesBot.Core.Extensions;
using System;
using System.IO;
using System.Threading.Tasks;

namespace RulesBot.Commands
{
    public class WednesdayModule: ModuleBase<SocketCommandContext>
    {
        private readonly AppConfig Configuration;
        private static int Stubbornness = 0;
        private static DateTime LastStubbornness;
        public WednesdayModule(AppConfig config)
        {
            Configuration = config;
        }

        [Command("wednesday")]
        public Task ItIsWednesdayMyDudes()
        {
            if (DateTime.Today.DayOfWeek != DayOfWeek.Monday)
            {
                if ((DateTime.UtcNow - LastStubbornness).TotalMinutes < 1) Stubbornness++;
                else Stubbornness = 1; 
                
                LastStubbornness = DateTime.UtcNow;

                if(Stubbornness < 3) return Context.Channel.SendMessageAsync("No it isn't >:(");
                else
                {
                    Stubbornness = 0;
                    return Context.Channel.SendMessageAsync($"FINE >:(\n\n{Configuration.Frogs.Random()}");
                }
            }


            return Context.Channel.SendMessageAsync($"It is Wednesday, my dudes\n\n{Configuration.Frogs.Random()}");
        }
    }
}
