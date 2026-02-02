namespace RestaurantOrderTracking.Domain.Enums
{
    public enum NotificationType
    {
        OrderCreated = 0,
        OrderItemReady = 1,
        OrderItemServed = 2,
        OrderCompleted = 3,
        TableRequest = 4,
        PaymentReceived = 5,
        SystemAlert = 6,
        General = 7
    }
}
