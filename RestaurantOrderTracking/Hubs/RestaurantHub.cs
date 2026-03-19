using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Security.Claims;

namespace RestaurantOrderTracking.WebApi.Hubs
{
    public class RestaurantHub : Hub
    {
        private const string RoleGroupPrefix = "role:";

        public override async Task OnConnectedAsync()
        {
            var roles = Context.User?.Claims
                .Where(claim => claim.Type == ClaimTypes.Role || claim.Type == "role")
                .Select(claim => claim.Value?.Trim().ToLowerInvariant())
                .Where(role => !string.IsNullOrWhiteSpace(role))
                .Distinct()
                .ToList();

            if (roles != null)
            {
                foreach (var role in roles)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"{RoleGroupPrefix}{role}");
                }
            }

            await base.OnConnectedAsync();
        }
    }
}
