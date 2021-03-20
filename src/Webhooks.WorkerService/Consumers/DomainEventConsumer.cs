using Core.Domain.Events;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webhooks.WorkerService.Consumers
{
    public class DomainEventConsumer<T> : IConsumer<T> where T : class, IDomainEvent
    {
        public async Task Consume(ConsumeContext<T> context)
        {
            
        }
    }
}
