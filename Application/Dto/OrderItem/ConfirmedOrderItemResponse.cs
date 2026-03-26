namespace RestaurantOrderTracking.Application.Dto.OrderItem
{
    public class ConfirmedOrderItemResponse
    {
        public Guid OrderItemId { get; set; }
        public Guid OrderId { get; set; }
        public Guid? TableId { get; set; }
        public string? TableNumber { get; set; }
        public Guid? AreaId { get; set; }
        public string? AreaName { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal ProductPrice { get; set; }
        public string OrderChannel { get; set; } = null!;
        public string? Note { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}