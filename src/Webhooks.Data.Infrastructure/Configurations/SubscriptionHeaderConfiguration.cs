using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Webhooks.Domain.Models;

namespace Webhooks.Data.Infrastructure.Configurations
{
    public class SubscriptionHeaderConfiguration : IEntityTypeConfiguration<SubscriptionHeader>
    {
        public void Configure(EntityTypeBuilder<SubscriptionHeader> builder)
        {
            builder.ToTable("SubscriptionHeaders", "webhooks");

            builder.Property(h => h.SubscriptionId)
                .IsRequired();

            builder.Property(h => h.Key)
                .IsRequired();

            builder.Property(h => h.Value)
                .IsRequired();

            builder.HasKey(b => new { b.SubscriptionId, b.Key });

        }
    }
}
