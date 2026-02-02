namespace RestaurantOrderTracking.Domain.Enums
{
    public enum OrderItemStatus
    {
        Pending = 0,
        Confirmed = 1,
        Cooking = 2,
        Ready = 3,
        Delivering = 4,
        Served = 5,
        Cancelled = 6
    }
}
