using System.Collections.Generic;

namespace RulesBot.Core.Data
{
    public class RedditResponseModel
    {
        public RedditResponseInner Data { get; set; }
        public class RedditResponseInner
        {
            public IEnumerable<SubredditData> Children { get; set; }
            public class SubredditData
            {
                public SubredditDataInner Data { get; set; }
                public class SubredditDataInner
                {
                    public string Title { get; set; }
                    public string Permalink { get; set; }
                    public int Score { get; set; }
                    public bool Stickied { get; set; }

                }
            }
        }
    }
}
