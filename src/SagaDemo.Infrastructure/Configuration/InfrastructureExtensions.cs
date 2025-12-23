using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SagaDemo.Contracts;
using SagaDemo.Infrastructure.Persistence;
using SagaDemo.Infrastructure.Saga;

namespace SagaDemo.Infrastructure.Configuration;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        
        var rabbitOptions = configuration.GetRabbitMqOptions();

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddMassTransit(cfg =>
        {
            cfg.AddSagaStateMachine<OrderSagaStateMachine, OrderSagaState>()
                .EntityFrameworkRepository(r =>
                {
                    r.ConcurrencyMode = ConcurrencyMode.Optimistic;
                    r.ExistingDbContext<AppDbContext>();
                    r.UseSqlServer();
                });

            cfg.AddEntityFrameworkOutbox<AppDbContext>(o =>
            {
                o.UseSqlServer();
                o.UseBusOutbox();
            });

            cfg.UsingRabbitMq((context, rabbit) =>
            {
                rabbit.ConfigureRabbitMqHost(rabbitOptions);
                rabbit.UseMessageRetry(r => r.Intervals(100, 200, 500, 1000));

                rabbit.ReceiveEndpoint(QueueNames.OrderSaga, e =>
                {
                    e.UseEntityFrameworkOutbox<AppDbContext>(context);
                    e.ConfigureSaga<OrderSagaState>(context);
                });
            });
        });

        return services;
    }

    public static async Task EnsureDatabaseCreatedAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        const int maxRetries = 3;
        var delay = TimeSpan.FromSeconds(3);
        
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                await context.Database.EnsureCreatedAsync();
                Console.WriteLine("Database initialized successfully");
                return;
            }
            catch (Exception ex) when (i < maxRetries - 1)
            {
                Console.WriteLine($"Database initialization attempt {i + 1}/{maxRetries} failed: {ex.Message}");
                Console.WriteLine($"   Retrying in {delay.TotalSeconds} seconds...");
                await Task.Delay(delay);
            }
        }
    }
}
