using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Webhooks.Application.Services;

namespace Webhooks.Application.Extensions
{
    public static class EventServiceExtensions
    {
        public static async Task Publish(this IEventsService eventsService,Type type, Object @event)
        {
            var method = eventsService.GetType().GetMethod(nameof(eventsService.Publish)).MakeGenericMethod(type);
            var model = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(@event), type);
            var task = (Task)method.Invoke(eventsService, new[] { model });
            await task.ConfigureAwait(false);
        }
    }
}
