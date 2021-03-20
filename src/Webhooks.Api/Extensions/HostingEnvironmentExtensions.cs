using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webhooks.Api.Extensions
{
    public static class HostingEnvironmentExtensions
    {
        public const string LOCAL = "LOCAL";
        public const string LOCALHOST = "LOCALHOST";

        public static bool IsLocal(this IHostEnvironment environment)
        {
            return environment.EnvironmentName.Equals(LOCAL, StringComparison.CurrentCultureIgnoreCase) ||
                    environment.EnvironmentName.Equals(LOCALHOST, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
