using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webhooks.WorkerService.Options
{
    /// <summary>
    /// Event payload delivery options
    /// </summary>
    public class DeliveryOptions
    {
        /// <summary>
        /// Number of retry attempts for failed delivery requests
        /// </summary>
        public int Attempts { get; set; }

        /// <summary>
        /// Delay in seconds between each retry attempt
        /// </summary>
        public int AttemptDelay { get; set; }

        /// <summary>
        /// HTTP request timeout in seconds
        /// </summary>
        public int Timeout { get; set; }


    }
}
