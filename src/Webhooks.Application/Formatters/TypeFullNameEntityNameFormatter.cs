using MassTransit.Topology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webhooks.Application.Formatters
{
    public class TypeFullNameEntityNameFormatter : IEntityNameFormatter
    {
        public string FormatEntityName<T>()
        {
            return typeof(T).FullName;
        }
    }
}
