using Microsoft.EntityFrameworkCore;
using RulesBot.Data.Entities;
using System.Reflection;

namespace RulesBot.Data
{
    public class RulesBotContext: DbContext
    {
        public DbSet<Phrase> Phrases { get; set; }

        public RulesBotContext(DbContextOptions<RulesBotContext> options): base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
