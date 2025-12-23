namespace SagaDemo.Contracts.Commands;

public record ReleaseStock
{
    public Guid OrderId { get; init; }
    public Guid CorrelationId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public string Reason { get; init; } = string.Empty;
}
