using Core.Domain.Events;
using MassTransit;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Webhooks.Data.Repositories;
using Webhooks.WorkerService.Constants;

namespace Webhooks.WorkerService.Consumers
{
    public class DomainEventConsumer<T> : IConsumer<T> where T : class, IDomainEvent
    {
        readonly ISubscriptonsRepository _subscriptionsRepository;
        readonly IHttpClientFactory _httpClientFactory;

        public DomainEventConsumer(
            ISubscriptonsRepository subscriptonsRepository,
            IHttpClientFactory httpClientFactory)
        {
            _subscriptionsRepository = subscriptonsRepository;
            _httpClientFactory = httpClientFactory;
        }

        public async Task Consume(ConsumeContext<T> context)
        {
            var subscriptionIdParsed = context.ReceiveContext.InputAddress.Segments.Last().Replace($"{typeof(T).FullName}_", string.Empty, System.StringComparison.InvariantCultureIgnoreCase);
            if (!Guid.TryParse(subscriptionIdParsed, out Guid subscriptionId) || subscriptionId == Guid.Empty)
                throw new Exception("Failed to parse ID");

            var subscription = await _subscriptionsRepository.GetSubscription(subscriptionId);
            if (subscription == null)
                throw new Exception($"Cannot find subscription {subscriptionId}");

            if (!subscription.Active)
                await Task.CompletedTask; // Not active subscription

            var client = _httpClientFactory.CreateClient(HttpClientNames.WebhookSubscriptionHttpClient);
            client.DefaultRequestHeaders.Clear();

            foreach (var header in subscription.Headers)
                client.DefaultRequestHeaders.Add(header.Key, header.Value);
            var content = new StringContent(JsonConvert.SerializeObject(context.Message), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(subscription.Endpoint, content);

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Unable to POST data for subscription {subscription.Id} to {subscription.Endpoint}, status code {response.StatusCode}");

        }
    }
}
