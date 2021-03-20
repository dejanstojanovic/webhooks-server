using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Webhooks.Application.Models
{
    public class SubscriptionModel
    {
        public Guid Id { get; set; }
        public Uri Endpoint { get; set; }

        [JsonIgnore]
        public Type Event { get; set; }

        public String EventName { get; set; }
        public bool Active { get; set; }
        public IDictionary<String, String> Headers { get; set; }
    }
}
