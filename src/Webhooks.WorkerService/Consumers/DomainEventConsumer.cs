using Core.Domain.Events;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using Microsoft.Extensions.Logging;
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
        readonly ILogger<DomainEventConsumer<T>> _logger;

        public DomainEventConsumer(
            ISubscriptonsRepository subscriptonsRepository,
            IHttpClientFactory httpClientFactory,
            ILogger<DomainEventConsumer<T>> logger)
        {
            _subscriptionsRepository = subscriptonsRepository;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<T> context)
        {
            var subscriptionIdParsed = context.ReceiveContext.InputAddress.Segments.Last().Replace($"{typeof(T).FullName}_", string.Empty, System.StringComparison.InvariantCultureIgnoreCase);
            if (!Guid.TryParse(subscriptionIdParsed, out Guid subscriptionId) || subscriptionId == Guid.Empty)
                throw new ArgumentException("Failed to parse ID");

            var subscription = await _subscriptionsRepository.GetSubscription(subscriptionId);
            if (subscription == null)
                throw new ArgumentException($"Cannot find subscription {subscriptionId}");

            if (!subscription.Active)
            {
                _logger.LogWarning($"Subscription {subscriptionId} is not active");
                return;
            }

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

    public class DomainEventConsumerDefinition<T> : ConsumerDefinition<DomainEventConsumer<T>> where T : class, IDomainEvent
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<DomainEventConsumer<T>> consumerConfigurator)
        {
            endpointConfigurator.ConfigureConsumeTopology = false;
        }
    }
}
