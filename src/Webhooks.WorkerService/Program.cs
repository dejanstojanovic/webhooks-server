using GreenPipes;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Webhooks.Application.Extensions;
using Webhooks.Application.Models;
using Webhooks.Application.Services;
using Webhooks.WorkerService.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using Core.Domain.Events;
using MassTransit.ConsumeConfigurators;
using Core.Domain.Events.Samples;
using Webhooks.WorkerService.Consumers;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.ExtensionsDependencyInjectionIntegration.Registration;
using Webhooks.Common.Extensions;

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
                    services.AddApplicationServices(configuration);

                    // Add options
                    services.Configure<DeliveryOptions>(configuration.GetSection(nameof(DeliveryOptions)));
                    services.Configure<RabbitMqOptions>(configuration.GetSection(nameof(RabbitMqOptions)));

                    RabbitMqOptions rabbitmqOptions = null;
                    DeliveryOptions deliveryOptions = null;
                    IEnumerable<SubscriptionModel> subscriptions;

                    using (var provider = services.BuildServiceProvider())
                    {
                        rabbitmqOptions = provider.GetService<IOptions<RabbitMqOptions>>().Value;
                        deliveryOptions = provider.GetService<IOptions<DeliveryOptions>>().Value;
                        subscriptions = provider.GetService<ISubscriptionsService>().GetActiveSubscriptions().Result;
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

                            foreach (var subscription in subscriptions)
                            {

                                #region Add consumer
                                var addConsumerMethod = x.GetType()
                                                         .GetMethods().Single(m => m.Name == nameof(IServiceCollectionBusConfigurator.AddConsumer) &&
                                                            m.ContainsGenericParameters &&
                                                            m.GetParameters().Length == 1 &&
                                                            m.GetParameters()[0].ParameterType.IsGenericType &&
                                                            m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(Action<>)
                                                            )
                                                         .MakeGenericMethod(typeof(DomainEventConsumer<>).MakeGenericType(subscription.Event));

                                var addConsumerMethodResult = addConsumerMethod.Invoke(x, new object[] { null });
                                //x.AddConsumer<DomainEventConsumer<OperationCompletedEvent>>();

                                #endregion

                                config.ReceiveEndpoint(queueName: $"{subscription.Id}_{subscription.EventName}", c =>
                                    {
                                        if (hostContext.HostingEnvironment.IsLocal())
                                            c.AutoDelete = true; // Delete when disconnected on local

                                        c.UseMessageRetry(r => r.Interval(deliveryOptions.Attempts, TimeSpan.FromSeconds(deliveryOptions.AttemptDelay)));

                                        #region Configure consumer 
                                        //TODO: Do this better
                                        var configureConsumerMethod = typeof(RegistrationContextExtensions)
                                                        .GetMethods().Single(m => m.Name == nameof(RegistrationContextExtensions.ConfigureConsumer) &&
                                                            m.ContainsGenericParameters &&
                                                            m.GetParameters().Length == 3 &&
                                                            m.GetParameters()[0].ParameterType == typeof(IReceiveEndpointConfigurator) &&
                                                            m.GetParameters()[1].ParameterType == typeof(IRegistration) &&
                                                            m.GetParameters()[2].ParameterType.IsGenericType &&
                                                            m.GetParameters()[2].ParameterType.GetGenericTypeDefinition() == typeof(Action<>))
                                                        .MakeGenericMethod(typeof(DomainEventConsumer<>).MakeGenericType(subscription.Event));

                                        var configureConsumerMethodResult = configureConsumerMethod.Invoke(null, new object[] { c, busContext, null });
                                        //c.ConfigureConsumer<DomainEventConsumer<OperationCompletedEvent>>(busContext);

                                        #endregion
                                    });


                            }
                        }));
                    });
                    #endregion



                    services.AddHostedService<Worker>();
                });
    }
}
