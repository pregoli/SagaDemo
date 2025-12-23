using MassTransit;
using SagaDemo.Contracts.Commands;
using SagaDemo.Contracts.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace SagaDemo.StockService.Cinsumers;

public class ReleaseStockConsumer(ILogger<ReleaseStockConsumer> logger) : IConsumer<ReleaseStock>
{
    public async Task Consume(ConsumeContext<ReleaseStock> context)
    {
        var msg = context.Message;
        logger.LogInformation("COMPENSATION: Releasing {Qty}x {Product} for {OrderId}", msg.Quantity, msg.ProductName, msg.OrderId);

        await Task.Delay(300);

        logger.LogInformation("Released stock for {OrderId}", msg.OrderId);
        await context.Publish(new StockReleased
        {
            OrderId = msg.OrderId,
            CorrelationId = msg.CorrelationId,
            ProductName = msg.ProductName,
            Quantity = msg.Quantity
        });
    }
}
