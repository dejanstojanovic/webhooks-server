using Core.Domain.Events;
using MassTransit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Webhooks.Common.Helpers;

namespace Webhooks.Application.Services
{
    /// <inheritdoc/>
    public class EventsService : IEventsService
    {
        readonly IBus _bus;
        public EventsService(IBus bus)
        {
            _bus = bus;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Type>> GetEvents()
        {
            return await Task.FromResult(DomainEventsHelper.GetDomainEventTypes());
        }

        /// <inheritdoc/>
        public async Task<Type> GetEventType(string name)
        {
            var @event = DomainEventsHelper.GetDomainEventTypes().SingleOrDefault(e => e.FullName.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            return await Task.FromResult(@event);
        }

        /// <inheritdoc/>
        public async Task Publish<T>(T @event) where T : class, IDomainEvent
        {
            var context = new ValidationContext(@event, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(@event, context, validationResults, true);
            if (!isValid)
                throw new ValidationException(validationResult: validationResults.First(), null, null);

            await _bus.Publish<T>(@event);
        }

    }
}
