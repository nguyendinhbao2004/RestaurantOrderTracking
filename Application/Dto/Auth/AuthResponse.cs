using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Application.Dto.Auth
{
    public class AuthResponse
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string Role { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }
}
