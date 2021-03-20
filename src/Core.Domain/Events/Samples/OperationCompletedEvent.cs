using System;

namespace Core.Domain.Events.Samples
{
    public class OperationCompletedEvent : IDomainEvent
    {
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        public OperationCompletedEvent()
        {
            Id = Guid.NewGuid();
            DateTime = DateTime.UtcNow;
        }
    }
}
