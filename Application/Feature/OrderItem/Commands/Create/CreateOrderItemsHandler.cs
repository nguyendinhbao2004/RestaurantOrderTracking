using MediatR;
using RestaurantOrderTracking.Domain.Common;
using Domain.Interface.Repository;
using RestaurantOrderTracking.Domain.Interface;
using RestaurantOrderTracking.Domain.Interface.Repository;

namespace RestaurantOrderTracking.Application.Feature.OrderItem.Commands.Create
{
    public class CreateOrderItemsHandler : IRequestHandler<CreateOrderItemsCommand, Result>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateOrderItemsHandler(IOrderRepository orderRepository, IProductRepository productRepository,
            IUnitOfWork unitOfWork) {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(CreateOrderItemsCommand request, CancellationToken cancellationToken)
        {
            // Validate input list
            if (request.Items == null || request.Items.Count == 0)
                return Result.Failure("Items list cannot be empty.");

            foreach (var item in request.Items)
            {
                if (item.Quantity <= 0)
                    return Result.Failure($"Quantity for product '{item.ProductId}' must be greater than 0.");

                var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);
                if (product is null)
                    return Result.Failure($"Product with ID '{item.ProductId}' was not found.");
            }

            // Load the order (include its items so the domain entity is fully populated)
            var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

            if (order is null)
                return Result.Failure($"Order with ID '{request.OrderId}' was not found.");

            var totalCreatedItems = 0;

            try
            {
                foreach (var item in request.Items)
                {
                    for (int i = 0; i < item.Quantity; i++)
                    {
                        order.AddItem(
                            productId: item.ProductId,
                            accountId: request.CreatedBy,
                            note: item.Note ?? string.Empty,
                            orderChannel: request.OrderChannel
                        );
                        totalCreatedItems++;
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure(ex.Message);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success($"{totalCreatedItems} order item(s) created successfully.");
        }
    }
}
