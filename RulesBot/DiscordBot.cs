using Discord;
using Discord.Commands;
using Discord.WebSocket;
using RulesBot.Core;
using RulesBot.Core.Data;
using RulesBot.Core.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Services;
using TwitchLib.Api.Services.Events.LiveStreamMonitor;

namespace RulesBot
{
    public class DiscordBot : IDisposable
    {
        #region Private fields
        private DiscordSocketClient Client;
        private CommandService CommandService;
        private AppConfig Configuration;
        private LiveStreamMonitorService LiveStreamMonitor;
        private TwitchAPI TwitchAPI;

        private static readonly Random rnd = new Random();
        #endregion

        #region Ctor
        public DiscordBot(AppConfig appConfig)
        {
            Configuration = appConfig;

            Client = new DiscordSocketClient(new DiscordSocketConfig
            {
                ConnectionTimeout = 15000,
                DefaultRetryMode = RetryMode.AlwaysRetry,
                GuildSubscriptions = false,
                LogLevel = LogSeverity.Info
            });

            CommandService = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = false,
                DefaultRunMode = RunMode.Async,
                IgnoreExtraArgs = true,
                LogLevel = LogSeverity.Info,
                SeparatorChar = ' '
            });


            Client.Log += DiscordClientLog;
            Client.MessageReceived += HandleDiscordMessage;
        }
        #endregion

        #region Public methods
        public async Task Start()
        {
            await Client.LoginAsync(TokenType.Bot, Configuration.BotToken);

            await CommandService.AddModulesAsync(Assembly.GetEntryAssembly(), DIHost.ServiceProvider);

            await ConfigureStreamMonitor();

            await Client.StartAsync();
        }
        #endregion

        private Task ConfigureStreamMonitor()
        {
            TwitchAPI = new TwitchAPI();
            TwitchAPI.Settings.ClientId = Environment.GetEnvironmentVariable("TWITCH_CLIENT");
            TwitchAPI.Settings.AccessToken = Environment.GetEnvironmentVariable("TWITCH_TOKEN");

            LiveStreamMonitor = new LiveStreamMonitorService(TwitchAPI, 60);
            LiveStreamMonitor.SetChannelsByName(Configuration.TwitchFriends.ToList());

            LiveStreamMonitor.OnStreamOnline += async (s, e) =>
            {
                await LiveStreamMonitor_OnStreamOnline(e);
            };

            if (!Configuration.IsDebug)
            {
                LiveStreamMonitor.Start();
            }

            return Task.CompletedTask;
        }

        private async Task LiveStreamMonitor_OnStreamOnline(OnStreamOnlineArgs e)
        {
            if (e.Stream.StartedAt.ToUniversalTime() < Configuration.CurrentRun) return;

            string msg = $"{e.Channel} just went live!\n\n{e.Stream.Title}\nhttps://twitch.tv/{e.Channel}";

            ulong serverId = UInt64.Parse(Environment.GetEnvironmentVariable("DISCORD_SERVER"));
            ulong channelId = UInt64.Parse(Environment.GetEnvironmentVariable("STREAM_CHANNEL"));

            var dorxServer = Client.GetGuild(serverId);

            if (dorxServer == null)
            {
                Log.Warning($"Can't find server {serverId}");
                return;
            }

            var liveStreamChannel = dorxServer.GetTextChannel(channelId);

            if (liveStreamChannel == null)
            {
                Log.Warning($"Can't find channel {channelId}");
                return;
            }

            await liveStreamChannel.SendMessageAsync(msg);
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Client.Dispose();
                    LiveStreamMonitor.Stop();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

        #region Event handlers
        private Task DiscordClientLog(LogMessage arg)
        {
            switch (arg.Severity)
            {
                case LogSeverity.Info:
                    Log.Info(arg.Message);
                    break;
                case LogSeverity.Warning:
                    Log.Warning(arg.Message);
                    break;
                case LogSeverity.Debug:
                    Log.Debug(arg.Message);
                    break;
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    if (arg.Exception != null) Log.Exception(arg.Exception);
                    else Log.Error(arg.Message);
                    break;
            }

            return Task.CompletedTask;
        }

        private async Task HandleDiscordMessage(SocketMessage socketMessage)
        {
            if (!(socketMessage is SocketUserMessage msg)) return;
            if (msg.Author.IsBot) return;

            int argPos = 0;
            char commandChar = Configuration.CommandCharacter[0];

            if (Configuration.ApplyChannelFilter && !Configuration.EnabledChannels.Contains(msg.Channel.Id)) return;

            if (msg.HasCharPrefix(commandChar, ref argPos))
            {
                Log.Info($"Message {msg.Content} was a command, attempting to execute");
                var context = new SocketCommandContext(Client, msg);

                var result = await CommandService.ExecuteAsync(context, argPos, DIHost.ServiceProvider);

                if (!result.IsSuccess)
                {
                    switch (result.Error)
                    {
                        case CommandError.UnknownCommand:
                            Log.Info($"Unknown command {msg.Content}");
                            break;
                        default:
                            Log.Error(result.ErrorReason);
                            break;
                    }
                }
                else Log.Info($"Command {msg.Content} executed successfully");
            }
            else
            {
                await HandleNonCommandMessages(msg);
            }
        }

        private async Task HandleNonCommandMessages(SocketUserMessage msg)
        {
            var r = msg.AsRailway();

            await ExecuteRule2(r);
            await Cuters(r);
            await ExecuteVerbMeUpInside(r);
        }

        private async Task ExecuteRule2(Railway<SocketUserMessage> r)
        {
            if (r.Handled) return;
            var msg = r.Value;

            if (Configuration.Rule2Phrases.Any(item => msg.Content.Contains(item, StringComparison.InvariantCultureIgnoreCase)))
            {
                await msg.Channel.SendMessageAsync("2) No Dying");
                r.Handled = true;
                Log.Info($"Message {msg.Content} was handled by {nameof(ExecuteRule2)}()");
            }
        }

        private async Task ExecuteVerbMeUpInside(Railway<SocketUserMessage> r)
        {
            if (r.Handled) return;
            var msg = r.Value;

            const int percentChance = 1;
            string foundVerb = null;

            foreach (var verb in Configuration.Verbs)
            {
                if (msg.Content.Contains(verb, StringComparison.InvariantCultureIgnoreCase))
                {
                    foundVerb = verb;
                    break;
                }
            }

            if (Configuration.IsDebug || rnd.Next(100) < percentChance)
            {
                await msg.Channel.SendMessageAsync($"{foundVerb.FirstCharToUpper()} me up inside");
                r.Handled = true;
                Log.Info($"Message {msg.Content} was handled by {nameof(ExecuteVerbMeUpInside)}()");
            }

        }
        private async Task Cuters(Railway<SocketUserMessage> r)
        {
            if (r.Handled) return;
            var msg = r.Value;
            if (!msg.Content.Contains("cute")) return;

            string selectedPhrase = Configuration.CutePhrases.Random();

            if (selectedPhrase.Contains("{0}", StringComparison.InvariantCultureIgnoreCase))
            {
                var user = msg.Author as SocketGuildUser;
                selectedPhrase = String.Format(selectedPhrase, user.Nickname ?? user.Username);
            }

            await msg.Channel.SendMessageAsync(selectedPhrase);
            r.Handled = true;
            Log.Info($"Message {msg.Content} was handled by {nameof(Cuters)}()");
        }
        #endregion
    }
}
