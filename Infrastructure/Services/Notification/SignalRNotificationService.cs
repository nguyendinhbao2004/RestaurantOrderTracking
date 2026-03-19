using Microsoft.AspNetCore.SignalR;
using RestaurantOrderTracking.Application.Common.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantOrderTracking.Infrastructure.Services.Notification
{
    public class SignalRNotificationService : INotificationService
    {
        private readonly IHubContext _hubContext;
        private const string RoleGroupPrefix = "role:";
        private const string UserGroupPrefix = "user:";

        public SignalRNotificationService(IHubContext hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyNewOrder(
            Guid orderId,
            string orderType,
            string status,
            IEnumerable<string>? targetRoles = null,
            CancellationToken cancellationToken = default)
        {
            await GetClients(targetRoles).SendAsync("NotifyNewOrder", new
            {
                OrderId = orderId,
                OrderType = orderType,
                Status = status,
                CreatedAt = DateTime.UtcNow
            }, cancellationToken);
        }

        public async Task NotifyOrderStatusChanged(
            Guid orderId,
            string previousStatus,
            string newStatus,
            IEnumerable<string>? targetRoles = null,
            CancellationToken cancellationToken = default)
        {
            await GetClients(targetRoles).SendAsync("NotifyOrderStatusChanged", new
            {
                OrderId = orderId,
                PreviousStatus = previousStatus,
                NewStatus = newStatus,
                UpdatedAt = DateTime.UtcNow
            }, cancellationToken);
        }

        public async Task NotifyPaymentSuccess(
            Guid orderId,
            decimal amount,
            string paymentMethod,
            IEnumerable<string>? targetRoles = null,
            IEnumerable<Guid>? targetAccountIds = null,
            CancellationToken cancellationToken = default)
        {
            await GetClients(targetRoles, targetAccountIds).SendAsync("NotifyPaymentSuccess", new
            {
                OrderId = orderId,
                Amount = amount,
                PaymentMethod = paymentMethod,
                PaidAt = DateTime.UtcNow
            }, cancellationToken);
        }

        private IClientProxy GetClients(IEnumerable<string>? targetRoles, IEnumerable<Guid>? targetAccountIds = null)
        {
            var roleGroups = (targetRoles ?? Enumerable.Empty<string>())
                .Where(role => !string.IsNullOrWhiteSpace(role))
                .Select(role => $"{RoleGroupPrefix}{role.Trim().ToLowerInvariant()}")
                .ToList();

            var accountGroups = (targetAccountIds ?? Enumerable.Empty<Guid>())
                .Where(accountId => accountId != Guid.Empty)
                .Select(accountId => $"{UserGroupPrefix}{accountId.ToString("D").ToLowerInvariant()}")
                .ToList();

            var groups = roleGroups
                .Concat(accountGroups)
                .Distinct()
                .ToList();

            return groups.Count == 0
                ? _hubContext.Clients.All
                : _hubContext.Clients.Groups(groups);
        }
    }
}
