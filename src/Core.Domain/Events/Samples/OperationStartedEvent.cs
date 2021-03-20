using Core.Domain.Validations;
using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Domain.Events.Samples
{
    public class OperationStartedEvent : IDomainEvent
    {
        [GuidNotEmpty]
        public Guid Id { get; set; }

        [Required]
        public String OperationName { get; set; }
        public String Username { get; set; }
        public DateTime DateTime { get; set; }
        public OperationStartedEvent()
        {
            DateTime = DateTime.UtcNow;
        }
    }
}
