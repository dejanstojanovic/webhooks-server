using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Webhooks.Data.Repositories;
using Webhooks.Domain.Commands;

namespace Webhooks.WorkerService.Consumers
{
    public class ActivateSubscriptionConsumer : IConsumer<ActivateSubscription>
    {
        readonly IBusControl _busControl;
        readonly ISubscriptonsRepository _subsriptionsRepository;
        readonly ILogger<ActivateSubscriptionConsumer> _logger;

        public ActivateSubscriptionConsumer(
            IBusControl busControl,
            ISubscriptonsRepository subscriptonsRepository,
            ILogger<ActivateSubscriptionConsumer> logger)
        {
            _busControl = busControl;
            _subsriptionsRepository = subscriptonsRepository;
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<ActivateSubscription> context)
        {
            var subscription = await _subsriptionsRepository.GetSubscription(context.Message.Id);

            if (subscription == null)
                throw new Exception($"Cannot find subscription {context.Message.Id}");

            if (!subscription.Active)
            {
                _logger.LogWarning($"Subscription {context.Message.Id} is not active");
                return;
            }

            // TODO: Connect new consumer

            await Task.CompletedTask;

        }
    }

}
