using MassTransit;
using System.Threading.Tasks;
using Webhooks.Domain.Commands;

namespace Webhooks.WorkerService.Consumers
{
    public class DeactivateSubscriptionConsumer : IConsumer<DeactivateSubscription>
    {
        readonly IBusControl _busControl;
        public DeactivateSubscriptionConsumer(IBusControl busControl)
        {
            _busControl = busControl;
        }

        public async Task Consume(ConsumeContext<DeactivateSubscription> context)
        {
            await Task.CompletedTask;
        }
    }

}
