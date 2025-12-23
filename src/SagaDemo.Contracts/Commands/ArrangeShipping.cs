namespace SagaDemo.Contracts.Commands;

public record ArrangeShipping
{
    public Guid OrderId { get; init; }
    public Guid CorrelationId { get; init; }
    public string CustomerEmail { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }
}
