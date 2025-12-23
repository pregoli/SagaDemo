namespace SagaDemo.Contracts.Events;

public record OrderFailed
{
    public Guid OrderId { get; init; }
    public Guid CorrelationId { get; init; }
    public string Reason { get; init; } = string.Empty;
}
