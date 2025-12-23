namespace SagaDemo.Contracts;

public static class QueueNames
{
    public const string OrderSaga = "order-saga";
    public const string ReserveStock = "reserve-stock";
    public const string ReleaseStock = "release-stock";
    public const string ProcessPayment = "process-payment";
    public const string ArrangeShipping = "arrange-shipping";
}
