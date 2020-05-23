using Discord;
using Discord.Commands;
using Discord.WebSocket;
using RulesBot.Core;
using RulesBot.Core.Data;
using RulesBot.Core.Extensions;
using RulesBot.Data;
using RulesBot.MessageHandlers;
using RulesBot.MessageHandlers.Handlers;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using YAUL.Utilities;

namespace RulesBot
{
    public class DiscordBot : IDisposable
    {
        #region Private fields
        private DiscordSocketClient Client;
        private CommandService CommandService;
        private MessageExecutor MessageExecutor;
        private StreamNotifier StreamNotifier;
        private readonly AppConfig Configuration;

        //private RulesBotContext Context;
        #endregion

        #region Ctor
        public DiscordBot(/*RulesBotContext context*/)
        {
            //Context = context;

            Configuration = ConfigurationHost.Current;

            MessageExecutor = new MessageExecutor();
            MessageExecutor.RegisterHandler(new Rule2Handler());
            MessageExecutor.RegisterHandler(new CuteChatHandler());
            MessageExecutor.RegisterHandler(new StreetsQuoteHandler());

            MessageExecutor.RegisterHandler(new RandomVerbHandler());

            Client = new DiscordSocketClient(new DiscordSocketConfig
            {
                ConnectionTimeout = 15000,
                DefaultRetryMode = RetryMode.AlwaysRetry,
                GuildSubscriptions = false,
                LogLevel = LogSeverity.Info,
            }); ;

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

            if(!Program.IsDebug) StreamNotifier = new StreamNotifier(Client);
        }
        #endregion

        #region Public methods
        public async Task Start()
        {
            Log.Info("Registering modules...");
            await CommandService.AddModulesAsync(Assembly.GetEntryAssembly(), DIHost.ServiceProvider);
            Log.Info("Registering modules done");

            await Client.LoginAsync(TokenType.Bot, EnvUtils.VariableOrThrow(Constants.Environment.DiscordBotToken));

            await Client.StartAsync();

            StreamNotifier?.Start();
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Client.Dispose();
                    StreamNotifier?.Stop();
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

            if (Configuration.BotSettings.EnableChannelBlacklist
                && Configuration.BotSettings.BlacklistedChannels.Contains(msg.Channel.Id)) return;

            //if (msg.HasCharPrefix(Configuration.BotSettings.CommandCharacter, ref argPos) )
            if (msg.ContainsCommand(Configuration.BotSettings.CommandCharacter, ref argPos) )
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
            => await MessageExecutor.Execute(msg);
        #endregion
    }
}
