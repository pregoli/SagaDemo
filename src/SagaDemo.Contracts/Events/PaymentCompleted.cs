namespace SagaDemo.Contracts.Events;

public record PaymentCompleted
{
    public Guid OrderId { get; init; }
    public Guid CorrelationId { get; init; }
    public string CustomerEmail { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string TransactionId { get; init; } = string.Empty;
}
