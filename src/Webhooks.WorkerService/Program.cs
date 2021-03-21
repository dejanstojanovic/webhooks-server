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
using Webhooks.Common.Extensions;
using Webhooks.Data.Extensions;
using Webhooks.Domain.Models;
using Webhooks.Data.Repositories;
using Webhooks.Common.Helpers;
using Core.Domain.Events.Samples;
using Webhooks.Domain.Commands;

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
                            config.Host(new Uri($"rabbitmq://{rabbitmqOptions.Host}/{rabbitmqOptions.VirtualHost}"), h =>
                            {
                                h.Username(rabbitmqOptions.Username);
                                h.Password(rabbitmqOptions.Password);
                            });

                            config.UseDelayedExchangeMessageScheduler();

                            #region Command consumers
                            x.AddConsumer<ActivateSubscriptionConsumer, ActivateSubscriptionConsumerDefinition>();
                            //x.AddConsumer<DeactivateSubscriptionConsumer, DeactivateSubscriptionConsumerDefinition>();

                            //config.ReceiveEndpoint(queueName: typeof(ActivateSubscription).FullName, c =>
                            //{
                            //    c.Bind(exchangeName: typeof(ActivateSubscription).FullName);
                            //    c.ConfigureConsumeTopology = false;
                            //    c.ConfigureConsumer<ActivateSubscriptionConsumer>(busContext);
                            //});

                            //config.ReceiveEndpoint(queueName: typeof(DeactivateSubscriptionConsumer).FullName, c =>
                            //{
                            //    //c.Bind(exchangeName: typeof(DeactivateSubscriptionConsumer).GetEndpointName());
                            //    c.ConfigureConsumer<DeactivateSubscriptionConsumer>(busContext);
                            //});
                            #endregion


                            foreach (var subscription in subscriptions)
                            {
                                var eventType = DomainEventsHelper.GetDomainEventTypes().SingleOrDefault(t => t.FullName == subscription.Event);
                                if (eventType == null)
                                    continue; // TODO: Log invalid event

                                #region Event consumers

                                #region Add event consumers
                                //var addConsumerMethod = x.GetType()
                                //                         .GetMethods().Single(m => m.Name == nameof(IServiceCollectionBusConfigurator.AddConsumer) &&
                                //                            m.ContainsGenericParameters &&
                                //                            m.GetParameters().Length == 1 &&
                                //                            m.GetParameters()[0].ParameterType.IsGenericType &&
                                //                            m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(Action<>)
                                //                            )
                                //                         .MakeGenericMethod(typeof(DomainEventConsumer<>).MakeGenericType(eventType));
                                //addConsumerMethod.Invoke(x, new object[] { null });
                                //x.AddConsumer<DomainEventConsumer<OperationCompletedEvent>>();


                                //var addConsumerMethod = x.GetType()
                                //     .GetMethods().Single(m => m.Name == nameof(IServiceCollectionBusConfigurator.AddConsumer) &&
                                //        m.ContainsGenericParameters &&
                                //        m.GetParameters().Length == 2 &&
                                //        !m.GetParameters()[0].ParameterType.IsGenericType &&
                                //        m.GetParameters()[1].ParameterType.IsGenericType &&
                                //        m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Action<>)
                                //        )
                                //     .MakeGenericMethod(typeof(DomainEventConsumer<>).MakeGenericType(eventType), typeof(DomainEventConsumerDefinition<>).MakeGenericType());


                                //x.AddConsumer<DomainEventConsumer<OperationCompletedEvent>, DomainEventConsumerDefinition<OperationCompletedEvent>>();

                                #endregion

                                #region Configure event consumers
                                //config.ReceiveEndpoint(queueName: $"{subscription.Event}_{subscription.Id}", c =>
                                //    {
                                //        //c.Bind(eventType.GetEndpointName());
                                //        c.UseMessageRetry(r => r.Interval(deliveryOptions.Attempts, TimeSpan.FromSeconds(deliveryOptions.AttemptDelay)));

                                //        var configureConsumerMethod = typeof(RegistrationContextExtensions)
                                //                        .GetMethods().Single(m => m.Name == nameof(RegistrationContextExtensions.ConfigureConsumer) &&
                                //                            m.ContainsGenericParameters &&
                                //                            m.GetParameters().Length == 3 &&
                                //                            m.GetParameters()[0].ParameterType == typeof(IReceiveEndpointConfigurator) &&
                                //                            m.GetParameters()[1].ParameterType == typeof(IRegistration) &&
                                //                            m.GetParameters()[2].ParameterType.IsGenericType &&
                                //                            m.GetParameters()[2].ParameterType.GetGenericTypeDefinition() == typeof(Action<>)
                                //                            )
                                //                        .MakeGenericMethod(typeof(DomainEventConsumer<>).MakeGenericType(eventType));

                                //        //configureConsumerMethod.Invoke(null, new object[] { c, busContext, null });
                                //        //c.ConfigureConsumer<DomainEventConsumer<OperationCompletedEvent>>(busContext);
                                //    });
                                #endregion

                                #endregion
                            }

                            config.ConfigureEndpoints(busContext);

                        }));
                    });
                    #endregion

                    services.AddHostedService<Worker>();
                });
    }
}
