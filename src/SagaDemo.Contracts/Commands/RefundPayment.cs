namespace SagaDemo.Contracts.Commands;

public record RefundPayment
{
    public Guid OrderId { get; init; }
    public Guid CorrelationId { get; init; }
    public string CustomerEmail { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Reason { get; init; } = string.Empty;
}
