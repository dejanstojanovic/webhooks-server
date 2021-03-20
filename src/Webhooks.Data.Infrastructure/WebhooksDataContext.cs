using Microsoft.EntityFrameworkCore;
using Webhooks.Domain.Models;

namespace Webhooks.Data.Infrastructure
{
    public class WebhooksDataContext : DbContext
    {

        #region Constructors
        public WebhooksDataContext() : base()
        {

        }

        public WebhooksDataContext(DbContextOptions<WebhooksDataContext> options) : base(options)
        {
        }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        }

        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<SubscriptionHeader> SubscriptionHeaders { get; set; }
    }
}
