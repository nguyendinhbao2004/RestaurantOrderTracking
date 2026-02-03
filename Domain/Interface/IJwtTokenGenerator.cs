using RestaurantOrderTracking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Domain.Interface
{
    public interface IJwtTokenGenerator
    {
        // Hàm sinh Access Token (kèm Roles)
        string GenerateToken(Account account, string roles);

        // Hàm sinh Refresh Token
        string GenerateRefreshToken();
    }
}
