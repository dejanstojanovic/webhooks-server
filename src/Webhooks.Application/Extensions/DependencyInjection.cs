using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Webhooks.Application.Options;
using Webhooks.Application.Services;
using Webhooks.Data.Extensions;
using MassTransit;
using System;
using Core.Domain.Events;

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
                x.AddBus(busContext => Bus.Factory.CreateUsingRabbitMq(config =>
                {
                    // NOTE: Generates uri with rabbitmqs instead rabbitmq
                    //config.Host(host: rabbitmqOptions.Host, port: rabbitmqOptions.Port, virtualHost: rabbitmqOptions.VirtualHost, h =>

                    config.Host(new Uri($"rabbitmq://{rabbitmqOptions.Host}/{rabbitmqOptions.VirtualHost}"), h =>
                    {
                        h.Username(rabbitmqOptions.Username);
                        h.Password(rabbitmqOptions.Password);
                    });

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
