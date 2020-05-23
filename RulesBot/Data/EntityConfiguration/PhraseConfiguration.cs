using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RulesBot.Data.Entities;

namespace RulesBot.Data.EntityConfiguration
{
    internal class PhraseConfiguration : IEntityTypeConfiguration<Phrase>
    {
        public void Configure(EntityTypeBuilder<Phrase> phrase)
        {
            phrase.ToTable("Phrase");

            phrase.HasKey(item => item.Id);

            phrase.Property(item => item.Type).IsRequired();
            phrase.Property(item => item.Value).IsRequired();
            phrase.Property(item => item.AddedOn).IsRequired();

            phrase.Property(item => item.AddedBy).IsRequired(false);
        }
    }
}
