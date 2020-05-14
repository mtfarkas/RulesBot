using System;
using System.Collections.Generic;

namespace RulesBot.Core.Data
{
    public class AppConfig
    {
        public bool IsDebug { get; set; }
        public string BotToken { get; set; }
        public DateTime CurrentRun { get; set; }
        public bool ApplyChannelFilter { get; set; }
        public IEnumerable<ulong> EnabledChannels { get; set; }
        public string CommandCharacter { get; set; }

        public IEnumerable<string> Verbs { get; set; }
        public IEnumerable<string> Frogs { get; set; }
        public IEnumerable<string> Rule2Phrases { get; set; }
        public IEnumerable<string> TwitchFriends { get; set; }
        public IEnumerable<string> CutePhrases { get; set; }
    }
}
