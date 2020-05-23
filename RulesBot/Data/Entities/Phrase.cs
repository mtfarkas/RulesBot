using System;

namespace RulesBot.Data.Entities
{
    public enum PhraseType
    {

    }

    public class Phrase
    {
        public Guid Id { get; set; }
        public PhraseType Type { get; set; }
        public string Value { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedOn { get; set; }
    }
}
