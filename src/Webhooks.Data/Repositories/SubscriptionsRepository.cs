using Microsoft.EntityFrameworkCore;
using Webhooks.Data.Infrastructure;
using Webhooks.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webhooks.Data.Repositories
{
    public class SubscriptionsRepository : ISubscriptonsRepository
    {
        readonly WebhooksDataContext _dataContext;
        public SubscriptionsRepository(WebhooksDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task AddSubscription(Subscription subscription)
        {
            await _dataContext.Subscriptions.AddAsync(subscription);
        }

        public async Task<Subscription> GetSubscription(Guid id)
        {
            return await _dataContext.Subscriptions.AsNoTracking()
                             .Include(s => s.Headers)
                             .SingleOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Subscription>> GetSubscriptions(bool? active = null)
        {
            var query = _dataContext.Subscriptions.AsNoTracking();
            if (active != null)
                query = query.Where(s => s.Active == active.Value);

            return await query
                          .Include(s => s.Headers)
                          .ToArrayAsync();
        }

        public async Task<IEnumerable<Subscription>> GetSubscriptionsForEvent(string eventName)
        {
            return await _dataContext.Subscriptions.AsNoTracking()
                            .Include(s => s.Headers)
                            .Where(s => s.Event == eventName).ToArrayAsync();
        }

        public async Task RemoveSubscription(Guid id)
        {
            var subscription = await _dataContext.Subscriptions.SingleOrDefaultAsync(s => s.Id == id);
            if (subscription != null)
                _dataContext.Subscriptions.Remove(subscription);

        }

        public async Task<bool> SubscriptionExists(Guid id)
        {
            return await _dataContext.Subscriptions.AnyAsync(s => s.Id == id);
        }

        public async Task<bool> SubscriptionExists(string @event, Uri endpoint)
        {
            return await _dataContext.Subscriptions.AnyAsync(s => s.Event == @event && s.Endpoint == endpoint);
        }
    }
}
