using YAUL.Extensions;

namespace RulesBot.Core.Data
{
    public class JokeModel
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Setup { get; set; }
        public string Punchline { get; set; }

        public string Complete
        {
            get => $"{Setup.FirstCharToUpper()}\n{Punchline.FirstCharToUpper()}";
        }
    }
}
