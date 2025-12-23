using MassTransit;
using SagaDemo.Contracts.Commands;
using SagaDemo.Contracts.Events;

namespace SagaDemo.StockService.Cinsumers;

public class ReserveStockConsumer(ILogger<ReserveStockConsumer> logger) : IConsumer<ReserveStock>
{
    private static readonly Random Random = new();

    public async Task Consume(ConsumeContext<ReserveStock> context)
    {
        var msg = context.Message;
        logger.LogInformation("Reserving {Qty}x {Product} for {OrderId}", msg.Quantity, msg.ProductName, msg.OrderId);

        await Task.Delay(500);

        if (Random.Next(100) < 10)
        {
            logger.LogWarning("Insufficient stock for {Product}", msg.ProductName);
            await context.Publish(new StockReservationFailed
            {
                OrderId = msg.OrderId,
                CorrelationId = msg.CorrelationId,
                ProductName = msg.ProductName,
                Quantity = msg.Quantity,
                Reason = $"Insufficient stock for {msg.ProductName}"
            });
            return;
        }

        logger.LogInformation("Reserved stock for {Product}", msg.ProductName);
        await context.Publish(new StockReserved
        {
            OrderId = msg.OrderId,
            CorrelationId = msg.CorrelationId,
            ProductName = msg.ProductName,
            Quantity = msg.Quantity
        });
    }
}
