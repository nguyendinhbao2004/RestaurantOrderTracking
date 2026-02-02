using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Exceptions;
using System;

namespace RestaurantOrderTracking.Domain.Entities
{
    public class RefreshToken : BaseEntities
    {
        public Guid UserId { get; private set; }
        public virtual Account User { get; private set; } = null!;

        public string Token { get; private set; } = null!;
        public string JwtId { get; private set; } = null!;
        public bool IsUsed { get; private set; }
        public bool IsRevoked { get; private set; }
        public DateTime Expires { get; private set; }
        public DateTime AddedDate { get; private set; }

        protected RefreshToken() { }

        public RefreshToken(Guid userId, string token, string jwtId, DateTime expires)
        {
            if (userId == Guid.Empty)
                throw new DomainException("UserId cannot be empty.");
            if (string.IsNullOrWhiteSpace(token))
                throw new DomainException("Token cannot be empty.");
            if (string.IsNullOrWhiteSpace(jwtId))
                throw new DomainException("JwtId cannot be empty.");

            UserId = userId;
            Token = token;
            JwtId = jwtId;
            Expires = expires;
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
            if (IsUsed)
                throw new DomainException("Token has already been used.");
            if (IsRevoked)
                throw new DomainException("Token has been revoked.");
            if (DateTime.UtcNow > Expires)
                throw new DomainException("Token has expired.");

            IsUsed = true;
        }

        public bool IsValid()
        {
            return !IsUsed && !IsRevoked && DateTime.UtcNow <= Expires;
        }
    }
}
