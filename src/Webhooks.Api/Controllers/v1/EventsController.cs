using Microsoft.AspNetCore.Mvc;
using Webhooks.Application.Models;
using Webhooks.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using Webhooks.Api.Models;
using Webhooks.Application.Extensions;

namespace Webhooks.Api.Controllers
{
    /// <summary>
    /// Events endpoints
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("[controller]")]
    [Route("v{version:apiVersion}/[controller]")]
    [SwaggerResponse((int)HttpStatusCode.Accepted, Type = null, Description = "Payload accepted for processing")]
    [SwaggerResponse((int)HttpStatusCode.NotFound, Type = null, Description = "Resource not found")]
    [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(IEnumerable<ValidationModel>), Description = "Validation failed")]
    [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ErrorModel), Description = "Application exception")]
    public class EventsController : ControllerBase
    {
        readonly IEventsService _eventsService;
        readonly ISubscriptionsService _subscriptionsService;
        public EventsController(
            IEventsService eventsService,
            ISubscriptionsService subscriptionsService)
        {
            _eventsService = eventsService;
            _subscriptionsService = subscriptionsService;
        }

        /// <summary>
        /// Retrieves list of event type names
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<String>>> GetEventNames()
        {
            var events = await _eventsService.GetEvents();
            if (events == null || !events.Any())
                return NotFound();

            return Ok(events.Select(t => t.FullName).ToArray());
        }

        /// <summary>
        /// Retrieves subscriptions for specific event
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        [HttpGet("{event}/subscriptions")]
        public async Task<ActionResult<IEnumerable<SubscriptionModel>>> GetEventSubscriptions(string @event)
        {
            var subscriptions = await _subscriptionsService.GetSubscriptionsForEvent(@event);
            if (subscriptions == null || !subscriptions.Any())
                return NotFound();

            return Ok(subscriptions);
        }

        /// <summary>
        /// Publish event
        /// </summary>
        /// <param name="event">Event type name</param>
        /// <param name="eventModel">Event body</param>
        /// <returns></returns>
        [HttpPost("{event}")]
        public async Task<ActionResult> PublishEvent(string @event, Object eventModel)
        {
            var type = await _eventsService.GetEventType(@event);
            if (type == null)
                return NotFound();

            await _eventsService.Publish(type, eventModel);
            return Accepted();
        }

    }
}
