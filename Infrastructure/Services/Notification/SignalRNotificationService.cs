using Microsoft.AspNetCore.SignalR;
using RestaurantOrderTracking.Application.Common.Interface;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantOrderTracking.Infrastructure.Services.Notification
{
    public class SignalRNotificationService : INotificationService
    {
        private readonly IHubContext _hubContext;

        public SignalRNotificationService(IHubContext hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyNewOrder(Guid orderId, string orderType, string status, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients.All.SendAsync("NotifyNewOrder", new
            {
                OrderId = orderId,
                OrderType = orderType,
                Status = status,
                CreatedAt = DateTime.UtcNow
            }, cancellationToken);
        }

        public async Task NotifyOrderStatusChanged(Guid orderId, string previousStatus, string newStatus, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients.All.SendAsync("NotifyOrderStatusChanged", new
            {
                OrderId = orderId,
                PreviousStatus = previousStatus,
                NewStatus = newStatus,
                UpdatedAt = DateTime.UtcNow
            }, cancellationToken);
        }

        public async Task NotifyPaymentSuccess(Guid orderId, decimal amount, string paymentMethod, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients.All.SendAsync("NotifyPaymentSuccess", new
            {
                OrderId = orderId,
                Amount = amount,
                PaymentMethod = paymentMethod,
                PaidAt = DateTime.UtcNow
            }, cancellationToken);
        }
    }
}
