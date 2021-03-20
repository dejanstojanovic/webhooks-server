using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webhooks.Api.Models
{
    public class ErrorModel
    {
        public String Message { get; set; }
        public String StackTrace { get; set; }
    }
}
