using MediatR;
using RestaurantOrderTracking.Application.Common.Interface;
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
        private readonly IAccountRepository _accountRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public CreateOrderItemsHandler(IOrderRepository orderRepository, IProductRepository productRepository,
            IAccountRepository accountRepository,
            IUnitOfWork unitOfWork,
            INotificationService notificationService) {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task<Result> Handle(CreateOrderItemsCommand request, CancellationToken cancellationToken)
        {
            // Validate input list
            if (request.Items == null || request.Items.Count == 0)
                return Result.Failure("Items list cannot be empty.");

            var categoryIdCache = new Dictionary<Guid, int>();

            foreach (var item in request.Items)
            {
                if (item.Quantity <= 0)
                    return Result.Failure($"Quantity for product '{item.ProductId}' must be greater than 0.");

                if (!categoryIdCache.ContainsKey(item.ProductId))
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);
                    if (product is null)
                        return Result.Failure($"Product with ID '{item.ProductId}' was not found.");
                    
                    categoryIdCache[item.ProductId] = product.CategoryId;
                }
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
                    int categoryId = categoryIdCache[item.ProductId];
                    for (int i = 0; i < item.Quantity; i++)
                    {
                        order.AddItem(
                            productId: item.ProductId,
                            accountId: request.CreatedBy,
                            note: item.Note ?? string.Empty,
                            orderChannel: request.OrderChannel,
                            categoryId: categoryId
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

            var headChefAccounts = await _accountRepository.GetAccountsByRoleAsync(6, cancellationToken);
            var targetHeadChefIds = headChefAccounts
                .Select(account => account.Id)
                .Distinct()
                .ToList();

            await _notificationService.NotifyOrderStatusChanged(
                orderId: request.OrderId,
                previousStatus: "N/A",
                newStatus: "OrderItemCreated",
                targetAccountIds: targetHeadChefIds,
                cancellationToken: cancellationToken);

            return Result.Success($"{totalCreatedItems} order item(s) created successfully.");
        }
    }
}
