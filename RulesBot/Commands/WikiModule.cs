using Discord.Commands;
using Flurl;
using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RulesBot.Commands
{
    public class WikiModule : ModuleBase<SocketCommandContext>
    {
        [Command("wiki")]
        public async Task WikiSearch(string searchArg)
        {
            string requestUrl = "https://en.wikipedia.org/w/api.php"
                .SetQueryParams(new
                {
                    action = "opensearch",
                    limit = 3,
                    @namespace = 0,
                    format = "json",
                    search = searchArg
                });

            var result = await requestUrl.GetAsync();
            var resultJson = await result.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(resultJson);

            int idx = 0;
            var articleTitles = new List<string>();
            var articleUrls = new List<string>();

            foreach (var elem in jsonDoc.RootElement.EnumerateArray())
            {
                idx++;
                if (idx == 1 || idx == 3) continue;
                else if (idx == 2)
                {
                    foreach (var title in elem.EnumerateArray())
                        articleTitles.Add(title.GetString());
                }
                else if (idx == 4)
                {
                    foreach (var article in elem.EnumerateArray())
                        articleUrls.Add(article.GetString());
                }
            }

            int count = Math.Min(articleTitles.Count, articleUrls.Count);
            var sb = new StringBuilder();

            if (count == 0)
            {
                await Context.Channel.SendMessageAsync($"No results found for query \"{searchArg}\"");
            }
            else
            {
                sb.AppendFormat("Top {0} results for query \"{1}\":\n", count, searchArg);

                for (int i = 0; i < count; i++)
                {
                    sb.AppendFormat("\t{0}) {1} - {2}\n", i + 1, articleTitles[i], articleUrls[i]);
                }

                await Context.Channel.SendMessageAsync(sb.ToString());
            }
        }
    }
}
