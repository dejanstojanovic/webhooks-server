using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Webhooks.Domain.Models;
using System;

namespace Webhooks.Data.Infrastructure.Configurations
{
    public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
    {
        public void Configure(EntityTypeBuilder<Subscription> builder)
        {
            builder.ToTable("Subscriptions", "webhooks");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Active)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(s => s.Event)
                .IsRequired();

            builder.Property(e => e.Endpoint)
                .IsRequired()
                .HasConversion(
                    v => v.ToString(),
                    v => new Uri(v));

            builder.HasIndex(b => new { b.Event, b.Endpoint })
                .IsUnique();

            builder.HasMany(s => s.Headers)
                .WithOne(s => s.Subscription)
                .HasForeignKey(h => h.SubscriptionId)
                .OnDelete(DeleteBehavior.ClientCascade);

        }
    }
}
