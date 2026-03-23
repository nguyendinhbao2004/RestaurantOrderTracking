using System;

namespace RestaurantOrderTracking.Application.Dto.OrderItem
{
    public class OrderItemResponse
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public string ProductName { get; set; }
        public string Status { get; set; }
        public Guid? TableId { get; set; }
        public string? TableNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }
}
