using MassTransit;
using System;
using System.Threading.Tasks;
using Webhooks.Domain.Commands;

namespace Webhooks.WorkerService.Consumers
{
    public class ActivateSubscriptionConsumer : IConsumer<ActivateSubscription>
    {
        public async Task Consume(ConsumeContext<ActivateSubscription> context)
        {
            await Task.CompletedTask;
        }
    }
}
