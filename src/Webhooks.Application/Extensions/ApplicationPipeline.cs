using Microsoft.AspNetCore.Builder;
using Webhooks.Data.Extensions;

namespace Webhooks.Application.Extensions
{
    public static class ApplicationPipeline
    {
        public static void UseApplicationServices(this IApplicationBuilder app)
        {
            app.UseDataServices();
        }
    }
}
