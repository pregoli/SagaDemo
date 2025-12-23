namespace SagaDemo.Contracts.Commands;

public record SubmitOrder
{
    public Guid OrderId { get; init; }
    public Guid CorrelationId { get; init; }
    public string CustomerEmail { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal TotalAmount { get; init; }
}
