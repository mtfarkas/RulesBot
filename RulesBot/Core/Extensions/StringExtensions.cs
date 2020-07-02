using System.Text;

namespace RulesBot.Core.Extensions
{
    public static class StringExtensions
    {
        public static string Sanitize(this string source)
        {
            var sb = new StringBuilder();
            foreach (char c in source)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }
}
