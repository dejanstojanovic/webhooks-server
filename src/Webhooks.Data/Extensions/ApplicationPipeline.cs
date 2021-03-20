using Microsoft.AspNetCore.Builder;
using Webhooks.Data.Infrastructure.Extensions;

namespace Webhooks.Data.Extensions
{
    public static class ApplicationPipeline
    {
        public static void UseDataServices(this IApplicationBuilder app)
        {
            app.UseDataInfrastructure();
        }
    }
}
