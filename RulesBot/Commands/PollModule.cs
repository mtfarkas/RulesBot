using Discord;
using Discord.Commands;
using RulesBot.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RulesBot.Commands
{
    [Group("poll")]
    public class PollModule: RulesBotModule
    {
        private static ConcurrentDictionary<ulong, PollModel> polls = new ConcurrentDictionary<ulong, PollModel>();

        [Command("start")]
        public Task AddPoll(params string[] choices)
        {
            var choiceList = choices.Where(item => item != null && !string.IsNullOrEmpty(item.Trim())).Select(item => item.Trim()).ToList();
            ulong currentChannel = Context.Message.Channel.Id;

            if (polls.Any(item => item.Value.IsValid && item.Value.Channeld == currentChannel))
            {
                return Context.Channel.SendMessageAsync("There's already a poll running here, silly!");
            }

            polls.TryRemove(currentChannel, out _);

            var pollModel = new PollModel
            {
                Channeld = currentChannel,
                StartMessageId = Context.Message.Id,
                Choices = choiceList.ToList(),
                PollStart = DateTime.UtcNow
            };

            polls.TryAdd(currentChannel, pollModel);

            string respStr = $"Hey, <@{Context.Message.Author.Id}> started a poll!\n";

            respStr += "Type one of these in chat to vote. If you vote multiple times, only the last one will count:\n";
            respStr += string.Join('\n', choiceList.Select(item => $" - {item}"));

            return Context.Channel.SendMessageAsync(respStr);
        }

        [Command("close")]
        public async Task ClosePoll()
        {
            ulong currentChannel = Context.Message.Channel.Id;

            if (!polls.TryGetValue(currentChannel, out var pollModel))
            {
                await Context.Channel.SendMessageAsync("There's no poll running in this channel!");
                return;
            }

            var pollAnswersDictionary = new Dictionary<ulong, string>();
            var messages = await Context.Channel.GetMessagesAsync(pollModel.StartMessageId, Discord.Direction.After, 500, Discord.CacheMode.AllowDownload).FlattenAsync();

            foreach (var msg in messages)
            {
                if (msg.Author.IsBot)
                {
                    continue;
                }

                string msgContent = msg.Content.ToUpper();

                string selectedChoice = pollModel.Choices.FirstOrDefault(item => msgContent.Contains(item.ToUpper()));

                if (selectedChoice != null && !string.IsNullOrEmpty(selectedChoice.Trim()))
                {
                    pollAnswersDictionary[msg.Author.Id] = selectedChoice;
                }
            }

            int voteCount = pollAnswersDictionary.Count;

            if (voteCount == 0)
            {
                await Context.Channel.SendMessageAsync("No votes, no winners. :(");
                return;
            }

            string respStr = "The poll is now closed! Here are the results:\n";

            respStr += $" Total votes: {voteCount}\n";

            foreach (var choice in pollModel.Choices)
            {
                int choiceCount = pollAnswersDictionary.Count(item => item.Value == choice);
                double choicePercentage = choiceCount / (double)voteCount * 100d;

                respStr += $" - {choice}: {choiceCount} votes ({choicePercentage:0.00}%)\n";
            }

            await Context.Channel.SendMessageAsync(respStr);
        }
    }
}
