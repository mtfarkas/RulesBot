using Discord.Commands;
using Flurl;
using Flurl.Http;
using RulesBot.Core;
using RulesBot.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using YAUL.Utilities;

namespace RulesBot.Commands
{
    public class ContentModule: RulesBotModule
    {
        private const string RedditUrlFormat = @"https://www.reddit.com/r/{0}/hot/.json?count={1}";

        [Command("prime")]
        public Task PrimeChime()
            => Context.Channel.SendFileAsync(PathUtils.MakeAbsolute("Assets", "prime_chime.png"), "Did someone say [Prime Chime]?");

        #region Reddit
        [Command("top")]
        public async Task GetSubredditTops(string subreddit)
        {
            if (subreddit.StartsWith("/r/")) subreddit = subreddit.Replace("/r/", "");

            var response = await GetTopPostsFromSubreddit(subreddit, 3);

            if (response == null) await Context.Channel.SendMessageAsync("I couldn't find what you're looking for, sorry :(");
            else
            {
                string message = ConstructFromResponse(response, subreddit);

                await Context.Channel.SendMessageAsync(message);
            }
        }

        [Command("til")]
        public async Task GetTopTIL()
        {
            var response = await GetTopPostsFromSubreddit("todayilearned", 3);

            if (response == null) await Context.Channel.SendMessageAsync("I couldn't find what you're looking for, sorry :(");
            else
            {
                string message = ConstructFromResponse(response, "todayilearned");

                await Context.Channel.SendMessageAsync(message);
            }
        }

        private string ConstructFromResponse(IEnumerable<RedditResponseModel.RedditResponseInner.SubredditData> response, string subreddit)
        {
            int idx = 1;
            var sb = new StringBuilder();

            sb.AppendFormat("Here's the top {0} posts today for /r/{1}:\n", 3, subreddit);

            foreach (var child in response)
            {
                sb.AppendFormat("{0}) {1} -- <https://reddit.com{2}>\n", idx++, child.Data.Title, child.Data.Permalink);
            }

            return sb.ToString();
        }

        // wtf
        private async Task<IEnumerable<RedditResponseModel.RedditResponseInner.SubredditData>> GetTopPostsFromSubreddit(string subreddit, int count)
        {
            string requestUrl = String.Format(RedditUrlFormat, subreddit, count);
            try
            {
                var result = await requestUrl.GetJsonAsync<RedditResponseModel>();
                return result.Data.Children.Where(item => !item.Data.Stickied).Take(count);
            }
            catch { return null; }

        }
        #endregion

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
                    sb.AppendFormat("\t{0}) {1} - <{2}>\n", i + 1, articleTitles[i], articleUrls[i]);
                }

                await Context.Channel.SendMessageAsync(sb.ToString());
            }
        }
    }
}
