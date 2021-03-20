using Webhooks.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Webhooks.Data.Repositories
{
    public interface ISubscriptonsRepository
    {
        Task AddSubscription(Subscription subscription);
        Task RemoveSubscription(Guid id);
        Task<Subscription> GetSubscription(Guid id);
        Task<IEnumerable<Subscription>> GetSubscriptions(bool? active = null);
        Task<IEnumerable<Subscription>> GetSubscriptionsForEvent(String eventName);
        Task<bool> SubscriptionExists(Guid id);
        Task<bool> SubscriptionExists(String @event, Uri endpoint);
    }
}
