using Application.Dto.Table;
using AutoMapper;
using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Interface.Repository;

namespace Application.Feature.Tables.Queries.GetByAreaId
{
    public class GetTablesByAreaIdHandler : IRequestHandler<GetTablesByAreaIdQueries, Result<IEnumerable<TableDetailResponse>>>
    {
        private readonly ITableRepository _tableRepository;
        private readonly IMapper _mapper;

        public GetTablesByAreaIdHandler(ITableRepository tableRepository, IMapper mapper)
        {
            _tableRepository = tableRepository;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<TableDetailResponse>>> Handle(GetTablesByAreaIdQueries request, CancellationToken cancellationToken)
        {
            var tables = await _tableRepository.GetTablesByAreaIdAsync(request.AreaId);
            
            if (tables == null || !tables.Any())
            {
                return Result<IEnumerable<TableDetailResponse>>.Success("No tables found in this area", new List<TableDetailResponse>());
            }

            var tableDetailResponses = _mapper.Map<IEnumerable<TableDetailResponse>>(tables);
            
            return Result<IEnumerable<TableDetailResponse>>.Success("Get Tables by Area ID Successfully", tableDetailResponses);
        }
    }
}
