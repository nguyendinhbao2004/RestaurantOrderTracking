using MediatR;
using RestaurantOrderTracking.Application.Dto.OrderItem;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Enums;
using RestaurantOrderTracking.Domain.Interface.Repository;

namespace RestaurantOrderTracking.Application.Feature.OrderItem.Queries.GetConfirmedOrderItems
{
    public class GetConfirmedOrderItemsHandler : IRequestHandler<GetConfirmedOrderItemsQuery, PagedResult<ConfirmedOrderItemResponse>>
    {
        private readonly IOrderItemRepository _orderItemRepository;

        public GetConfirmedOrderItemsHandler(IOrderItemRepository orderItemRepository)
        {
            _orderItemRepository = orderItemRepository;
        }

        public async Task<PagedResult<ConfirmedOrderItemResponse>> Handle(GetConfirmedOrderItemsQuery request, CancellationToken cancellationToken)
        {
            var pageIndex = request.PageIndex < 1 ? 1 : request.PageIndex;
            var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

            var (orderItems, totalCount) = await _orderItemRepository.GetPagedOrderItemsByStatusAsync(OrderItemStatus.Confirmed, pageIndex, pageSize);

            var data = orderItems.Select(item => new ConfirmedOrderItemResponse
            {
                OrderItemId = item.Id,
                OrderId = item.OrderId,
                TableId = item.Order?.TableId,
                TableNumber = item.Order?.Table?.TableNumber,
                AreaId = item.Order?.Table?.AreaId,
                AreaName = item.Order?.Table?.Area?.Name,
                ProductId = item.ProductId,
                ProductName = item.Product?.Name ?? string.Empty,
                ProductPrice = item.Product?.Price ?? 0,
                OrderChannel = item.OrderChannel,
                Note = item.Note,
                Status = item.Status.ToString(),
                CreatedAt = item.CreatedAt
            }).ToList();

            return new PagedResult<ConfirmedOrderItemResponse>(data, pageIndex, pageSize, totalCount, "Get confirmed order items successfully");
        }
    }
}