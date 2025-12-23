using SagaDemo.Contracts;
using SagaDemo.Infrastructure.Configuration;
using SagaDemo.Infrastructure.Saga;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

var host = builder.Build();

await host.Services.EnsureDatabaseCreatedAsync();

var rabbitOptions = builder.Configuration.GetRabbitMqOptions();

Console.WriteLine($"Starting {nameof(OrderSagaStateMachine)} listening at queue {QueueNames.OrderSaga}");

await host.RunAsync();
