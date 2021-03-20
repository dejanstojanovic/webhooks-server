using System;
using System.Collections.Generic;

namespace Webhooks.Domain.Models
{
    /// <summary>
    /// Subscription domain model
    /// </summary>
    public class Subscription
    {
        /// <summary>
        /// Uniques subcription identifier
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Subscription endpoint url to whic event payload will be sent to
        /// </summary>
        public Uri Endpoint { get; set; }

        /// <summary>
        /// Marsk subscription as active and ready for receiving events
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Name of the domain event being subscribed to
        /// </summary>
        public String Event { get; set; }

        /// <summary>
        /// Collection of headers to be set in HTTP request
        /// </summary>
        public virtual IEnumerable<SubscriptionHeader> Headers {get;set; }
    }
}
