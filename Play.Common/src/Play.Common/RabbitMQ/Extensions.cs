using System.Reflection;
using GreenPipes;
using GreenPipes.Configurators;
using MassTransit;
using MassTransit.Definition;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Play.Common.Settings;

namespace Play.Common.RabbitMQ;

public static class Extensions
{
    public static IServiceCollection AddMassTransitWithRabbitMq(
        this IServiceCollection services,
        Action<IRetryConfigurator>? configureRetries = null)
    {
        services.AddMassTransit(configure =>
        {
            configure.AddConsumers(Assembly.GetEntryAssembly());
            configure.ConfigureRabbitMq(configureRetries);
        });

        //Register a background service with the .NET Core application. 
        //This service was responsible for managing the lifecycle of the MassTransit message bus.
        //It ensured the bus was started when the application started and stopped when the application stopped.
        services.AddMassTransitHostedService();

        return services;
    }

    public static void ConfigureRabbitMq(this IServiceCollectionBusConfigurator configure,
        Action<IRetryConfigurator>? configureRetries = null)
    {
        configure.UsingRabbitMq((context, configurator) =>
        {
            // Retrieve the configuration settings for the service and RabbitMQ
            var configuration = context.GetService<IConfiguration>();
            var serviceSettings = configuration.GetSection(nameof(ServiceSettings))
                                   .Get<ServiceSettings>();
            var rabbitMQSettings = configuration.GetSection(nameof(RabbitMQSettings))
                                   .Get<RabbitMQSettings>();

            // Set up RabbitMQ host and configure endpoints based on service name
            configurator.Host(rabbitMQSettings!.Host);
            //The KebabCaseEndpointNameFormatter is a class in the MassTransit library,
            //which is used to format endpoint names in a "kebab-case" style.
            var endpointNameFormatter = new KebabCaseEndpointNameFormatter(serviceSettings!.ServiceName, false);
            configurator.ConfigureEndpoints(context, endpointNameFormatter);

            // Configure message retry policy: 3 retries with 5 seconds delay
            configureRetries ??= (retryConfigurator) => retryConfigurator.Interval(3, TimeSpan.FromSeconds(5));

            configurator.UseMessageRetry(configureRetries);
        });
    }
}