using RestaurantOrderTracking.Application.Dto.Order;
using RestaurantOrderTracking.Domain.Entities;

namespace Application.Dto.Table
{
    public class TableDetailResponse
    {
        public Guid Id { get; set; }
        public string TableNumber { get; set; } = null!;
        public string AreaName { get; set; }
        public string Status { get; set; }
        public string? QRCode { get; set; }
        public int Capacity { get; set; }

        public List<OrderResponse> Orders { get; set; } = new();

    }
}