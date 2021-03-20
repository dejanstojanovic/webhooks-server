using Microsoft.Extensions.Hosting;
using System;

namespace Webhooks.Common.Extensions
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
