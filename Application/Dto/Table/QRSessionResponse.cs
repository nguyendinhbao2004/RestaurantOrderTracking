using System;

namespace RestaurantOrderTracking.Application.Dto.Table
{
    public class QRSessionResponse
    {
        public Guid TableId { get; set; }
        public string TableNumber { get; set; } = null!;
        public string SessionToken { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public bool IsActive { get; set; }
    }
}
