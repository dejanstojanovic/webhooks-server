using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Webhooks.Data.Infrastructure.Constants;

namespace Webhooks.Data.Infrastructure.Extensions
{
    public static class DependencyInjection
    {
        public static void AddDatabaseContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<WebhooksDataContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString(DbContextConfigConstants.DB_CONNECTION_CONFIG_NAME),
                    x =>
                    {
                        x.MigrationsHistoryTable("__EFMigrationsHistory");
                        x.MigrationsAssembly(typeof(WebhooksDataContext).Assembly.GetName().Name);
                    }
                );
            });

        }

    }
}
