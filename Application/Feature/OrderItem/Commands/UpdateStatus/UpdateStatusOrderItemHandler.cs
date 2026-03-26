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
        private readonly IChefRepository _chefRepository;
        private readonly IUnitOfWork _unitOfWork;

        private readonly INotificationService _notificationService;

        public UpdateStatusOrderItemHandler(
            IOrderItemRepository orderItemRepository,
            IOrderItemLogRepository orderItemLogRepository,
            IChefRepository chefRepository,
            IUnitOfWork unitOfWork,
            INotificationService notificationService)
        {
            _orderItemRepository = orderItemRepository;
            _orderItemLogRepository = orderItemLogRepository;
            _chefRepository = chefRepository;
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task<Result> Handle(UpdateStatusOrderItemCommand request, CancellationToken cancellationToken)
        {
            if (request.OrderItemIds == null || !request.OrderItemIds.Any())
                return Result.Failure("No OrderItems provided for update.");

            var orderTransitions = new Dictionary<Guid, OrderItemStatus>();
            var successCount = 0;
            bool hasCookingToReadyTransition = false;

            foreach (var orderItemId in request.OrderItemIds)
            {
                // 1. Load the order item
                var orderItem = await _orderItemRepository.GetByIdAsync(orderItemId, cancellationToken);
                if (orderItem is null)
                    return Result.Failure($"OrderItem with ID '{orderItemId}' was not found.");

                var previousStatus = orderItem.Status;

                // 2. Attempt the status transition (domain guards sequential rule & terminal states)
                try
                {
                    orderItem.UpdateStatus(request.NewStatus);
                }
                catch (InvalidOperationException ex)
                {
                    return Result.Failure($"Failed to update OrderItem '{orderItemId}': " + ex.Message);
                }

                // 3. Handle assignee based on transition
                if (previousStatus == OrderItemStatus.Confirmed && request.NewStatus == OrderItemStatus.Cooking)
                {
                    if (!request.AssigneeId.HasValue)
                        return Result.Failure("AssigneeId (chef) is required when transitioning from Confirmed to Cooking.");

                    var chef = await _chefRepository.GetByAccountIdAsync(request.AssigneeId.Value);
                    if (chef is null)
                        return Result.Failure($"Chef with account id '{request.AssigneeId.Value}' was not found.");

                    if (!chef.IsAvailable)
                        return Result.Failure($"Chef with account id '{request.AssigneeId.Value}' is not available.");

                    orderItem.AssignChef(request.AssigneeId.Value);
                    chef.UpdateAvailability(false);
                }
                else if (previousStatus == OrderItemStatus.Cooking && request.NewStatus == OrderItemStatus.Ready)
                {
                    hasCookingToReadyTransition = true;
                }
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

                // Track the first known transition for an order's notification
                orderTransitions.TryAdd(orderItem.OrderId, previousStatus);
                successCount++;
            }

            // 5. Check chef availability if any order item transitioned from Cooking to Ready
            if (hasCookingToReadyTransition && request.AccountId.HasValue)
            {
                var remainingCookingItems = await _orderItemRepository.FindAsync(x =>
                    x.ChefAccountId == request.AccountId.Value &&
                    x.Status == OrderItemStatus.Cooking &&
                    !request.OrderItemIds.Contains(x.Id));

                if (!remainingCookingItems.Any())
                {
                    var chef = await _chefRepository.GetByAccountIdAsync(request.AccountId.Value);
                    if (chef != null && !chef.IsAvailable)
                    {
                        chef.UpdateAvailability(true);
                    }
                }
            }

            // 6. Save both changes in a single transaction
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 7. Send real-time notification about the status change for each unique order
            foreach (var kvp in orderTransitions)
            {
                await _notificationService.NotifyOrderStatusChanged(
                    orderId: kvp.Key,
                    previousStatus: kvp.Value.ToString(),
                    newStatus: request.NewStatus.ToString(),
                    targetRoles: ResolveTargetRoles(request.NewStatus),
                    cancellationToken: cancellationToken);
            }

            return Result.Success($"{successCount} OrderItem(s) updated to {request.NewStatus} successfully.");
        }

        private static IEnumerable<string> ResolveTargetRoles(OrderItemStatus status)
        {
            return status switch
            {
                OrderItemStatus.Confirmed => new[] { "Chef", "Manager" },
                OrderItemStatus.Cooking => new[] { "Chef", "Manager" },
                OrderItemStatus.Ready => new[] { "Waiter", "Manager" },
                OrderItemStatus.Delivering => new[] { "Waiter", "Manager" },
                OrderItemStatus.Served => new[] { "Waiter", "Cashier", "Manager" },
                _ => new[] { "Manager" }
            };
        }
    }
}
