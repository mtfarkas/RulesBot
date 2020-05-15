using Discord.WebSocket;
using RulesBot.Core;
using RulesBot.Core.Data;
using RulesBot.Core.Utils;
using System;
using System.Linq;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Services;
using TwitchLib.Api.Services.Events.LiveStreamMonitor;

namespace RulesBot
{
    public class StreamNotifier
    {
        private readonly DiscordSocketClient DiscordClient;
        private readonly LiveStreamMonitorService LiveStreamMonitor;
        private readonly TwitchAPI TwitchAPI;
        private readonly AppConfig Configuration;
        public StreamNotifier(DiscordSocketClient discordClient)
        {
            Configuration = ConfigurationHost.Current;

            DiscordClient = discordClient;

            TwitchAPI = new TwitchAPI();
            TwitchAPI.Settings.ClientId = EnvironmentUtils.VariableOrThrow(Constants.Environment.TwitchClientId);
            TwitchAPI.Settings.AccessToken = EnvironmentUtils.VariableOrThrow(Constants.Environment.TwitchBotToken);

            LiveStreamMonitor = new LiveStreamMonitorService(TwitchAPI, 20);
            LiveStreamMonitor.SetChannelsByName(Configuration.TwitchFriends.ToList());

            LiveStreamMonitor.OnStreamOnline += async (s, e) =>
            {
                await LiveStreamMonitor_OnStreamOnline(e);
            };

            ConfigurationHost.ConfigurationChanged += ConfigurationChanged;
        }

        private void ConfigurationChanged(object sender, YAUL.Data.GenericEventArgs<AppConfig> e)
        {
            if (e.Value.StreamNotifierSettings.Enabled) Start();
            else Stop();
        }

        public void Start()
        {
            if(Configuration.StreamNotifierSettings.Enabled && !LiveStreamMonitor.Enabled)
            {
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
