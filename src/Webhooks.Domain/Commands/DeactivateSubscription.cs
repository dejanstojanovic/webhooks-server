using System;

namespace Webhooks.Domain.Commands
{
    public class DeactivateSubscription
    {
        public DeactivateSubscription(Guid id)
        {
            this.Id = id;
        }
        public Guid Id { get; private set; }
    }
}
