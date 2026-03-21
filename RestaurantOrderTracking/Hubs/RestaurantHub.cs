using Microsoft.AspNetCore.SignalR;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace RestaurantOrderTracking.WebApi.Hubs
{
    public class RestaurantHub : Hub
    {
        private const string RoleGroupPrefix = "role:";
        private const string UserGroupPrefix = "user:";
        private const string OrderCodeGroupPrefix = "order-code:";

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

            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? Context.User?.FindFirstValue(JwtRegisteredClaimNames.Sub)
                         ?? Context.User?.FindFirstValue("sub");

            if (Guid.TryParse(userId, out var accountId))
            {
                await Groups.AddToGroupAsync(
                    Context.ConnectionId,
                    $"{UserGroupPrefix}{accountId.ToString("D").ToLowerInvariant()}");
            }

            await base.OnConnectedAsync();
        }

        public async Task SubscribeOrderCode(long orderCode)
        {
            if (orderCode <= 0)
                throw new HubException("OrderCode không hợp lệ.");

            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                $"{OrderCodeGroupPrefix}{orderCode}");
        }

        public async Task UnsubscribeOrderCode(long orderCode)
        {
            if (orderCode <= 0)
                return;

            await Groups.RemoveFromGroupAsync(
                Context.ConnectionId,
                $"{OrderCodeGroupPrefix}{orderCode}");
        }
    }
}
