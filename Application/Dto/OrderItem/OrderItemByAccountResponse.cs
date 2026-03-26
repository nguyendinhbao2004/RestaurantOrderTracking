using System;

namespace RestaurantOrderTracking.Application.Dto.OrderItem
{
    public class OrderItemByAccountResponse
    {
        public Guid OrderItemId { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? Note { get; set; }
        public string Status { get; set; } = null!;
        public DateTime OrderAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public Guid? TableId { get; set; }
        public string? TableNumber { get; set; }
        public Guid? AreaId { get; set; }
        public string? AreaName { get; set; }
        public string? OrderType { get; set; }
        public string? OrderStatus { get; set; }
    }
}
