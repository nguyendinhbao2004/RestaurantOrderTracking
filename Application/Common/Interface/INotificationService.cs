using System;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantOrderTracking.Application.Common.Interface
{
    public interface INotificationService
    {
        Task NotifyNewOrder(Guid orderId, string orderType, string status, CancellationToken cancellationToken = default);

        Task NotifyOrderStatusChanged(Guid orderId, string previousStatus, string newStatus, CancellationToken cancellationToken = default);

        Task NotifyPaymentSuccess(Guid orderId, decimal amount, string paymentMethod, CancellationToken cancellationToken = default);
    }
}
