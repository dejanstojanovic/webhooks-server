using System;

namespace Webhooks.Common.Extensions
{
    public static class MasstransitExtensions
    {
        public static string GetEndpointName(this Type type)
        {
            return $"{type.Namespace}:{type.Name}";
        }
    }
}
