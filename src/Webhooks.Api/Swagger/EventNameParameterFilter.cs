using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Core.Domain.Events;
using Webhooks.Application.Services;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Webhooks.Api.Swagger
{

    public class EventNameParameterFilter : IParameterFilter
    {
        readonly IServiceScopeFactory _serviceScopeFactory;

        public EventNameParameterFilter(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
        {
            if (parameter.Name.Equals("event", StringComparison.InvariantCultureIgnoreCase))
            {

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var eventsService = scope.ServiceProvider.GetRequiredService<IEventsService>();
                    IEnumerable<String> events = eventsService.GetEvents().Result.Select(t => t.FullName);

                    parameter.Schema.Enum = events.Select(e => new OpenApiString(e)).ToList<IOpenApiAny>();

                }
            }
        }
    }



}
