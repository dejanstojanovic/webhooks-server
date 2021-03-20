using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Webhooks.Application.Models
{
    public class SubscriptionAddModel
    {
        public Guid Id { get; set; }
        public String Event { get; set; }
        public Uri Endpoint { get; set; }
        public bool Active { get; set; }
        public IDictionary<String, String> Headers { get; set; }
    }
}
