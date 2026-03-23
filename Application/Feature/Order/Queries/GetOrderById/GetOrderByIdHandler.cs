using MediatR;
using RestaurantOrderTracking.Application.Dto.Order;
using RestaurantOrderTracking.Application.Dto.OrderItem;
using RestaurantOrderTracking.Domain.Interface.Repository;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantOrderTracking.Application.Feature.Order.Queries.GetOrderById
{
    public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, OrderDetailResponse>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrderByIdHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<OrderDetailResponse> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetOrderByIdWithDetailsAsync(request.Id);

            if (order is null)
                throw new KeyNotFoundException($"Order with id {request.Id} not found.");

            var response = new OrderDetailResponse
            {
                Id = order.Id,
                TableId = order.TableId,
                TableNumber = order.Table?.TableNumber,
                OrderType = order.OrderTypes.ToString(),
                Status = order.Status.ToString(),
                WaiterId = order.WaiterId,
                WaiterName = order.Waiter?.FullName,
                CustomerId = order.CustomerId,
                CustomerName = order.Customer?.Name,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                OrderItems = order.OrderItems.Select(item => new OrderItemDetailResponse
                {
                    Id = item.Id,
                    OrderId = item.OrderId,
                    TableId = order.TableId,
                    TableNumber = order.Table?.TableNumber,
                    AreaId = order.Table?.AreaId,
                    AreaName = order.Table?.Area?.Name,
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
                }).ToList()
            };

            return response;
        }
    }
}
