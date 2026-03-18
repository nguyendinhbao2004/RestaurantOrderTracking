using MediatR;
using RestaurantOrderTracking.Application.Dto.Table;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Enums;
using RestaurantOrderTracking.Domain.Interface.Repository;

namespace Application.Feature.Tables.Queries.GetAllTable
{
    public class GetAllTableHandler : IRequestHandler<GetAllTableQueries, PagedResult<TableResponse>>
    {
        private readonly ITableRepository _tableRepository;

        public GetAllTableHandler(ITableRepository tableRepository)
        {
            _tableRepository = tableRepository;
        }

        public async Task<PagedResult<TableResponse>> Handle(GetAllTableQueries request, CancellationToken cancellationToken)
        {
            var (tables, totalRecords) = await _tableRepository.GetPagedTablesAsync(request.Keyword, request.PageIndex, request.PageSize);

            var tableResponses = tables.Select(table => new TableResponse
            {
                Id = table.Id,
                TableNumber = table.TableNumber,
                AreaName = table.Area?.Name ?? string.Empty,
                Status = table.Orders.Any(order =>
                    order.Status == OrderStatus.Confirmed ||
                    order.Status == OrderStatus.Preparing ||
                    order.Status == OrderStatus.Paying)
                        ? TableStatus.Occupied.ToString()
                        : table.Status.ToString(),
                Capacity = table.Capacity
            }).ToList();

            return new PagedResult<TableResponse>(tableResponses, request.PageIndex, request.PageSize, totalRecords, "Get Table Successfully");
        }
    }
}