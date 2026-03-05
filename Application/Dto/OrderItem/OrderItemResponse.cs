using System;

namespace RestaurantOrderTracking.Application.Dto.OrderItem
{
    public class OrderItemResponse
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public string ProductName { get; set; }
        public string Status { get; set; }
    }
}
