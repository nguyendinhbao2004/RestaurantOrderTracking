using System.Security.Claims;

namespace RestaurantOrderTracking.WebApi.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal principal)
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userId, out var guid))
            {
                return guid;
            }
            throw new UnauthorizedAccessException("UserId is unvalid");
        }
    }
}
