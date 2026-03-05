using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Enums;
using RestaurantOrderTracking.Domain.Interface;
using RestaurantOrderTracking.Domain.Interface.Repository;

namespace RestaurantOrderTracking.Application.Feature.OrderItem.Commands.UpdateInfo
{
    public class UpdateInfoOrderItemHandler : IRequestHandler<UpdateInfoOrderItemCommand, Result>
    {
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateInfoOrderItemHandler(
            IOrderItemRepository orderItemRepository,
            IUnitOfWork unitOfWork)
        {
            _orderItemRepository = orderItemRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateInfoOrderItemCommand request, CancellationToken cancellationToken)
        {
            // Require at least one field to be provided
            if (!request.ChefAccountId.HasValue && !request.WaiterAccountId.HasValue && request.Note is null)
                return Result.Failure("At least one field (ChefAccountId, WaiterAccountId, or Note) must be provided.");

            var orderItem = await _orderItemRepository.GetByIdAsync(request.OrderItemId, cancellationToken);
            if (orderItem is null)
                return Result.Failure($"OrderItem with ID '{request.OrderItemId}' was not found.");

            var errors = new List<string>();
            bool anyUpdated = false;

            // ChefAccountId — null: skip silently | value + wrong status: error, skip | value + correct status: update
            if (request.ChefAccountId.HasValue)
            {
                if (orderItem.Status != OrderItemStatus.Cooking)
                    errors.Add($"ChefAccountId cannot be updated: status must be Cooking (2). Current status: {orderItem.Status}.");
                else
                {
                    orderItem.AssignChef(request.ChefAccountId.Value);
                    anyUpdated = true;
                }
            }

            // WaiterAccountId — null: skip silently | value + wrong status: error, skip | value + correct status: update
            if (request.WaiterAccountId.HasValue)
            {
                if (orderItem.Status != OrderItemStatus.Delivering)
                    errors.Add($"WaiterAccountId cannot be updated: status must be Delivering (4). Current status: {orderItem.Status}.");
                else
                {
                    orderItem.AssignWaiter(request.WaiterAccountId.Value);
                    anyUpdated = true;
                }
            }

            // Note — null: skip silently (keep existing) | value + wrong status: error, skip | value + correct status: update
            if (request.Note is not null)
            {
                if (orderItem.Status != OrderItemStatus.Pending && orderItem.Status != OrderItemStatus.Confirmed)
                    errors.Add($"Note cannot be updated: status must be Pending (0) or Confirmed (1). Current status: {orderItem.Status}.");
                else
                {
                    orderItem.UpdateNote(request.Note);
                    anyUpdated = true;
                }
            }

            // Save only if at least one field was successfully updated
            if (anyUpdated)
                await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Partial success: some fields updated, some rejected
            if (errors.Any() && anyUpdated)
            {
                var message = "Some fields were updated. The following fields could not be updated due to status restrictions:";
                return new Result(true, message, errors);
            }

            // Full failure: all provided fields were rejected
            if (errors.Any())
                return Result.Failure(errors);

            return Result.Success("OrderItem info updated successfully.");
        }
    }
}

