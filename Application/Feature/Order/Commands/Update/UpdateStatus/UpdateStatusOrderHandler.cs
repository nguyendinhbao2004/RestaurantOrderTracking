using MediatR;
using RestaurantOrderTracking.Application.Common.Interface;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Enums;
using RestaurantOrderTracking.Domain.Interface;
using RestaurantOrderTracking.Domain.Interface.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Application.Feature.Order.Commands.Update.UpdateStatus
{
    public class UpdateStatusOrderHandler : IRequestHandler<UpdateStatusOrderCommand, Result<Guid>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ITableRepository _tableRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        public UpdateStatusOrderHandler(
            IOrderRepository orderRepository,
            ITableRepository tableRepository,
            IUnitOfWork unitOfWork,
            INotificationService notificationService)
        {
            _orderRepository = orderRepository;
            _tableRepository = tableRepository;
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task<Result<Guid>> Handle(UpdateStatusOrderCommand request, CancellationToken cancellationToken)
        {

            var order = await _orderRepository.GetByIdAsync(request.Id);

            if (order == null)
                return Result<Guid>.Failure("Order not found.");

            var previousStatus = order.Status;

            try
            {

                order.UpdateStatus(request.NewStatus);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure(ex.Message);
            }

            if ((request.NewStatus == OrderStatus.Completed || request.NewStatus == OrderStatus.Cancelled)
                && order.TableId.HasValue)
            {
                var table = await _tableRepository.GetByIdAsync(order.TableId.Value, cancellationToken);
                if (table != null)
                {
                    table.SetAvailable();
                    _tableRepository.Update(table, cancellationToken);
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _notificationService.NotifyOrderStatusChanged(
                order.Id,
                previousStatus.ToString(),
                order.Status.ToString(),
                targetRoles: ResolveTargetRoles(order.Status),
                cancellationToken: cancellationToken);

            return Result<Guid>.Success("Update Order Status Successfully", order.Id);
        }

        private static IEnumerable<string> ResolveTargetRoles(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Confirmed => new[] { "Waiter", "Manager" },
                OrderStatus.Preparing => new[] { "Chef", "Manager" },
                OrderStatus.Delivering => new[] { "Waiter", "Manager" },
                OrderStatus.Paying => new[] { "Cashier", "Manager" },
                OrderStatus.Completed => new[] { "Cashier", "Manager" },
                OrderStatus.Cancelled => new[] { "Manager", "Waiter", "Cashier" },
                _ => new[] { "Manager" }
            };
        }
    }
}
