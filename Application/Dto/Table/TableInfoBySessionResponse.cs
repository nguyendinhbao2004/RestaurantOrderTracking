using System;

namespace RestaurantOrderTracking.Application.Dto.Table
{
    /// <summary>
    /// Response DTO for table information retrieved by session token.
    /// Used when customers scan QR code to access the menu.
    /// </summary>
    public class TableInfoBySessionResponse
    {
        public Guid TableId { get; set; }
        public string TableNumber { get; set; } = null!;
        public string AreaName { get; set; } = null!;
        public string Status { get; set; } = null!;
        public int Capacity { get; set; }
        
        /// <summary>
        /// The session token that was validated
        /// </summary>
        public string SessionToken { get; set; } = null!;
        
        /// <summary>
        /// When the session expires
        /// </summary>
        public DateTime ExpiresAt { get; set; }
    }
}
