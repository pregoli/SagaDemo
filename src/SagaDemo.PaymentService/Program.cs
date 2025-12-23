using MassTransit;
using SagaDemo.Contracts;
using SagaDemo.Infrastructure.Configuration;
using SagaDemo.PaymentService.Consumers;

var builder = Host.CreateApplicationBuilder(args);

var rabbitOptions = builder.Configuration.GetRabbitMqOptions();

builder.Services.AddMassTransit(cfg =>
{
    cfg.AddConsumer<ProcessPaymentConsumer>();

    cfg.UsingRabbitMq((context, rabbit) =>
    {
        rabbit.ConfigureRabbitMqHost(rabbitOptions);
        rabbit.UseMessageRetry(r => r.Intervals(100, 200, 500, 1000));

        rabbit.ReceiveEndpoint(QueueNames.ProcessPayment, e =>
        {
            e.ConfigureConsumer<ProcessPaymentConsumer>(context);
        });
    });
});

var host = builder.Build();

Console.WriteLine($"Starting {nameof(ProcessPaymentConsumer)} listening at queue {QueueNames.ProcessPayment}");

await host.RunAsync();
