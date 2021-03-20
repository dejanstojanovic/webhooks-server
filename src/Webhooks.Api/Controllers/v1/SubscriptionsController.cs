using Microsoft.AspNetCore.Mvc;
using Webhooks.Application.Models;
using Webhooks.Application.Services;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Net;
using System.Threading.Tasks;
using Webhooks.Api.Models;
using System.Collections.Generic;
using System.Linq;

namespace Webhooks.Api.Controllers
{
    /// <summary>
    /// Subscriptions endpoints
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("[controller]")]
    [Route("v{version:apiVersion}/[controller]")]
    [SwaggerResponse((int)HttpStatusCode.Created, Type = null, Description = "Resource successfully created")]
    [SwaggerResponse((int)HttpStatusCode.NotFound, Type = null, Description = "Resource not found")]
    [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(IEnumerable<ValidationModel>), Description = "Validation failed")]
    [SwaggerResponse((int)HttpStatusCode.Conflict, Type = null, Description = "Resource already exists")]
    [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ErrorModel), Description = "Application exception")]
    public class SubscriptionsController : ControllerBase
    {
        readonly ISubscriptionsService _subscriptionsService;

        public SubscriptionsController(ISubscriptionsService subscriptionsService)
        {
            _subscriptionsService = subscriptionsService;
        }

        /// <summary>
        /// Retrieves specific subscription with <paramref name="id"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<SubscriptionModel>> GetSubscription(Guid id)
        {
            var subscription = await _subscriptionsService.GetSubscription(id);
            if (subscription == null)
                return NotFound();

            return Ok(subscription);
        }

        /// <summary>
        /// Registers new event subscription
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddSubscription(SubscriptionAddModel model)
        {
            await _subscriptionsService.AddSubscription(model);
            return CreatedAtAction(nameof(GetSubscription), new { id = model.Id }, null);
        }

        /// <summary>
        /// Removes an existing subscription
        /// </summary>
        /// <param name="id">Subscription unique identifier</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSubscription(Guid id)
        {
            await _subscriptionsService.RemoveSubscription(id);
            return NoContent();
        }

        /// <summary>
        /// Retrieves all subscriptions
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubscriptionModel>>> GetSubscriptions()
        {
            var subscriptions = await _subscriptionsService.GetSubscriptions();
            if (subscriptions == null || !subscriptions.Any())
                return NotFound();

            return Ok(subscriptions);
        }
    }
}
