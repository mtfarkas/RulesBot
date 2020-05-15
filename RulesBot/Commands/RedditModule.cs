using Discord.Commands;
using Flurl;
using Flurl.Http;
using RulesBot.Core.Data;
using RulesBot.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RulesBot.Commands
{
    public class RedditModule : RulesBotCommand
    {
        private const string UrlFormat = @"https://www.reddit.com/r/{0}/hot/.json?count={1}";

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
            string requestUrl = String.Format(UrlFormat, subreddit, count);
            try
            {
                var result = await requestUrl.GetJsonAsync<RedditResponseModel>();
                return result.Data.Children.Where(item => !item.Data.Stickied).Take(count);
            }
            catch { return null; }

        }
    }
}
