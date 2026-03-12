using AutoMapper;
using MediatR;
using RestaurantOrderTracking.Application.Dto.Table;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Interface.Repository;

namespace Application.Feature.Tables.Queries.GetAllTable
{
    public class GetAllTableHandler : IRequestHandler<GetAllTableQueries, PagedResult<TableResponse>>
    {
        private readonly ITableRepository _tableRepository;
        private readonly IMapper _mapper;
        public GetAllTableHandler(ITableRepository tableRepository, IMapper mapper)
        {
            _tableRepository = tableRepository;
            _mapper = mapper;
        }
        public async Task<PagedResult<TableResponse>> Handle(GetAllTableQueries request, CancellationToken cancellationToken)
        {
            var (tables, totalRecords) = await _tableRepository.GetPagedTablesAsync(request.Keyword, request.PageIndex, request.PageSize);
            var tableResponses = _mapper.Map<List<TableResponse>>(tables);
            return new PagedResult<TableResponse>(tableResponses, request.PageIndex, request.PageSize, totalRecords, "Get Table Successfully");
        }
    }
}