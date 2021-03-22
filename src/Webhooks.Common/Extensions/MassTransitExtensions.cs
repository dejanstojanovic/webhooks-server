using System;

namespace Webhooks.Common.Extensions
{
    public static class MassTransitExtensions
    {
        public static Uri GetQueueAddress(this Type type)
        {
            return new Uri($"queue:{type.FullName}");
        }

        public static Uri GetExchangeAddress(this Type type)
        {
            return new Uri($"exchange:{type.FullName}");
        }
    }
}
