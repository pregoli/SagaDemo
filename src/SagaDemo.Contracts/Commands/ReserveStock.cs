namespace SagaDemo.Contracts.Commands;

public record ReserveStock
{
    public Guid OrderId { get; init; }
    public Guid CorrelationId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }
}
