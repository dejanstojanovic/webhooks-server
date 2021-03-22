using GreenPipes;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Webhooks.WorkerService.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using Webhooks.WorkerService.Consumers;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Webhooks.Data.Extensions;
using Webhooks.Domain.Models;
using Webhooks.Data.Repositories;
using Webhooks.Common.Helpers;
using Webhooks.Domain.Commands;
using Webhooks.Common.Options;
using Webhooks.WorkerService.Constants;

namespace Webhooks.WorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    var configuration = hostContext.Configuration;

                    // Add application options
                    services.AddDataRepositories(configuration);

                    // Add options
                    services.Configure<DeliveryOptions>(configuration.GetSection(nameof(DeliveryOptions)));
                    services.Configure<RabbitMqOptions>(configuration.GetSection(nameof(RabbitMqOptions)));

                    RabbitMqOptions rabbitmqOptions = null;
                    DeliveryOptions deliveryOptions = null;
                    IEnumerable<Subscription> subscriptions;

                    using (var provider = services.BuildServiceProvider())
                    {
                        rabbitmqOptions = provider.GetService<IOptions<RabbitMqOptions>>().Value;
                        deliveryOptions = provider.GetService<IOptions<DeliveryOptions>>().Value;
                        subscriptions = provider.GetService<ISubscriptonsRepository>().GetSubscriptions(true).Result;
                    }

                    #region Masstransit config
                    services.AddMassTransit(x =>
                    {
                        x.AddBus(busContext => Bus.Factory.CreateUsingRabbitMq(config =>
                        {
                            config.Host(rabbitmqOptions.Uri, h =>
                            {
                                h.Username(rabbitmqOptions.Username);
                                h.Password(rabbitmqOptions.Password);
                            });

                            #region Command consumers

                            x.AddConsumer<ActivateSubscriptionConsumer>();
                            config.ReceiveEndpoint(queueName: typeof(ActivateSubscription).FullName, c =>
                            {
                                c.ConfigureConsumeTopology = false;
                                c.ConfigureConsumer<ActivateSubscriptionConsumer>(busContext);
                            });

                            x.AddConsumer<DeactivateSubscriptionConsumer>();
                            config.ReceiveEndpoint(queueName: typeof(DeactivateSubscription).FullName, c =>
                            {
                                c.ConfigureConsumeTopology = false;
                                c.ConfigureConsumer<DeactivateSubscriptionConsumer>(busContext);
                            });

                            #endregion

                            #region Event consumers
                            foreach (var subscription in subscriptions)
                            {
                                var eventType = DomainEventsHelper.GetDomainEventTypes().SingleOrDefault(t => t.FullName == subscription.Event);
                                if (eventType == null)
                                    continue; // TODO: Log invalid event



                                #region Add event consumers
                                var addConsumerMethod = x.GetType()
                                                         .GetMethods().Single(m => m.Name == nameof(IServiceCollectionBusConfigurator.AddConsumer) &&
                                                            m.ContainsGenericParameters &&
                                                            m.GetParameters().Length == 1 &&
                                                            m.GetParameters()[0].ParameterType.IsGenericType &&
                                                            m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(Action<>)
                                                            )
                                                         .MakeGenericMethod(typeof(DomainEventConsumer<>).MakeGenericType(eventType));
                                addConsumerMethod.Invoke(x, new object[] { null });

                                //x.AddConsumer(eventType);
                                //x.AddConsumer<DomainEventConsumer<OperationCompletedEvent>>();

                                #endregion

                                #region Configure event consumers
                                config.ReceiveEndpoint(queueName: $"{subscription.Event}_{subscription.Id}", c =>
                                    {

                                        c.Bind(exchangeName: eventType.FullName);
                                        c.UseMessageRetry(r => r.Interval(deliveryOptions.Attempts, TimeSpan.FromSeconds(deliveryOptions.AttemptDelay)));
                                        c.ConfigureConsumeTopology = false;
                                        var configureConsumerMethod = typeof(RegistrationContextExtensions)
                                                        .GetMethods().Single(m => m.Name == nameof(RegistrationContextExtensions.ConfigureConsumer) &&
                                                            m.ContainsGenericParameters &&
                                                            m.GetParameters().Length == 3 &&
                                                            m.GetParameters()[0].ParameterType == typeof(IReceiveEndpointConfigurator) &&
                                                            m.GetParameters()[1].ParameterType == typeof(IRegistration) &&
                                                            m.GetParameters()[2].ParameterType.IsGenericType &&
                                                            m.GetParameters()[2].ParameterType.GetGenericTypeDefinition() == typeof(Action<>)
                                                            )
                                                        .MakeGenericMethod(typeof(DomainEventConsumer<>).MakeGenericType(eventType));

                                        configureConsumerMethod.Invoke(null, new object[] { c, busContext, null });

                                        //c.ConfigureConsumer(busContext, eventType);
                                        //c.ConfigureConsumer<DomainEventConsumer<OperationCompletedEvent>>(busContext);
                                    });
                                #endregion



                            }
                            #endregion
                        }));
                    });

                    services.AddScoped<ActivateSubscriptionConsumer>();
                    services.AddScoped<DeactivateSubscriptionConsumer>();

                    foreach (var @event in DomainEventsHelper.GetDomainEventTypes())
                        services.AddScoped(typeof(DomainEventConsumer<>).MakeGenericType(@event));
                    #endregion

                    #region HttpClientFactory config
                    services.AddHttpClient(HttpClientNames.WebhookSubscriptionHttpClient, c =>
                    {
                        if (deliveryOptions.Timeout > 0)
                            c.Timeout = TimeSpan.FromSeconds(deliveryOptions.Timeout);
                    });
                    #endregion

                    services.AddHostedService<Worker>();
                });
    }
}
