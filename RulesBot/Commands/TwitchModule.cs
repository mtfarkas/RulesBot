using Discord.Commands;
using RulesBot.Core;
using RulesBot.Core.Repositories;
using System.Threading.Tasks;

namespace RulesBot.Commands
{
    [Group("twitch")]
    public class TwitchModule : RulesBotModule
    {
        private readonly ITwitchRepository twitchRepository;

        public TwitchModule(ITwitchRepository twitchRepository)
        {
            this.twitchRepository = twitchRepository;
        }

        [Command("add")]
        public async Task Add(string channel)
        {
            var result = await twitchRepository.AddTwitchFriendAsync(channel);

            if (result) await Context.Channel.SendMessageAsync($"Channel {channel} successfully added to the livestream monitor");
            else await Context.Channel.SendMessageAsync($"Error adding channel or it's already added");
        }

        [Command("remove")]
        public async Task Remove(string channel)
        {
            var result = await twitchRepository.RemoveTwitchFriendAsync(channel);

            if (result) await Context.Channel.SendMessageAsync($"Channel {channel} successfully removed from the livestream monitor");
            else await Context.Channel.SendMessageAsync($"Error removing channel or it's not present");
        }
    }
}
