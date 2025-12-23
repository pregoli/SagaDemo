using MassTransit;
using SagaDemo.Contracts.Commands;
using SagaDemo.Contracts.Events;

namespace SagaDemo.PaymentService.Consumers;

public class ProcessPaymentConsumer(ILogger<ProcessPaymentConsumer> logger) : IConsumer<ProcessPayment>
{
    private static readonly Random Random = new();

    public async Task Consume(ConsumeContext<ProcessPayment> context)
    {
        var msg = context.Message;
        logger.LogInformation("Processing {Amount:C} for {Email}, Order {OrderId}", msg.Amount, msg.CustomerEmail, msg.OrderId);

        await Task.Delay(800);

        if (Random.Next(100) < 20)
        {
            var reason = msg.Amount > 500 ? "Credit limit exceeded" : "Card declined";
            logger.LogWarning("Payment failed for {OrderId}: {Reason}", msg.OrderId, reason);
            await context.Publish(new PaymentFailed
            {
                OrderId = msg.OrderId,
                CorrelationId = msg.CorrelationId,
                CustomerEmail = msg.CustomerEmail,
                Amount = msg.Amount,
                Reason = reason
            });
            return;
        }

        var transactionId = Guid.NewGuid().ToString("N")[..12].ToUpper();
        logger.LogInformation("Payment success for {OrderId}, TxId: {TxId}", msg.OrderId, transactionId);

        await context.Publish(new PaymentCompleted
        {
            OrderId = msg.OrderId,
            CorrelationId = msg.CorrelationId,
            CustomerEmail = msg.CustomerEmail,
            Amount = msg.Amount,
            TransactionId = transactionId
        });
    }
}
