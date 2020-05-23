using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RulesBot.Data.Entities;

namespace RulesBot.Data.EntityConfiguration
{
    internal class TwitchFriendConfiguration : IEntityTypeConfiguration<TwitchFriend>
    {
        public void Configure(EntityTypeBuilder<TwitchFriend> builder)
        {
            builder.ToTable("TwitchFriend");

            builder.HasKey(item => item.Channel);

            builder.Property(item => item.Channel).IsRequired();
        }
    }
}
