using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Interface;
using RestaurantOrderTracking.Domain.Interface.Repository;

namespace RestaurantOrderTracking.Application.Feature.OrderItem.Commands.Create
{
    public class CreateOrderItemsHandler : IRequestHandler<CreateOrderItemsCommand, Result>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateOrderItemsHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(CreateOrderItemsCommand request, CancellationToken cancellationToken)
        {
            // Validate input list
            if (request.Items == null || request.Items.Count == 0)
                return Result.Failure("Items list cannot be empty.");

            // Load the order (include its items so the domain entity is fully populated)
            var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

            if (order is null)
                return Result.Failure($"Order with ID '{request.OrderId}' was not found.");

            try
            {
                // Use the existing domain method to add each product as an OrderItem
                foreach (var item in request.Items)
                {
                    order.AddItem(
                        productId: item.ProductId,
                        accountId: request.CreatedBy,
                        note: item.Note ?? string.Empty,
                        orderChannel: request.OrderChannel
                    );
                }
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure(ex.Message);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success($"{request.Items.Count} order item(s) created successfully.");
        }
    }
}
