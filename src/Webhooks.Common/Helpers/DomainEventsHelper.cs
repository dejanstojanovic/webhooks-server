using Core.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Webhooks.Common.Helpers
{
    public static class DomainEventsHelper
    {
        static IEnumerable<Type> _eventTypes = typeof(IDomainEvent).Assembly.GetTypes()
                .Where(t => !t.IsAbstract && t.IsPublic && typeof(IDomainEvent).IsAssignableFrom(t))
                .OrderBy(t=>t.Name);
        public static IEnumerable<Type> GetDomainEventTypes()
        {
            return _eventTypes;
        }
    }
}
