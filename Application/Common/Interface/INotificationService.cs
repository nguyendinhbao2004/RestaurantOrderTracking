using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantOrderTracking.Application.Common.Interface
{
    public interface INotificationService
    {
        Task NotifyNewOrder(
            Guid orderId,
            string orderType,
            string status,
            IEnumerable<string>? targetRoles = null,
            CancellationToken cancellationToken = default);

        Task NotifyOrderStatusChanged(
            Guid orderId,
            string previousStatus,
            string newStatus,
            IEnumerable<string>? targetRoles = null,
            CancellationToken cancellationToken = default);

        Task NotifyPaymentSuccess(
            Guid orderId,
            decimal amount,
            string paymentMethod,
            IEnumerable<string>? targetRoles = null,
            CancellationToken cancellationToken = default);
    }
}
