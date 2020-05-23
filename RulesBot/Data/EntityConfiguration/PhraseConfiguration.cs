using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RulesBot.Data.Entities;
using System;

namespace RulesBot.Data.EntityConfiguration
{
    internal class PhraseConfiguration : IEntityTypeConfiguration<Phrase>
    {
        public void Configure(EntityTypeBuilder<Phrase> phrase)
        {
            phrase.ToTable("Phrase");

            phrase.HasKey(item => item.Id);

            var enumConverter = new ValueConverter<PhraseType, string>(
                v => v.ToString(),
                v => (PhraseType)Enum.Parse(typeof(PhraseType), v)
            );

            phrase.Property(item => item.Type)
                .HasConversion(enumConverter)
                .IsRequired();

            phrase.Property(item => item.Value).IsRequired();
        }
    }
}
