using System;

namespace Webhooks.Domain.Commands
{
    public class ActivateSubscription
    {
        public ActivateSubscription(Guid id)
        {
            this.Id = id;
        }
        public Guid Id { get; private set; }
    }
}
