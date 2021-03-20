using Webhooks.Application.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Webhooks.Application.Services
{
    /// <summary>
    /// Subscription methods
    /// </summary>
    public interface ISubscriptionsService
    {
        /// <summary>
        /// Get subscription with specific id
        /// </summary>
        /// <param name="id">Subscription unique id</param>
        /// <returns></returns>
        Task<SubscriptionModel> GetSubscription(Guid id);

        /// <summary>
        /// Get subscriptions which are active
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<SubscriptionModel>> GetActiveSubscriptions();

        /// <summary>
        /// Get all subscriptions, both active and inactive
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<SubscriptionModel>> GetSubscriptions();

        /// <summary>
        /// Registers new domain event subscription
        /// </summary>
        /// <param name="subscriptionModel">Subscription model</param>
        /// <returns></returns>
        Task AddSubscription(SubscriptionAddModel subscriptionModel);

        /// <summary>
        /// Removes existing subscription
        /// </summary>
        /// <param name="id">Subscription Id</param>
        /// <returns></returns>
        Task RemoveSubscription(Guid id);

        /// <summary>
        /// Retrieves list of subscriptions of specific event
        /// </summary>
        /// <param name="eventName">Event type full name</param>
        /// <returns></returns>
        Task<IEnumerable<SubscriptionModel>> GetSubscriptionsForEvent(String eventName);

        /// <summary>
        /// Retrieves list of subscriptions of specific event
        /// </summary>
        /// <param name="event">Domain event type</param>
        /// <returns></returns>
        Task<IEnumerable<SubscriptionModel>> GetSubscriptionsForEvent(Type @event);
    }
}
