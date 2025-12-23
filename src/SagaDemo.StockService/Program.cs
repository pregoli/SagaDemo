using MassTransit;
using SagaDemo.Contracts;
using SagaDemo.Infrastructure.Configuration;
using SagaDemo.StockService.Cinsumers;

var builder = Host.CreateApplicationBuilder(args);

var rabbitOptions = builder.Configuration.GetRabbitMqOptions();

builder.Services.AddMassTransit(cfg =>
{
    cfg.AddConsumer<ReserveStockConsumer>();
    cfg.AddConsumer<ReleaseStockConsumer>();

    cfg.UsingRabbitMq((context, rabbit) =>
    {
        rabbit.ConfigureRabbitMqHost(rabbitOptions);
        rabbit.UseMessageRetry(r => r.Intervals(100, 200, 500, 1000));

        rabbit.ReceiveEndpoint(QueueNames.ReserveStock, e =>
        {
            e.ConfigureConsumer<ReserveStockConsumer>(context);
        });

        rabbit.ReceiveEndpoint(QueueNames.ReleaseStock, e =>
        {
            e.ConfigureConsumer<ReleaseStockConsumer>(context);
        });
    });
});

var host = builder.Build();

Console.WriteLine($"Starting {nameof(ReserveStockConsumer)} listening at queue {QueueNames.ReserveStock}");
Console.WriteLine($"Starting {nameof(ReleaseStockConsumer)} listening at queue {QueueNames.ReleaseStock}");

await host.RunAsync();
