using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Enums;
using RestaurantOrderTracking.Domain.Interface;
using RestaurantOrderTracking.Domain.Interface.Repository;

namespace RestaurantOrderTracking.Application.Feature.OrderItem.Commands.AssignChef
{
    public class AssignChefToOrderItemHandler : IRequestHandler<AssignChefToOrderItemCommand, Result>
    {
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IChefRepository _chefRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AssignChefToOrderItemHandler(
            IOrderItemRepository orderItemRepository,
            IChefRepository chefRepository,
            IUnitOfWork unitOfWork)
        {
            _orderItemRepository = orderItemRepository;
            _chefRepository = chefRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(AssignChefToOrderItemCommand request, CancellationToken cancellationToken)
        {
            var orderItem = await _orderItemRepository.GetByIdAsync(request.OrderItemId, cancellationToken);
            if (orderItem is null)
            {
                return Result.Failure($"Order item with id '{request.OrderItemId}' was not found.");
            }

            var chef = await _chefRepository.GetByAccountIdAsync(request.AccountId);
            if (chef is null)
            {
                return Result.Failure($"Chef with account id '{request.AccountId}' was not found.");
            }

            if (!chef.IsAvailable)
            {
                return Result.Failure($"Chef with account id '{request.AccountId}' is not available.");
            }

            orderItem.AssignChef(request.AccountId);

            try
            {
                orderItem.UpdateStatus(OrderItemStatus.Cooking);
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure(ex.Message);
            }

            chef.UpdateAvailability(false);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success("Assign chef to order item successfully.");
        }
    }
}