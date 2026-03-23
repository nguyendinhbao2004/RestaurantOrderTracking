using MediatR;
using RestaurantOrderTracking.Application.Dto.OrderItem;
using RestaurantOrderTracking.Domain.Enums;
using RestaurantOrderTracking.Domain.Interface.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantOrderTracking.Application.Feature.OrderItem.Queries.GetOrderItemsByStatus
{
    public class GetOrderItemsByStatusHandler : IRequestHandler<GetOrderItemsByStatusQuery, List<OrderItemDetailResponse>>
    {
        private readonly IOrderItemRepository _orderItemRepository;

        public GetOrderItemsByStatusHandler(IOrderItemRepository orderItemRepository)
        {
            _orderItemRepository = orderItemRepository;
        }

        public async Task<List<OrderItemDetailResponse>> Handle(GetOrderItemsByStatusQuery request, CancellationToken cancellationToken)
        {
            var statusEnum = (OrderItemStatus)request.Status;
            var orderItems = await _orderItemRepository.GetOrderItemsByStatusAsync(statusEnum);

            return orderItems.Select(item => new OrderItemDetailResponse
            {
                Id = item.Id,
                OrderId = item.OrderId,
                TableId = item.Order?.TableId,
                TableNumber = item.Order?.Table?.TableNumber,
                ProductId = item.ProductId,
                ProductName = item.Product?.Name ?? string.Empty,
                ProductPrice = item.Product?.Price ?? 0,
                ChefAccountId = item.ChefAccountId,
                ChefName = item.ChefAccount?.FullName,
                WaiterAccountId = item.WaiterAccountId,
                WaiterName = item.WaiterAccount?.FullName,
                OrderChannel = item.OrderChannel,
                Note = item.Note,
                Status = item.Status.ToString(),
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt
            }).ToList();
        }
    }
}
