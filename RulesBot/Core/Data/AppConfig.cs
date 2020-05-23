using System.Collections.Generic;

namespace RulesBot.Core.Data
{
    public class AppConfig
    {
        public BotSettings BotSettings { get; set; }
        public StreamNotifierSettings StreamNotifierSettings { get; set; }

        public IEnumerable<string> Verbs { get; set; }
        public IEnumerable<string> TwitchFriends { get; set; }
    }

    public class BotSettings
    {
        public char CommandCharacter { get; set; }
        public bool EnableChannelBlacklist { get; set; }
        public IEnumerable<ulong> BlacklistedChannels { get; set; }
    }

    public class StreamNotifierSettings
    {
        public bool Enabled { get; set; }
        public ulong NotificationGuild { get; set; }
        public ulong NotificationChannel { get; set; }
    }
}
