using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Webhooks.Data.Infrastructure.Extensions;
using Webhooks.Data.Repositories;

namespace Webhooks.Data.Extensions
{
    public static class DependencyInjection
    {
        public static void AddDataRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDatabaseContext(configuration);

            services.Scan(scan => scan
             .FromAssembliesOf(typeof(ISubscriptonsRepository))
             .AddClasses(classes => classes.InNamespaces(typeof(ISubscriptonsRepository).Namespace))
             .AsImplementedInterfaces()
             .WithScopedLifetime()
            );

            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
