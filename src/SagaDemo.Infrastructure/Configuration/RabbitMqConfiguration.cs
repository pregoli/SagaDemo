using MassTransit;
using Microsoft.Extensions.Configuration;

namespace SagaDemo.Infrastructure.Configuration;

public static class RabbitMqConfiguration
{
    public static RabbitMqOptions GetRabbitMqOptions(this IConfiguration configuration)
    {
        return configuration.GetSection(RabbitMqOptions.SectionName).Get<RabbitMqOptions>() 
               ?? new RabbitMqOptions();
    }

    public static void ConfigureRabbitMqHost(
        this IRabbitMqBusFactoryConfigurator rabbit, 
        RabbitMqOptions options)
    {
        rabbit.Host(options.Host, "/", h =>
        {
            h.Username(options.Username);
            h.Password(options.Password);
        });
    }
}
