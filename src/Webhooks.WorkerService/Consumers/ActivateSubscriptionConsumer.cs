using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
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
