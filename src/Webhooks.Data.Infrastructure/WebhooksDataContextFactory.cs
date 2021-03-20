using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Webhooks.Data.Infrastructure.Constants;
using System.IO;

namespace Webhooks.Data.Infrastructure
{

    public class WebhooksDataContextFactory : IDesignTimeDbContextFactory<WebhooksDataContext>
    {
        public WebhooksDataContext CreateDbContext(string[] args)
        {
            var dbContext = new WebhooksDataContext(new DbContextOptionsBuilder<WebhooksDataContext>().UseSqlServer(
                 new ConfigurationBuilder()
                     .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), $"appsettings.json"))
                     .AddEnvironmentVariables()
                     .Build()
                     .GetConnectionString(DbContextConfigConstants.DB_CONNECTION_CONFIG_NAME)
                 ).Options);
            dbContext.Database.Migrate();
            return dbContext;
        }

    }
}
