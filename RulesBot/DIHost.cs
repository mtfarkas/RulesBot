using Microsoft.Extensions.DependencyInjection;
using RulesBot.Core.Data;
using RulesBot.Core.Exceptions;
using RulesBot.Core.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace RulesBot
{
    public static class DIHost
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public static void Setup()
        {
            var services = new ServiceCollection();

            SetupConfiguration(services);

            ConfigureServices(services);

            ServiceProvider = services.BuildServiceProvider();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<DiscordBot>();
        }

        private static void SetupConfiguration(IServiceCollection services)
        {
            var config = new AppConfig()
            {
                BotToken = Environment.GetEnvironmentVariable("BOT_TOKEN") ?? throw new MissingEnvironmentVariableException("BOT_TOKEN"),
                CurrentRun = DateTime.UtcNow
            };

            var configJson = JsonDocument.Parse(File.ReadAllText(Path.Combine("Assets", "config", "config.json")));

            config.ApplyChannelFilter = configJson.RootElement.GetProperty("ApplyChannelFilter").GetBoolean();

            if(config.ApplyChannelFilter) config.EnabledChannels = configJson.RootElement.GetProperty("EnabledChannels").EnumerateArray().Select(item => item.GetUInt64());

            config.CommandCharacter = configJson.RootElement.GetProperty("CommandCharacter").GetString();

            config.Verbs = File.ReadAllText(Path.Combine("Assets", "config", "verbs.txt")).Replace("\r\n", "\n").Split("\n", StringSplitOptions.RemoveEmptyEntries).Select(item => item.ToLower());
            config.Frogs = File.ReadAllText(Path.Combine("Assets", "config", "wednesday.txt")).Replace("\r\n", "\n").Split("\n", StringSplitOptions.RemoveEmptyEntries);
            config.Rule2Phrases = File.ReadAllText(Path.Combine("Assets", "config", "rule2.txt")).Replace("\r\n", "\n").Split("\n", StringSplitOptions.RemoveEmptyEntries);
            config.TwitchFriends = File.ReadAllText(Path.Combine("Assets", "config", "twitch_friends.txt")).Replace("\r\n", "\n").Split("\n", StringSplitOptions.RemoveEmptyEntries);
            config.CutePhrases = File.ReadAllText(Path.Combine("Assets", "config", "bot_cuties.txt")).Replace("\r\n", "\n").Split("\n", StringSplitOptions.RemoveEmptyEntries);

#if DEBUG
            config.IsDebug = true;
#else
            config.IsDebug = false;
#endif

            services.AddSingleton(config);
        }

        public static T Get<T>(bool required = true)
        {
            ServiceProvider.MustNotBeNull(nameof(ServiceProvider));

            if (required) return ServiceProvider.GetRequiredService<T>();
            else return ServiceProvider.GetService<T>();
        }
    }
}
