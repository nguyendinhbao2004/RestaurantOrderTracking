using System;

namespace RestaurantOrderTracking.Application.Dto.Area
{
    public class AreaResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
}
