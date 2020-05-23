using System;

namespace RulesBot.Data.Entities
{
    public enum PhraseType
    {
        Unknown,
        Verb,
        Rule2,
        Cute,
        Wednesday,
        Streets
    }

    public class Phrase
    {
        public int Id { get; set; }
        public PhraseType Type { get; set; }
        public string Value { get; set; }
    }
}
