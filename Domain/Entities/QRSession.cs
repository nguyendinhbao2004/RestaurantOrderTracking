using RestaurantOrderTracking.Domain.Common;
using System;

namespace RestaurantOrderTracking.Domain.Entities
{
    public class QRSession : BaseEntities
    {
        public Guid TableId { get; private set; }
        public virtual Table Table { get; private set; } = null!;

        public string SessionToken { get; private set; } = null!;
        public DateTime ExpiresAt { get; private set; }
        public bool IsActive { get; private set; }

        protected QRSession() { }

        public QRSession(Guid tableId, int expirationMinutes = 480)
        {
            TableId = tableId;
            SessionToken = Guid.NewGuid().ToString("N");
            ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);
            IsActive = true;
        }

        public void Refresh(int expirationMinutes = 480)
        {
            SessionToken = Guid.NewGuid().ToString("N");
            ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Revoke()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool IsExpired() => DateTime.UtcNow > ExpiresAt;

        public bool IsValid() => IsActive && !IsExpired();
    }
}
