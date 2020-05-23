using Discord.WebSocket;
using RulesBot.Core;
using RulesBot.Core.Data;
using RulesBot.Core.Repositories;
using System.Linq;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Services;
using TwitchLib.Api.Services.Events.LiveStreamMonitor;
using YAUL.Utilities;

namespace RulesBot
{
    public class StreamNotifier
    {
        private readonly DiscordSocketClient DiscordClient;
        private readonly LiveStreamMonitorService LiveStreamMonitor;
        private readonly TwitchAPI TwitchAPI;
        private readonly AppConfig Configuration;
        private readonly ITwitchRepository twitchRepository;
        public StreamNotifier(DiscordSocketClient discordClient, ITwitchRepository twitchRepository)
        {
            this.twitchRepository = twitchRepository;
            Configuration = ConfigurationHost.Current;

            DiscordClient = discordClient;

            TwitchAPI = new TwitchAPI();
            TwitchAPI.Settings.ClientId = EnvUtils.VariableOrThrow(Constants.Environment.TwitchClientId);
            TwitchAPI.Settings.AccessToken = EnvUtils.VariableOrThrow(Constants.Environment.TwitchBotToken);

            LiveStreamMonitor = new LiveStreamMonitorService(TwitchAPI, 20);

            LiveStreamMonitor.OnStreamOnline += async (s, e) =>
            {
                await LiveStreamMonitor_OnStreamOnline(e);
            };

            TwitchRepository.TwitchFriendListChanged += async (s, e) =>
            {
                await TwitchRepository_TwitchFriendListChanged();
            };
        }

        private async Task TwitchRepository_TwitchFriendListChanged()
        {
            Log.Info("Twitch friend list changed, restarting live stream monitor");
            Stop();

            await Start();
            Log.Info("Live stream monitor restart done!");
        }

        public async Task Start()
        {
            if(Configuration.StreamNotifierSettings.Enabled && !LiveStreamMonitor.Enabled)
            {
                LiveStreamMonitor.ChannelsToMonitor?.Clear();
                var channels = await twitchRepository.FindAllTwitchFriendsAsync();
                LiveStreamMonitor.SetChannelsByName(channels.ToList());

                Log.Info("LiveStreamMonitor started");
                LiveStreamMonitor.Start();
            }
        }

        public void Stop()
        {
            if (LiveStreamMonitor.Enabled)
            {
                Log.Info("LiveStreamMonitor stopped");
                LiveStreamMonitor.Stop();
            }
        }

        private async Task LiveStreamMonitor_OnStreamOnline(OnStreamOnlineArgs e)
        {
            if (e.Stream.StartedAt.ToUniversalTime() < Program.Started) return;

            string msg = $"{e.Channel} just went live!\n=================================\n{e.Stream.Title}\nhttps://twitch.tv/{e.Channel}";

            var guild = DiscordClient.GetGuild(Configuration.StreamNotifierSettings.NotificationGuild);
            if(guild == null)
            {
                Log.Info($"Can't find guild {Configuration.StreamNotifierSettings.NotificationGuild}");
                return;
            }
            var channel = guild.GetTextChannel(Configuration.StreamNotifierSettings.NotificationChannel);
            if(channel == null)
            {
                Log.Info($"Can't find channel {Configuration.StreamNotifierSettings.NotificationChannel}");
                return;
            }

            await channel.SendMessageAsync(msg);
        }
    }
}
