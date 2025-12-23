namespace SagaDemo.Application.DTOs;

public record CreateOrderDto
{
    public string CustomerEmail { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal TotalAmount { get; init; }
}

public record OrderDto
{
    public Guid Id { get; init; }
    public string CustomerEmail { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal TotalAmount { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? FailureReason { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public record CreateOrderResult
{
    public Guid OrderId { get; init; }
    public Guid CorrelationId { get; init; }
    public string Status { get; init; } = string.Empty;
}
