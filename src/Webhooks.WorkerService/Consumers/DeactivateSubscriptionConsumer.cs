using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using System.Threading.Tasks;
using Webhooks.Domain.Commands;

namespace Webhooks.WorkerService.Consumers
{
    public class DeactivateSubscriptionConsumer : IConsumer<DeactivateSubscription>
    {
        public async Task Consume(ConsumeContext<DeactivateSubscription> context)
        {
            await Task.CompletedTask;
        }
    }

    public class DeactivateSubscriptionConsumerDefinition : ConsumerDefinition<DeactivateSubscriptionConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<DeactivateSubscriptionConsumer> consumerConfigurator)
        {
            endpointConfigurator.ConfigureConsumeTopology = false;
        }
    }

}
