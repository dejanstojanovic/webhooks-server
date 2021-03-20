using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Webhooks.Data.Infrastructure.Extensions
{
    public  static class ApplicationPipeline
    {
        public static void UseDataInfrastructure(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<WebhooksDataContext>())
                {
                    context.Database.Migrate();
                }
            }
        }
    }
}
