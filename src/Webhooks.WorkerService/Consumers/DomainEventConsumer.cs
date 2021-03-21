using Core.Domain.Events;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using System.Threading.Tasks;

namespace Webhooks.WorkerService.Consumers
{
    public class DomainEventConsumer<T> : IConsumer<T> where T : class, IDomainEvent
    {
        public async Task Consume(ConsumeContext<T> context)
        {
            await Task.CompletedTask;
        }
    }

    public class DomainEventConsumerDefinition<T> : ConsumerDefinition<DomainEventConsumer<T>> where T: class, IDomainEvent
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<DomainEventConsumer<T>> consumerConfigurator)
        {
            endpointConfigurator.ConfigureConsumeTopology = false;
        }
    }
}
