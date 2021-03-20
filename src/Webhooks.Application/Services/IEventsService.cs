using Core.Domain.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Webhooks.Application.Services
{
    /// <summary>
    /// Event related methods
    /// </summary>
    public interface IEventsService
    {
        /// <summary>
        /// Retrieves all existing domain event
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Type>> GetEvents();

        /// <summary>
        /// Retrieves event of a specific name
        /// </summary>
        /// <param name="name">Event name</param>
        /// <returns></returns>
        Task<Type> GetEventType(string name);

        /// <summary>
        /// Publishes even instance to all subscribers
        /// </summary>
        /// <typeparam name="T">Event type</typeparam>
        /// <param name="event">Event instance</param>
        /// <returns></returns>
        Task Publish<T>(T @event) where T : class, IDomainEvent;
    }
}
