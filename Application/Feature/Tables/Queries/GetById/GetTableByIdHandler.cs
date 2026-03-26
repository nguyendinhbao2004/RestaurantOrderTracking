using Application.Dto.Table;
using AutoMapper;
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
        private readonly IMapper _mapper;

        public GetTableByIdHandler(ITableRepository tableRepository, IMapper mapper)
        {
            _tableRepository = tableRepository;
            _mapper = mapper;
        }

        public async Task<Result<TableDetailResponse>> Handle(GetTableByIdQueries request, CancellationToken cancellationToken)
        {
            var table = await _tableRepository.GetByIdAsync(request.Id);
            if (table == null)
            {
                return Result<TableDetailResponse>.Failure($"Table with ID {request.Id} not found.");
            }

            // Only get the order with status Confirmed
            var confirmedOrder = table.Orders
                .Where(order => order.Status == OrderStatus.Confirmed)
                .OrderByDescending(order => order.CreatedAt)
                .FirstOrDefault();

            var tableDetailResponse = _mapper.Map<TableDetailResponse>(table);
            tableDetailResponse.Orders = MapActiveOrder(confirmedOrder);

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