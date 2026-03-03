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

        /// <summary>
        /// Base64-encoded PNG image. Use as: &lt;img src="data:image/png;base64,{QRCodeBase64}" /&gt;
        /// </summary>
        public string? QRCodeBase64 { get; set; }
    }
}
