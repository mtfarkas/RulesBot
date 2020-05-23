using Microsoft.EntityFrameworkCore;
using RulesBot.Data;
using RulesBot.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RulesBot.Core.Repositories
{
    public interface IPhraseRepository
    {
        Task<IEnumerable<Phrase>> FindPhrasesAsync(PhraseType type);
        Task<Phrase> FindPhraseAsync(int id, bool tracking = false);
        Task<Phrase> AddPhraseAsync(string value, PhraseType type);
        Task<bool> DeletePhraseAsync(int id);
    }

    public class PhraseRepository : IPhraseRepository
    {
        private readonly RulesBotContext context;
        public PhraseRepository(RulesBotContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Phrase>> FindPhrasesAsync(PhraseType type)
        {
            return await context.Phrases.AsNoTracking()
                .Where(item => item.Type == type)
                .ToListAsync();
        }

        public async Task<Phrase> AddPhraseAsync(string value, PhraseType type)
        {
            int newId;
            try { newId = await context.Phrases.AsNoTracking().MaxAsync(item => item.Id); }
            catch { newId = 0; }
            newId += 1;

            var phrase = new Phrase
            {
                Id = newId,
                Type = type,
                Value = value
            };

            await context.Phrases.AddAsync(phrase);
            await context.SaveChangesAsync();

            return phrase;
        }

        public Task<Phrase> FindPhraseAsync(int id, bool tracking = false)
        {
            var result = context.Phrases.AsQueryable();

            if (!tracking) result = result.AsNoTracking();

            return result.FirstOrDefaultAsync(item => item.Id == id);
        }

        public async Task<bool> DeletePhraseAsync(int id)
        {
            var phrase = await FindPhraseAsync(id, true);

            if (phrase == null) return false;

            context.Phrases.Remove(phrase);
            int removed = await context.SaveChangesAsync();

            return removed != 0;
        }
    }
}
