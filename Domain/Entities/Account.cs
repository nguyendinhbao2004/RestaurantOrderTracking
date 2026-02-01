using RestaurantOrderTracking.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Domain.Entities
{
    public class Account : BaseEntities
    {
        public int RoleId { get; private set; }
        public Role Roles { get; private set; } = null!;

        private readonly List<RefreshToken> _refreshTokens = new();
        public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

        private readonly List<VoiceCommand> _voiceCommands = new();
        public IReadOnlyCollection<VoiceCommand> VoiceCommands => _voiceCommands.AsReadOnly();

        public string UserName { get; private set; } = null!;

        public string FullName { get; private set; } = null!;

        public string Phone { get; private set; } = null!;

        public string PasswordHash { get; private set; } = null!;

        public Boolean IsActive { get; private set; }

        public Boolean IsWorking { get; private set; }

        protected Account() { }

        public Account(int roleId, string userName, string fullName, string phone, string passwordHash, bool isActive = true, bool isWorking = true)
        {
            RoleId = roleId;
            UserName = userName;
            FullName = fullName;
            Phone = phone;
            PasswordHash = passwordHash;
        }

        public void UpdateInfo(string fullName, string phone)
        {
            FullName = fullName;
            Phone = phone;
        }

        public void UpdateIsActive(bool isActive, bool isWorking)
        {
            IsActive = isActive;
        }

        public void UpdateIsWorking(bool isWorking)
        {
            IsWorking = isWorking;
        }

        public void AddRefreshToken(string token, string jwtId, int expiryDays = 30)
        {
            // Id là User.Id (kiểu string của Identity)
            // User tự tạo ra RefreshToken cho chính mình
            var refreshToken = new RefreshToken(this.Id, token, jwtId, DateTime.UtcNow.AddDays(expiryDays));
            _refreshTokens.Add(refreshToken);
        }

    }
}
