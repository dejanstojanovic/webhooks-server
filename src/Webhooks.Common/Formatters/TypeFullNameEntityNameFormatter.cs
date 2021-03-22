using MassTransit.Topology;

namespace Webhooks.Common.Formatters
{
    public class TypeFullNameEntityNameFormatter : IEntityNameFormatter
    {
        public string FormatEntityName<T>()
        {
            return typeof(T).FullName;
        }
    }
}
