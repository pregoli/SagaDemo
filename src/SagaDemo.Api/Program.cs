using MassTransit;
using Microsoft.EntityFrameworkCore;
using SagaDemo.Application.DTOs;
using SagaDemo.Contracts;
using SagaDemo.Contracts.Commands;
using SagaDemo.Infrastructure.Configuration;
using SagaDemo.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

EndpointConvention.Map<SubmitOrder>(new Uri($"queue:{QueueNames.OrderSaga}"));

var app = builder.Build();

await app.Services.EnsureDatabaseCreatedAsync();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/orders", async (
    CreateOrderDto dto,
    IBus bus,
    ILogger<Program> logger,
    CancellationToken ct) =>
{
    var correlationId = Guid.NewGuid();

    logger.LogInformation("Sending SubmitOrder: {OrderId}, Product: {Product}", correlationId, dto.ProductName);

    var endpoint = await bus.GetSendEndpoint(new Uri($"queue:{QueueNames.OrderSaga}"));
    await endpoint.Send(new SubmitOrder
    {
        OrderId = correlationId,
        CorrelationId = correlationId,
        CustomerEmail = dto.CustomerEmail,
        ProductName = dto.ProductName,
        Quantity = dto.Quantity,
        TotalAmount = dto.TotalAmount
    }, ct);

    return Results.Accepted($"/orders/{correlationId}", new CreateOrderResult
    {
        OrderId = correlationId,
        CorrelationId = correlationId,
        Status = "Submitted"
    });
})
.WithName("CreateOrder");

app.MapGet("/orders/{id:guid}", async (Guid id, AppDbContext db, CancellationToken ct) =>
{
    var saga = await db.OrderSagaStates.FirstOrDefaultAsync(s => s.OrderId == id, ct);
    if (saga is null) return Results.NotFound();

    return Results.Ok(new OrderDto
    {
        Id = saga.OrderId,
        CustomerEmail = saga.CustomerEmail,
        ProductName = saga.ProductName,
        Quantity = saga.Quantity,
        TotalAmount = saga.TotalAmount,
        Status = saga.CurrentState,
        FailureReason = saga.FailureReason,
        CreatedAt = saga.CreatedAt,
        UpdatedAt = saga.UpdatedAt
    });
})
.WithName("GetOrder");

app.MapGet("/orders", async (AppDbContext db, CancellationToken ct) =>
{
    var orders = await db.OrderSagaStates
        .OrderByDescending(s => s.CreatedAt)
        .Select(s => new OrderDto
        {
            Id = s.OrderId,
            CustomerEmail = s.CustomerEmail,
            ProductName = s.ProductName,
            Quantity = s.Quantity,
            TotalAmount = s.TotalAmount,
            Status = s.CurrentState,
            FailureReason = s.FailureReason,
            CreatedAt = s.CreatedAt,
            UpdatedAt = s.UpdatedAt
        })
        .ToListAsync(ct);

    return Results.Ok(orders);
})
.WithName("GetAllOrders");

app.MapGet("/sagas/{correlationId:guid}", async (Guid correlationId, AppDbContext db, CancellationToken ct) =>
{
    var saga = await db.OrderSagaStates.FirstOrDefaultAsync(s => s.CorrelationId == correlationId, ct);
    if (saga is null) return Results.NotFound();

    return Results.Ok(new
    {
        saga.CorrelationId,
        saga.OrderId,
        saga.CurrentState,
        saga.CustomerEmail,
        saga.ProductName,
        saga.Quantity,
        saga.TotalAmount,
        saga.FailureReason,
        saga.TrackingNumber,
        saga.EstimatedDelivery,
        saga.CreatedAt,
        saga.UpdatedAt
    });
})
.WithName("GetSagaState");

app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }))
.WithName("HealthCheck");

app.Run();
