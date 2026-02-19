using RestaurantOrderTracking.Domain.Entities;

namespace Application.Dto.Table
{
    public class TableDetailResponse
    {
        public string TableNumber { get; set; } = null!;
        public string AreaName { get; set; }
        public string Status { get; set; }
        public string? QRCode { get; set; }
        public int Capacity { get; set; }

        public List<Order> Orders { get; set; } = new List<Order>();

    }
}