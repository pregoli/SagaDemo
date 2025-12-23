namespace SagaDemo.Contracts.Events;

public record ShippingArranged
{
    public Guid OrderId { get; init; }
    public Guid CorrelationId { get; init; }
    public string TrackingNumber { get; init; } = string.Empty;
    public DateTime EstimatedDelivery { get; init; }
}
