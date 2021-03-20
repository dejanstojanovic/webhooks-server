using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webhooks.Api.Models
{
    public class ValidationModel
    {
        public ValidationModel()
        {

        }

        public ValidationModel(string field, string message)
        {
            this.Field = field;
            this.Message = message;
        }
        public String Field { get; set; }
        public String Message { get; set; }
    }
}
