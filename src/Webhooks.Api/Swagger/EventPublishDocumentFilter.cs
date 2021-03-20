using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Webhooks.Application.Services;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Webhooks.Api.Swagger
{
    public class EventPublishDocumentFilter : IDocumentFilter
    {
        readonly IServiceScopeFactory _serviceScopeFactory;
        readonly IEnumerable<Type> _events;
        public EventPublishDocumentFilter(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var eventsService = scope.ServiceProvider.GetRequiredService<IEventsService>();
                _events = eventsService.GetEvents().Result;
            }
        }
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {

            OpenApiMediaType getOpenApiMediaType(Type type)
            {
                return new OpenApiMediaType()
                {
                    Example = new OpenApiString(JsonConvert.SerializeObject(Activator.CreateInstance(type))),
                    Schema = new OpenApiSchema
                    {
                        Type = "String"
                    }
                };
            }

            foreach (var @event in _events)
            {
                //context.SchemaRepository.RegisterType(@event, @event.FullName);
                swaggerDoc.Components.RequestBodies.Add(@event.FullName, new OpenApiRequestBody
                {
                    Content = new Dictionary<string, OpenApiMediaType>()
                        {
                            { "application/json-patch+json", getOpenApiMediaType(@event)},
                            { "application/json", getOpenApiMediaType(@event)},
                            { "text/json", getOpenApiMediaType(@event)},
                            { "application/*+json", getOpenApiMediaType(@event)}
                        }
                });
            }

            var apiDescriptions = context.ApiDescriptions.Where(d => d.HttpMethod.Equals("POST", StringComparison.InvariantCultureIgnoreCase) &&
                                                                     d.RelativePath.Equals("events/{event}", StringComparison.InvariantCultureIgnoreCase) &&
                                                                     d.ParameterDescriptions.Any(p => p.Name.Equals("eventModel", StringComparison.InvariantCultureIgnoreCase) &&
                                                                                                      p.Source.Id.Equals("Body", StringComparison.InvariantCultureIgnoreCase))
                                                                     );

            foreach (var apiDescription in apiDescriptions)
            {
                foreach (var @event in _events)
                {
                    swaggerDoc.Paths.Add($"/{apiDescription.GroupName}/events/{@event.FullName}", new OpenApiPathItem()
                    {
                        Operations = new Dictionary<OperationType, OpenApiOperation>()
                        {
                            {
                                OperationType.Post, new OpenApiOperation()
                                {
                                    Tags = new List<OpenApiTag>(){
                                        new OpenApiTag() {Name="Events publishing" }
                                    },
                                    RequestBody = new OpenApiRequestBody()
                                    {
                                        Reference =  new OpenApiReference()
                                        {
                                            Type = ReferenceType.RequestBody,
                                            Id = $"components/requestBodies/{@event.FullName}",
                                            ExternalResource = ""
                                        }
                                    },
                                    Summary = $"Publishes {@event.FullName} event"
                                }
                            }
                        }
                    });
                }

                swaggerDoc.Paths.Remove($"/{apiDescription.GroupName}/{apiDescription.RelativePath}");
            }
        }

    }
}

