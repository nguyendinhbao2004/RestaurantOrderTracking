using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Domain.Entities
{
    public class RefreshToken : BaseEntities
    {
        public Guid UserId { get; private set; } // IdentityUser dùng String ID
        public string Token { get; private set; }
        public string JwtId { get; private set; } // JTI
        public bool IsUsed { get; private set; }
        public bool IsRevoked { get; private set; }
        public DateTime Expires { get; private set; }
        public DateTime AddedDate { get; private set; } = DateTime.UtcNow;

        protected RefreshToken()
        {

        }
        // Constructor
        public RefreshToken(Guid userId, string token, string jwtId, DateTime expires)
        {
            if (userId == Guid.Empty) throw new DomainException("UserId cannot be empty.");
            if (string.IsNullOrWhiteSpace(token)) throw new DomainException("Token cannot be empty.");

            UserId = userId;
            Token = token;
            JwtId = jwtId;
            Expires = expires;

            // Giá trị mặc định
            IsUsed = false;
            IsRevoked = false;
            AddedDate = DateTime.UtcNow;
        }

        public void Revoke()
        {
            IsRevoked = true;
        }

        public void Use()
        {
            IsUsed = true;
        }
    }
}
