using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Webhooks.Application.Services;
using Webhooks.Data.Extensions;
using MassTransit;
using System;
using Core.Domain.Events;
using Webhooks.Domain.Commands;
using Webhooks.Common.Helpers;
using MassTransit.Topology;
using Webhooks.Common.Options;
using Webhooks.Common.Extensions;
using Webhooks.Common.Formatters;

namespace Webhooks.Application.Extensions
{
    public static class DependencyInjection
    {
        public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDataRepositories(configuration);
            services.AddAutoMapper(typeof(DependencyInjection).Assembly);

            // Add application services
            services.Scan(scan => scan
            .FromAssembliesOf(typeof(ISubscriptionsService))
            .AddClasses(classes => classes.InNamespaces(typeof(ISubscriptionsService).Namespace))
            .AsImplementedInterfaces()
            .WithScopedLifetime()
           );

            #region Masstransit
            var rabbitmqOptions = new RabbitMqOptions();
            configuration.Bind(nameof(RabbitMqOptions), rabbitmqOptions);

            services.AddMassTransit(x =>
            {
                x.AddRequestClient<ActivateSubscription>(typeof(ActivateSubscription).GetQueueAddress());
                x.AddRequestClient<DeactivateSubscription>(typeof(DeactivateSubscription).GetQueueAddress());


                x.AddBus(busContext => Bus.Factory.CreateUsingRabbitMq(config =>
                {
                    config.Host(rabbitmqOptions.Uri, h =>
                    {
                        h.Username(rabbitmqOptions.Username);
                        h.Password(rabbitmqOptions.Password);
                    });

                    foreach (var @event in DomainEventsHelper.GetDomainEventTypes())
                    {
                        config.MessageTopology.SetEntityNameFormatter(new TypeFullNameEntityNameFormatter());
                    }

                    config.Publish<IDomainEvent>(c =>
                    {
                        c.Exclude = true;
                    });

                }));
            });

            #endregion

        }

    }
}
