using MassTransit;
using SagaDemo.Contracts;
using SagaDemo.Infrastructure.Configuration;
using SagaDemo.ShippingService.Consumers;

var builder = Host.CreateApplicationBuilder(args);

var rabbitOptions = builder.Configuration.GetRabbitMqOptions();

builder.Services.AddMassTransit(cfg =>
{
    cfg.AddConsumer<ArrangeShippingConsumer>();

    cfg.UsingRabbitMq((context, rabbit) =>
    {
        rabbit.ConfigureRabbitMqHost(rabbitOptions);
        rabbit.UseMessageRetry(r => r.Intervals(100, 200, 500, 1000));

        rabbit.ReceiveEndpoint(QueueNames.ArrangeShipping, e =>
        {
            e.ConfigureConsumer<ArrangeShippingConsumer>(context);
        });
    });
});

var host = builder.Build();

Console.WriteLine($"Starting {nameof(ArrangeShippingConsumer)} listening at queue {QueueNames.ArrangeShipping}");

await host.RunAsync();
