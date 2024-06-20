using System.Reflection;
using GreenPipes;
using MassTransit;
using MassTransit.Definition;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Play.Common.Settings;

namespace Play.Common.RabbitMQ;

public static class Extensions
{
    public static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services)
    {
        services.AddMassTransit(configure =>
        {
            configure.AddConsumers(Assembly.GetEntryAssembly());

            configure.UsingRabbitMq((context, configurator) =>
             {
                 var configuration = context.GetService<IConfiguration>();
                 var serviceSettings = configuration.GetSection(nameof(ServiceSettings))
                                        .Get<ServiceSettings>();
                 var rabbitMQSettings = configuration.GetSection(nameof(RabbitMQSettings))
                                        .Get<RabbitMQSettings>();
                 configurator.Host(rabbitMQSettings!.Host);
                 var endpointNameFormatter = new KebabCaseEndpointNameFormatter(serviceSettings!.ServiceName, false);
                 configurator.ConfigureEndpoints(context, endpointNameFormatter);
                 configurator.UseMessageRetry(retryConfigurator =>
                 {
                     retryConfigurator.Interval(3, TimeSpan.FromSeconds(5));
                 });
             });
        });

        //Register a background service with the .NET Core application. 
        //This service was responsible for managing the lifecycle of the MassTransit message bus.
        //It ensured the bus was started when the application started and stopped when the application stopped.
        services.AddMassTransitHostedService();

        return services;
    }
}