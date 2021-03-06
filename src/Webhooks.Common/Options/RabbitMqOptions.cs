using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webhooks.Common.Options
{
    public class RabbitMqOptions
    {
        public String Host { get; set; }
        public String VirtualHost { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }
        public int ConcurrentMessages { get; set; }
        public Uri Uri
        {
            get
            {
                return new Uri($"rabbitmq://{this.Host}/{this.VirtualHost}");
            }
        }
    }
}
