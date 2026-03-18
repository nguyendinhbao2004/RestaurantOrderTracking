using Application.Dto.Table;
using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Entities;
using RestaurantOrderTracking.Domain.Enums;
using RestaurantOrderTracking.Domain.Interface.Repository;

namespace Application.Feature.Tables.Queries.GetById
{
    public class GetTableByIdHandler : IRequestHandler<GetTableByIdQueries, Result<TableDetailResponse>>
    {
        private readonly ITableRepository _tableRepository;

        public GetTableByIdHandler(ITableRepository tableRepository)
        {
            _tableRepository = tableRepository;
        }

        public async Task<Result<TableDetailResponse>> Handle(GetTableByIdQueries request, CancellationToken cancellationToken)
        {
            var table = await _tableRepository.GetByIdAsync(request.Id);
            if (table == null)
            {
                return Result<TableDetailResponse>.Failure($"Table with ID {request.Id} not found.");
            }

            var activeOrder = table.Orders
                .Where(order => order.Status != OrderStatus.Completed && order.Status != OrderStatus.Cancelled)
                .OrderByDescending(order => order.CreatedAt)
                .FirstOrDefault();

            var hasOccupiedOrder = table.Orders.Any(order =>
                order.Status == OrderStatus.Confirmed ||
                order.Status == OrderStatus.Preparing ||
                order.Status == OrderStatus.Paying);

            var tableDetailResponse = new TableDetailResponse
            {
                Id = table.Id,
                TableNumber = table.TableNumber,
                AreaName = table.Area?.Name ?? string.Empty,
                Status = hasOccupiedOrder ? TableStatus.Occupied.ToString() : table.Status.ToString(),
                QRCode = table.QRCode,
                Capacity = table.Capacity,
                Orders = MapActiveOrder(activeOrder)
            };

            return Result<TableDetailResponse>.Success("Get Table Detail Successfully", tableDetailResponse);
        }

        private static ActiveOrderDto? MapActiveOrder(Order? activeOrder)
        {
            if (activeOrder == null)
            {
                return null;
            }

            var nonCancelledItems = activeOrder.OrderItems
                .Where(orderItem => orderItem.Status != OrderItemStatus.Cancelled && orderItem.Product != null)
                .ToList();

            var groupedOrderItems = nonCancelledItems
                .GroupBy(orderItem => new { orderItem.ProductId, orderItem.Status, orderItem.Note })
                .Select(group =>
                {
                    var firstItem = group.First();

                    return new OrderItemDto
                    {
                        Id = firstItem.Id,
                        ProductId = group.Key.ProductId,
                        ProductName = firstItem.Product.Name,
                        Price = firstItem.Product.Price,
                        Quantity = group.Count(),
                        Note = group.Key.Note,
                        Status = group.Key.Status.ToString()
                    };
                })
                .ToList();

            return new ActiveOrderDto
            {
                Id = activeOrder.Id,
                OrderType = activeOrder.OrderTypes.ToString(),
                Status = activeOrder.Status.ToString(),
                TotalAmount = nonCancelledItems.Sum(orderItem => orderItem.Product.Price),
                OrderItems = groupedOrderItems
            };
        }
    }
}