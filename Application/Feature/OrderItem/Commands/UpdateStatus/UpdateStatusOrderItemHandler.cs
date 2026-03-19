using MediatR;
using RestaurantOrderTracking.Application.Common.Interface;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Entities;
using RestaurantOrderTracking.Domain.Enums;
using RestaurantOrderTracking.Domain.Interface;
using RestaurantOrderTracking.Domain.Interface.Repository;

namespace RestaurantOrderTracking.Application.Feature.OrderItem.Commands.UpdateStatus
{
    public class UpdateStatusOrderItemHandler : IRequestHandler<UpdateStatusOrderItemCommand, Result>
    {
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IOrderItemLogRepository _orderItemLogRepository;
        private readonly IUnitOfWork _unitOfWork;

        private readonly INotificationService _notificationService;

        public UpdateStatusOrderItemHandler(
            IOrderItemRepository orderItemRepository,
            IOrderItemLogRepository orderItemLogRepository,
            IUnitOfWork unitOfWork,
            INotificationService notificationService)
        {
            _orderItemRepository = orderItemRepository;
            _orderItemLogRepository = orderItemLogRepository;
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task<Result> Handle(UpdateStatusOrderItemCommand request, CancellationToken cancellationToken)
        {
            // 1. Load the order item
            var orderItem = await _orderItemRepository.GetByIdAsync(request.OrderItemId, cancellationToken);
            if (orderItem is null)
                return Result.Failure($"OrderItem with ID '{request.OrderItemId}' was not found.");

            var previousStatus = orderItem.Status;


            // 2. Attempt the status transition (domain guards sequential rule & terminal states)
            try
            {
                orderItem.UpdateStatus(request.NewStatus);
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure(ex.Message);
            }

            // 3. Handle assignee based on transition
            // Confirmed(1) → Cooking(2): chef must be provided
            if (previousStatus == OrderItemStatus.Confirmed && request.NewStatus == OrderItemStatus.Cooking)
            {
                if (!request.AssigneeId.HasValue)
                    return Result.Failure("AssigneeId (chef) is required when transitioning from Confirmed to Cooking.");

                orderItem.AssignChef(request.AssigneeId.Value);
            }
            // Ready(3) → Delivering(4): waiter = person doing the action (AccountId)
            // If AccountId is null (customer action), waiter assignment is skipped
            else if (previousStatus == OrderItemStatus.Ready && request.NewStatus == OrderItemStatus.Delivering)
            {
                if (request.AccountId.HasValue)
                    orderItem.AssignWaiter(request.AccountId.Value);
            }

            // 4. Create audit log entry
            var log = new OrderItemLog(
                orderItemId: orderItem.Id,
                previousStatus: previousStatus,
                newStatus: request.NewStatus,
                changeSource: request.ChangeSource,
                accountId: request.AccountId
            );

            await _orderItemLogRepository.AddAsync(log);

            // 5. Save both changes in a single transaction
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            // 6. Send real-time notification about the status change
            await _notificationService.NotifyOrderStatusChanged(
                orderId: orderItem.OrderId,
                previousStatus: previousStatus.ToString(),
                newStatus: request.NewStatus.ToString(),
                cancellationToken: cancellationToken);  

            return Result.Success($"OrderItem status updated from {previousStatus} to {request.NewStatus} successfully.");
        }
    }
}
