using System;

namespace RestaurantOrderTracking.Application.Dto.Area
{
    public class AreaWaiterResponse
    {
        public Guid AccountId { get; set; }
        public string FullName { get; set; } = null!;
    }
}
