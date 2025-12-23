namespace SagaDemo.Contracts.Events;

public record OrderCompleted
{
    public Guid OrderId { get; init; }
    public Guid CorrelationId { get; init; }
    public string TrackingNumber { get; init; } = string.Empty;
}
