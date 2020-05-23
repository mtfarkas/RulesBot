using Microsoft.EntityFrameworkCore;
using RulesBot.Data;
using RulesBot.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RulesBot.Core.Repositories
{
    public interface ITwitchRepository
    {
        Task<bool> AddTwitchFriendAsync(string channel);
        Task<bool> RemoveTwitchFriendAsync(string channel);

        Task<IEnumerable<string>> FindAllTwitchFriendsAsync();
    }

    public class TwitchRepository : ITwitchRepository
    {
        private readonly RulesBotContext context;
        public TwitchRepository(RulesBotContext context)
        {
            this.context = context;
        }

        public static event EventHandler TwitchFriendListChanged;

        public async Task<bool> AddTwitchFriendAsync(string channel)
        {
            var newVal = new TwitchFriend
            {
                Channel = channel
            };

            await context.TwitchFriends.AddAsync(newVal);

            int result = 0;

            try { result = await context.SaveChangesAsync(); }
            catch { }

            bool success = result != 0;

            if (success) TwitchFriendListChanged?.Invoke(null, null);

            return success;
        }

        public async Task<IEnumerable<string>> FindAllTwitchFriendsAsync()
        {
            return await context.TwitchFriends.AsNoTracking()
                .Select(item => item.Channel)
                .ToListAsync();
        }

        public async Task<bool> RemoveTwitchFriendAsync(string channel)
        {
            string channelLower = channel.ToLower();

            var item = await context.TwitchFriends
                .AsQueryable()
                .FirstOrDefaultAsync(item => item.Channel == channelLower);

            if (item == null) return false;

            context.TwitchFriends.Remove(item);

            int result = await context.SaveChangesAsync();

            bool success = result != 0;

            if (success) TwitchFriendListChanged?.Invoke(null, null);

            return success;
        }
    }
}
