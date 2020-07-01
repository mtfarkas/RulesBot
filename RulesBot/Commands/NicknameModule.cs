using Discord.Commands;
using Discord.WebSocket;
using RulesBot.Core;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RulesBot.Commands
{
    [Group("nick")]
    public class NicknameModule: RulesBotModule
    {
        private static readonly Regex userIdPattern = new Regex(@"<@!?(.+)>");

        [Command("add")]
        public async Task Add(string nickName, string mentioned = null)
        {
            try
            {
                SocketGuildUser user = GetUser(mentioned);
                if (user == null)
                {
                    var caller = Context.User as SocketGuildUser;
                    string name = caller?.Nickname ?? Context.User.Username;
                    await Context.Channel.SendMessageAsync($"I'm afraid I can't do that, {name}");
                }
                else await SetNickname(user, nickName);

                await Context.Channel.SendMessageAsync("Your wish is my command");
            }
            catch (Exception ex)
            {
                var caller = Context.User as SocketGuildUser;
                string name = caller?.Nickname ?? Context.User.Username;
                await Context.Channel.SendMessageAsync($"I'm afraid I can't do that, {name}");
                Log.Exception(ex); 
            }
        }

        [Command("clear")]
        public Task Clear(string mentioned = null)
            => Add(string.Empty, mentioned);

        private SocketGuildUser GetUser(string mention)
        {
            SocketGuildUser user = null;
            if (mention != null)
            {
                var match = userIdPattern.Match(mention);

                if (match.Success && ulong.TryParse(match.Groups[1].Value, out ulong userId))
                {
                    user = Context.Guild.GetUser(userId);
                }
            }
            else
            {
                user = Context.User as SocketGuildUser;
            }

            return user;
        }

        private Task SetNickname(SocketGuildUser user, string nick)
            => user?.ModifyAsync(props => props.Nickname = nick);
    }
}
