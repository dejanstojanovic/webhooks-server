using AutoMapper;
using Webhooks.Application.Exceptions;
using Webhooks.Application.Models;
using Webhooks.Data;
using Webhooks.Data.Repositories;
using Webhooks.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MassTransit;
using Webhooks.Domain.Commands;

namespace Webhooks.Application.Services
{
    /// <inheritdoc/>
    public class SubscriptionsService : ISubscriptionsService
    {
        readonly ISubscriptonsRepository _subscriptionsRepositry;
        readonly IUnitOfWork _unitOfWork;
        readonly IMapper _mapper;
        readonly IBusControl _busControl;
        readonly ISendEndpointProvider _sendEndpointProvider;
        public SubscriptionsService(
            ISubscriptonsRepository subscriptonsRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ISendEndpointProvider sendEndpointProvider,
            IBusControl busControl)
        {
            _subscriptionsRepositry = subscriptonsRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _busControl = busControl;
            _sendEndpointProvider = sendEndpointProvider;
        }

        /// <inheritdoc/>
        public async Task AddSubscription(SubscriptionAddModel subscriptionModel)
        {
            if (await _subscriptionsRepositry.SubscriptionExists(subscriptionModel.Id) ||
                await _subscriptionsRepositry.SubscriptionExists(subscriptionModel.Event, subscriptionModel.Endpoint))
                throw new DuplicateException();

            var subscription = _mapper.Map<Subscription>(subscriptionModel);
            await _subscriptionsRepositry.AddSubscription(subscription);

            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"exchange:{typeof(ActivateSubscription).FullName}"));
            await endpoint.Send(new ActivateSubscription(subscriptionModel.Id));

            await _unitOfWork.Save();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<SubscriptionModel>> GetActiveSubscriptions()
        {
            var subscriptions = await _subscriptionsRepositry.GetSubscriptions(true);
            return _mapper.Map<IEnumerable<SubscriptionModel>>(subscriptions);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<SubscriptionModel>> GetSubscriptions()
        {
            var subscriptions = await _subscriptionsRepositry.GetSubscriptions(null);
            return _mapper.Map<IEnumerable<SubscriptionModel>>(subscriptions);
        }

        /// <inheritdoc/>
        public async Task<SubscriptionModel> GetSubscription(Guid id)
        {
            var subscription = await _subscriptionsRepositry.GetSubscription(id);
            if (subscription == null)
                throw new NotFoundException();

            return _mapper.Map<SubscriptionModel>(subscription);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<SubscriptionModel>> GetSubscriptionsForEvent(string eventName)
        {
            var subscriptions = await _subscriptionsRepositry.GetSubscriptionsForEvent(eventName);
            return _mapper.Map<IEnumerable<SubscriptionModel>>(subscriptions);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<SubscriptionModel>> GetSubscriptionsForEvent(Type @event)
        {
            return await GetSubscriptionsForEvent(@event.FullName);
        }

        /// <inheritdoc/>
        public async Task RemoveSubscription(Guid id)
        {
            await _subscriptionsRepositry.RemoveSubscription(id);
            await _unitOfWork.Save();

            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"exchange:{typeof(DeactivateSubscription).FullName}"));
            await endpoint.Send(new DeactivateSubscription(id));

        }

    }
}
