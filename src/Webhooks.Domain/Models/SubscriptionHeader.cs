using System;

namespace Webhooks.Domain.Models
{
    /// <summary>
    /// Subscription header key-value pair entity
    /// </summary>
    public class SubscriptionHeader
    {
        /// <summary>
        /// Subscription id to which header key-value belongs to
        /// </summary>
        public Guid SubscriptionId { get; set; }

        /// <summary>
        /// Header key
        /// </summary>
        public String Key { get; set; }

        /// <summary>
        /// Header value
        /// </summary>
        public String Value { get; set; }

        /// <summary>
        /// Subscription entity header entry belongs to
        /// </summary>
        public virtual Subscription Subscription { get; set; }
    }
}
