using MassTransit;
using Microsoft.Extensions.Logging;
using SagaDemo.Contracts;
using SagaDemo.Contracts.Commands;
using SagaDemo.Contracts.Events;

namespace SagaDemo.Infrastructure.Saga;

public class OrderSagaStateMachine : MassTransitStateMachine<OrderSagaState>
{
    public State Submitted { get; private set; } = null!;
    public State StockReserved { get; private set; } = null!;
    public State PaymentCompleted { get; private set; } = null!;
    public State Completed { get; private set; } = null!;
    public State Failed { get; private set; } = null!;
    public State Compensating { get; private set; } = null!;

    public Event<SubmitOrder> SubmitOrderCommand { get; private set; } = null!;
    public Event<StockReserved> StockReservedEvent { get; private set; } = null!;
    public Event<StockReservationFailed> StockReservationFailedEvent { get; private set; } = null!;
    public Event<PaymentCompleted> PaymentCompletedEvent { get; private set; } = null!;
    public Event<PaymentFailed> PaymentFailedEvent { get; private set; } = null!;
    public Event<ShippingArranged> ShippingArrangedEvent { get; private set; } = null!;
    public Event<StockReleased> StockReleasedEvent { get; private set; } = null!;

    public OrderSagaStateMachine(ILogger<OrderSagaStateMachine> logger)
    {
        InstanceState(x => x.CurrentState);

        ConfigureCorrelation();
        ConfigureLogging(logger);

        Initially(
            When(SubmitOrderCommand)
                .Then(context =>
                {
                    context.Saga.OrderId = context.Message.OrderId;
                    context.Saga.CustomerEmail = context.Message.CustomerEmail;
                    context.Saga.ProductName = context.Message.ProductName;
                    context.Saga.Quantity = context.Message.Quantity;
                    context.Saga.TotalAmount = context.Message.TotalAmount;
                    context.Saga.CreatedAt = DateTime.UtcNow;
                })
                .Send(new Uri($"queue:{QueueNames.ReserveStock}"), context => new ReserveStock
                {
                    OrderId = context.Message.OrderId,
                    CorrelationId = context.Message.CorrelationId,
                    ProductName = context.Message.ProductName,
                    Quantity = context.Message.Quantity
                })
                .TransitionTo(Submitted)
        );

        During(Submitted,
            When(StockReservedEvent)
                .Then(context => context.Saga.UpdatedAt = DateTime.UtcNow)
                .Send(new Uri($"queue:{QueueNames.ProcessPayment}"), context => new ProcessPayment
                {
                    OrderId = context.Saga.OrderId,
                    CorrelationId = context.Saga.CorrelationId,
                    CustomerEmail = context.Saga.CustomerEmail,
                    Amount = context.Saga.TotalAmount
                })
                .TransitionTo(StockReserved),

            When(StockReservationFailedEvent)
                .Then(context =>
                {
                    context.Saga.FailureReason = context.Message.Reason;
                    context.Saga.UpdatedAt = DateTime.UtcNow;
                })
                .Publish(context => new OrderFailed
                {
                    OrderId = context.Saga.OrderId,
                    CorrelationId = context.Saga.CorrelationId,
                    Reason = context.Message.Reason
                })
                .TransitionTo(Failed)
        );

        During(StockReserved,
            When(PaymentCompletedEvent)
                .Then(context => context.Saga.UpdatedAt = DateTime.UtcNow)
                .Send(new Uri($"queue:{QueueNames.ArrangeShipping}"), context => new ArrangeShipping
                {
                    OrderId = context.Saga.OrderId,
                    CorrelationId = context.Saga.CorrelationId,
                    CustomerEmail = context.Saga.CustomerEmail,
                    ProductName = context.Saga.ProductName,
                    Quantity = context.Saga.Quantity
                })
                .TransitionTo(PaymentCompleted),

            When(PaymentFailedEvent)
                .Then(context =>
                {
                    context.Saga.FailureReason = $"Payment failed: {context.Message.Reason}";
                    context.Saga.UpdatedAt = DateTime.UtcNow;
                })
                .Send(new Uri($"queue:{QueueNames.ReleaseStock}"), context => new ReleaseStock
                {
                    OrderId = context.Saga.OrderId,
                    CorrelationId = context.Saga.CorrelationId,
                    ProductName = context.Saga.ProductName,
                    Quantity = context.Saga.Quantity,
                    Reason = context.Message.Reason
                })
                .TransitionTo(Compensating)
        );

        During(PaymentCompleted,
            When(ShippingArrangedEvent)
                .Then(context =>
                {
                    context.Saga.TrackingNumber = context.Message.TrackingNumber;
                    context.Saga.EstimatedDelivery = context.Message.EstimatedDelivery;
                    context.Saga.UpdatedAt = DateTime.UtcNow;
                })
                .Publish(context => new OrderCompleted
                {
                    OrderId = context.Saga.OrderId,
                    CorrelationId = context.Saga.CorrelationId,
                    TrackingNumber = context.Saga.TrackingNumber ?? string.Empty
                })
                .TransitionTo(Completed)
        );

        During(Compensating,
            When(StockReleasedEvent)
                .Then(context => context.Saga.UpdatedAt = DateTime.UtcNow)
                .Publish(context => new OrderFailed
                {
                    OrderId = context.Saga.OrderId,
                    CorrelationId = context.Saga.CorrelationId,
                    Reason = context.Saga.FailureReason ?? "Unknown error"
                })
                .TransitionTo(Failed)
        );
    }

    private void ConfigureCorrelation()
    {
        Event(() => SubmitOrderCommand, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => StockReservedEvent, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => StockReservationFailedEvent, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => PaymentCompletedEvent, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => PaymentFailedEvent, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => ShippingArrangedEvent, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => StockReleasedEvent, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
    }

    private void ConfigureLogging(ILogger logger)
    {
        WhenEnterAny(binder => binder.Then(ctx =>
            logger.LogInformation(
                "Saga {CorrelationId} | Order {OrderId} | {PreviousState} -> {CurrentState}",
                ctx.Saga.CorrelationId,
                ctx.Saga.OrderId,
                ctx.Event.Name,
                ctx.Saga.CurrentState)));
    }
}
