using MassTransit;
using SagaDemo.Contracts.Commands;
using SagaDemo.Contracts.Events;

namespace SagaDemo.ShippingService.Consumers;

public class ArrangeShippingConsumer(ILogger<ArrangeShippingConsumer> logger) : IConsumer<ArrangeShipping>
{
    private static readonly Random Random = new();

    public async Task Consume(ConsumeContext<ArrangeShipping> context)
    {
        var msg = context.Message;
        logger.LogInformation("Arranging delivery of {Qty}x {Product} for {OrderId}", msg.Quantity, msg.ProductName, msg.OrderId);

        await Task.Delay(600);

        var trackingNumber = $"TRK-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
        var estimatedDelivery = DateTime.UtcNow.AddDays(Random.Next(3, 8));

        logger.LogInformation("Shipping arranged for {OrderId}, Tracking: {Tracking}", msg.OrderId, trackingNumber);

        await context.Publish(new ShippingArranged
        {
            OrderId = msg.OrderId,
            CorrelationId = msg.CorrelationId,
            TrackingNumber = trackingNumber,
            EstimatedDelivery = estimatedDelivery
        });
    }
}
