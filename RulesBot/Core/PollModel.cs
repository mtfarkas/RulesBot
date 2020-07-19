using System;
using System.Collections.Generic;

namespace RulesBot.Core
{
    public class PollModel
    {
        public static readonly TimeSpan PollMaxLifeTime = TimeSpan.FromMinutes(15);

        public DateTime PollStart { get; set; }
        public ulong StartMessageId { get; set; }
        public ulong Channeld { get; set; }
        public IReadOnlyCollection<string> Choices { get; set; }
        public bool IsValid { get => (DateTime.UtcNow - PollStart) <= PollMaxLifeTime; }
    }
}
