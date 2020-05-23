using Discord.Commands;
using RulesBot.Core;
using RulesBot.Core.Repositories;
using RulesBot.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YAUL.Extensions;

namespace RulesBot.Commands
{
    public class SystemModule : RulesBotModule
    {
        private readonly Dictionary<string, PhraseType> TagTypeMap = new Dictionary<string, PhraseType>
        {
            { "verb", PhraseType.Verb },
            { "wednesday", PhraseType.Wednesday },
            { "streets", PhraseType.Streets },
            { "cute", PhraseType.Cute },
            { "rule2", PhraseType.Rule2 }
        };

        private readonly IPhraseRepository phraseRepository;
        public SystemModule(IPhraseRepository phraseRepository)
        {
            this.phraseRepository = phraseRepository;
        }

        [Command("del")]
        public async Task Delete(ulong message)
        {
            var msg = await Context.Channel.GetMessageAsync(message);
            if (msg != null && msg.Author.Id == Context.Client.CurrentUser.Id) await msg.DeleteAsync();
        }

        [Command("ping")]
        public Task Pong()
            => Context.Channel.SendMessageAsync("pong!");

        [Command("uptime")]
        public Task Uptime()
            => Context.Channel.SendMessageAsync((DateTime.UtcNow - Program.Started).PrettyPrint());

        [Command("add")]
        [RequireOwner]
        public async Task Add(string category, string content)
        {
            if (content.Length > 140)
            {
                await Context.Channel.SendMessageAsync("That's too long, try to limit yourself to 140 characters for now.");
                return;
            }

            if (!TagTypeMap.TryGetValue(category.ToLower(), out var type)) return;

            var newPhrase = await phraseRepository.AddPhraseAsync(content, type);

            await Context.Channel.SendMessageAsync($"Phrase added, ID: {newPhrase.Id}");
        }

        [Command("rem")]
        [RequireOwner]
        public async Task Remove(int id)
        {
            if (id < 0)
            {
                await Context.Channel.SendMessageAsync("That's an invalid ID");
                return;
            }

            var result = await phraseRepository.DeletePhraseAsync(id);

            if (result) await Context.Channel.SendMessageAsync($"Phrase {id} deleted");
            else await Context.Channel.SendMessageAsync($"No phrase with the ID {id}, or deletion failed");
        }

        [Command("phrase")]
        [RequireOwner]
        public async Task GetPhrase(int id)
        {
            if (id < 0)
            {
                await Context.Channel.SendMessageAsync("That's an invalid ID");
                return;
            }

            var result = await phraseRepository.FindPhraseAsync(id);

            if (result != null) await Context.Channel.SendMessageAsync($"ID: {id}, \"{result.Value}\"");
            else await Context.Channel.SendMessageAsync($"No phrase with the ID {id}");
        }
    }
}
